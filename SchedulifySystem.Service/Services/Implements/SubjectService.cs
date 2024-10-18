using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create Subject
        public async Task<BaseResponseModel> CreateSubject(SubjectAddModel subjectAddModel)
        {
            var existSubject = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.SubjectName.ToLower() == subjectAddModel.SubjectName.ToLower()) && (t.SchoolId == subjectAddModel.SchoolId) && (t.IsDeleted == false));
            var existSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(subjectAddModel.SchoolId) ?? throw new NotExistsException($"School is not existed with id {subjectAddModel.SchoolId}");

            if (existSubject.FirstOrDefault() != null)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.SUBJECT_NAME_EXISTED
                };
            }

            var baseAbbreviation = subjectAddModel.Abbreviation.ToLower();
            var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation) && (t.SchoolId == subjectAddModel.SchoolId) && (t.IsDeleted == false)));

            string newAbbreviation = baseAbbreviation;
            int counter = 1;

            while (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == newAbbreviation))
            {
                var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");

                if (match.Success)
                {
                    var textPart = match.Groups[1].Value;
                    var numberPart = int.Parse(match.Groups[2].Value);
                    newAbbreviation = $"{textPart}{numberPart + counter}";
                }
                else
                {
                    newAbbreviation = $"{baseAbbreviation}{counter}";
                }
                counter++;
            }

            subjectAddModel.Abbreviation = newAbbreviation.ToUpper();

            var subjectEntity = _mapper.Map<Subject>(subjectAddModel);
            await _unitOfWork.SubjectRepo.AddAsync(subjectEntity);
            await _unitOfWork.SaveChangesAsync();

            subjectEntity.School = existSchool;
            var result = _mapper.Map<SubjectViewModel>(subjectEntity);

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.ADD_SUBJECT_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region Create Subject List
        public async Task<BaseResponseModel> CreateSubjectList(int schoolId, List<SubjectAddListModel> subjectAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");
                    var addedSubjects = new List<string>();

                    var usedAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var subject in subjectAddModel)
                    {
                        subject.SubjectName = subject.SubjectName.Trim();
                        subject.Abbreviation = subject.Abbreviation.Trim();
                    }

                    //check duplicate name in list
                    var duplicateSubjectName = subjectAddModel
                        .GroupBy(b => b.SubjectName, StringComparer.OrdinalIgnoreCase)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicateSubjectName.Count != 0)
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.SUBJECT_NAME_ALREADY_EXIST,
                            Result = duplicateSubjectName
                        };
                    }

                    var existingSubjects = await _unitOfWork.SubjectRepo
                        .GetAsync(filter: t => t.SchoolId == schoolId);

                    // duplicate name in database
                    var existingSubjectNames = existingSubjects
                        .Select(s => s.SubjectName)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var dbDuplicateSubjectNames = subjectAddModel
                        .Where(s => existingSubjectNames.Contains(s.SubjectName))
                        .Select(s => s.SubjectName)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (dbDuplicateSubjectNames.Count != 0)
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.SUBJECT_NAME_ALREADY_EXIST,
                            Result = dbDuplicateSubjectNames
                        };
                    }

                    foreach (var createSubjectModel in subjectAddModel)
                    {
                        var baseAbbreviation = createSubjectModel.Abbreviation.ToLower();
                        var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation) && (t.SchoolId == schoolId) && (t.IsDeleted == false)));

                        string newAbbreviation = baseAbbreviation;
                        int counter = 1;

                        while (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == newAbbreviation) || usedAbbreviations.Contains(newAbbreviation))
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");

                            if (match.Success)
                            {
                                var textPart = match.Groups[1].Value;
                                var numberPart = int.Parse(match.Groups[2].Value);
                                newAbbreviation = $"{textPart}{numberPart + counter}";
                            }
                            else
                            {
                                newAbbreviation = $"{baseAbbreviation}{counter}";
                            }
                            counter++;
                        }

                        createSubjectModel.Abbreviation = newAbbreviation.ToUpper();
                        usedAbbreviations.Add(createSubjectModel.Abbreviation);

                        var newSubject = _mapper.Map<Subject>(createSubjectModel);
                        newSubject.SchoolId = schoolId;
                        await _unitOfWork.SubjectRepo.AddAsync(newSubject);
                        addedSubjects.Add(createSubjectModel.SubjectName);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status201Created,
                        Message = ConstantResponse.ADD_SUBJECT_LIST_SUCCESS,
                        Result = new
                        {
                            AddedSubjects = addedSubjects,
                        }
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = $"Error: {ex.Message}"
                    };
                }
            }
        }
        #endregion

        #region Get subject by id
        public async Task<BaseResponseModel> GetSubjectById(int subjectId)
        {
            var subjects = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId) ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(subjects.SchoolId ?? -1);
            subjects.School = school;
            var subject = _mapper.Map<SubjectViewModel>(subjects);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_SUCCESS,
                Result = subject
            };
        }
        #endregion

        #region get subject list by school id
        public async Task<BaseResponseModel> GetSubjectBySchoolId(int schoolId, string? subjectName, bool includeDeleted, int pageIndex, int pageSize)
        {
            if (schoolId != 0)
            {
                var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            }
            var subject = await _unitOfWork.SubjectRepo.ToPaginationIncludeAsync(
                pageSize, pageIndex,
                filter: t => (schoolId == 0 || t.SchoolId == schoolId) && (includeDeleted ? true : t.IsDeleted == false) && (subjectName == null || t.SubjectName.ToLower().Contains(subjectName.ToLower())),
                include: query => query.Include(t => t.School));
            if (subject.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.SUBJECT_NOT_EXISTED
                };
            }
            var result = _mapper.Map<Pagination<SubjectViewModel>>(subject);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_LIST_SUCCESS,
                Result = result
            };
        }

        #endregion

        #region update subject by id
        public async Task<BaseResponseModel> UpdateSubjectById(int subjectId, SubjectUpdateModel subjectUpdateModel)
        {
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId) ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

            subjectUpdateModel.SubjectName = subjectUpdateModel.SubjectName.Trim();
            subjectUpdateModel.Abbreviation = subjectUpdateModel.Abbreviation.Trim();
            if (subjectUpdateModel.SubjectName != subject.SubjectName)
            {
                var duplicateName = await _unitOfWork.SubjectRepo.GetAsync(
                                    filter: t => (t.SubjectName.ToLower().Equals(subjectUpdateModel.SubjectName.ToLower()))
                                    && (t.IsDeleted == false));
                if (duplicateName.Any())
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.SUBJECT_NAME_EXISTED
                    };
                }
            }


            if (subjectUpdateModel.Abbreviation != subject.Abbreviation)
            {
                var baseAbbreviation = subjectUpdateModel.Abbreviation.ToLower();
                var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(
                    filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation)
                    && (t.IsDeleted == false)));

                string newAbbreviation = baseAbbreviation;
                int counter = 1;

                while (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == newAbbreviation))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");

                    if (match.Success)
                    {
                        var textPart = match.Groups[1].Value;
                        var numberPart = int.Parse(match.Groups[2].Value);
                        newAbbreviation = $"{textPart}{numberPart + counter}";
                    }
                    else
                    {
                        newAbbreviation = $"{baseAbbreviation}{counter}";
                    }
                    counter++;
                }

                subjectUpdateModel.Abbreviation = newAbbreviation.ToUpper();
            }

            var subjectUpdate = _mapper.Map(subjectUpdateModel, subject);
            subjectUpdate.UpdateDate = DateTime.UtcNow;
            _unitOfWork.SubjectRepo.Update(subjectUpdate);
            await _unitOfWork.SaveChangesAsync();
            var schoool = await _unitOfWork.SchoolRepo.GetByIdAsync(subjectUpdate.SchoolId ?? -1);
            subjectUpdate.School = schoool;

            var result = _mapper.Map<SubjectViewModel>(subjectUpdate);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_SUBJECT_SUCCESS,
                Result = result
            };
        }
        #endregion
    }
}
