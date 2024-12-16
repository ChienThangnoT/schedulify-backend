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

        //#region Create Subject
        //public async Task<BaseResponseModel> CreateSubject(SubjectAddModel subjectAddModel)
        //{
        //    var existSubject = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.SubjectName.ToLower() == subjectAddModel.SubjectName.ToLower()) && (t.IsDeleted == false));

        //    if (existSubject.FirstOrDefault() != null)
        //    {
        //        return new BaseResponseModel()
        //        {
        //            Status = StatusCodes.Status400BadRequest,
        //            Message = ConstantResponse.SUBJECT_NAME_EXISTED
        //        };
        //    }

        //    var baseAbbreviation = subjectAddModel.Abbreviation.ToLower();
        //    var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation) && (t.IsDeleted == false)));

        //    string newAbbreviation = baseAbbreviation;
        //    int counter = 1;

        //    while (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == newAbbreviation))
        //    {
        //        var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");

        //        if (match.Success)
        //        {
        //            var textPart = match.Groups[1].Value;
        //            var numberPart = int.Parse(match.Groups[2].Value);
        //            newAbbreviation = $"{textPart}{numberPart + counter}";
        //        }
        //        else
        //        {
        //            newAbbreviation = $"{baseAbbreviation}{counter}";
        //        }
        //        counter++;
        //    }

        //    subjectAddModel.Abbreviation = newAbbreviation.ToUpper();

        //    var subjectEntity = _mapper.Map<Subject>(subjectAddModel);
        //    await _unitOfWork.SubjectRepo.AddAsync(subjectEntity);
        //    await _unitOfWork.SaveChangesAsync();
        //    var result = _mapper.Map<SubjectViewModel>(subjectEntity);

        //    return new BaseResponseModel()
        //    {
        //        Status = StatusCodes.Status201Created,
        //        Message = ConstantResponse.ADD_SUBJECT_SUCCESS,
        //        Result = result
        //    };
        //}
        //#endregion

        #region Create Subject List
        public async Task<BaseResponseModel> CreateSubjectList(int schoolYearId, List<SubjectAddListModel> subjectAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var _ = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => t.IsDeleted == false)
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
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
                            Message = ConstantResponse.SUBJECT_NAME_ALREADY_EXIST_IN_LIST,
                            Result = duplicateSubjectName
                        };
                    }

                    var existingSubjects = await _unitOfWork.SubjectRepo
                        .GetAsync(filter: t => !t.IsDeleted);

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
                        var baseAbbreviation = createSubjectModel.Abbreviation.ToLower().Trim();

                        var existingAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => t.Abbreviation.ToLower().StartsWith(baseAbbreviation) && t.IsDeleted == false);

                        // include abbre in current transaction
                        var duplicateAbbre = existingAbbre.Concat(
                            subjectAddModel.Where(s => s != createSubjectModel && s.Abbreviation.ToLower().StartsWith(baseAbbreviation))
                            .Select(s => new Subject { Abbreviation = s.Abbreviation.ToLower() })
                        ).ToList();

                        string newAbbreviation = baseAbbreviation;
                        int counter = 0;

                        var existingNumbers = duplicateAbbre
                            .Select(t => System.Text.RegularExpressions.Regex.Match(t.Abbreviation, @"^.*?(\d+)$"))
                            .Where(m => m.Success)
                            .Select(m => int.Parse(m.Groups[1].Value))
                            .ToList();

                        if (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == baseAbbreviation))
                        {
                            if (existingNumbers.Count > 0)
                            {
                                counter = existingNumbers.Max() + 1;
                            }
                            else
                            {
                                counter = 1;
                            }

                            newAbbreviation = $"{baseAbbreviation}{counter}";
                        }
                        else if (existingNumbers.Count > 0)
                        {
                            counter = existingNumbers.Max() + 1;
                            newAbbreviation = $"{baseAbbreviation}{counter}";
                        }

                        while (duplicateAbbre.Any(t => t.Abbreviation.ToLower() == newAbbreviation) || usedAbbreviations.Contains(newAbbreviation))
                        {
                            counter++;
                            newAbbreviation = $"{baseAbbreviation}{counter}";
                        }

                        createSubjectModel.Abbreviation = newAbbreviation.ToUpper();
                        usedAbbreviations.Add(createSubjectModel.Abbreviation);

                        var newSubject = _mapper.Map<Subject>(createSubjectModel);
                        newSubject.SchoolYearId = schoolYearId;
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
            var subjects = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId, include: query => query.Include(t => t.SchoolYear) 
                ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED));
            var subject = _mapper.Map<SubjectViewModel>(subjects);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_SUCCESS,
                Result = subject
            };
        }
        #endregion

        #region get subject list with filter
        public async Task<BaseResponseModel> GetSubjectById(int schoolYearId, int? id, string? subjectName, bool? isRequired, bool includeDeleted, int pageIndex, int pageSize)
        {
            if (id != null)
            {
                var _ = await _unitOfWork.SubjectRepo.GetAsync(filter: t => t.Id == id && t.SchoolYearId == schoolYearId && t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
            }
            var subject = await _unitOfWork.SubjectRepo.ToPaginationIncludeAsync(
                pageSize, 
                pageIndex,
                filter: t => t.SchoolYearId == schoolYearId && (id == null || t.Id == id) 
                            && (isRequired == null || t.IsRequired == isRequired) && (includeDeleted || t.IsDeleted == false)
                            && (subjectName == null || t.SubjectName.ToLower().Contains(subjectName.ToLower())),
                include: query => query.Include(t => t.SchoolYear)
                );
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
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

            if (!string.IsNullOrWhiteSpace(subjectUpdateModel.SubjectName) &&
                subjectUpdateModel.SubjectName.Trim() != subject.SubjectName)
            {
                var duplicateName = await _unitOfWork.SubjectRepo.GetAsync(
                    filter: t => t.SubjectName.ToLower() == subjectUpdateModel.SubjectName.ToLower()
                                 && !t.IsDeleted);

                if (duplicateName.Any())
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.SUBJECT_NAME_EXISTED
                    };
                }
                subject.SubjectName = subjectUpdateModel.SubjectName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(subjectUpdateModel.Abbreviation) &&
                subjectUpdateModel.Abbreviation.Trim() != subject.Abbreviation)
            {
                var baseAbbreviation = subjectUpdateModel.Abbreviation.ToLower();
                var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(
                    filter: t => t.Abbreviation.ToLower().StartsWith(baseAbbreviation)
                                 && !t.IsDeleted);

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

                subject.Abbreviation = newAbbreviation.ToUpper();
            }

            if (subjectUpdateModel.Description != null)
            {
                subject.Description = subjectUpdateModel.Description.Trim();
            }

            if (subjectUpdateModel.IsRequired.HasValue)
            {
                subject.IsRequired = subjectUpdateModel.IsRequired.Value;
            }

            if (subjectUpdateModel.TotalSlotInYear.HasValue)
            {
                subject.TotalSlotInYear = subjectUpdateModel.TotalSlotInYear.Value;
            }

            if (subjectUpdateModel.SlotSpecialized.HasValue)
            {
                subject.SlotSpecialized = subjectUpdateModel.SlotSpecialized.Value;
            }

            subject.UpdateDate = DateTime.UtcNow;

            _unitOfWork.SubjectRepo.Update(subject);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<SubjectViewModel>(subject);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_SUBJECT_SUCCESS,
                Result = result
            };
        }

        #endregion

        #region delete subejct
        public async Task<BaseResponseModel> DeleteSubjectById(int subjectId)
        {
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
            var subjectIsUsed = await _unitOfWork.CurriculumDetailRepo.GetAsync(filter: t => t.SubjectId == subjectId && t.IsDeleted == false);
            if(subjectIsUsed != null)
            {
                throw new DefaultException(ConstantResponse.SUBJECT_ALREADY_USED);
            }
            subject.IsDeleted = true;
            _unitOfWork.SubjectRepo.Update(subject);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.DELETE_SUBJECT_SUCCESS
            };
        }
        #endregion
    }
}
