using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
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
            var existSubject = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.SubjectName.ToLower() == subjectAddModel.SubjectName.ToLower())&&(t.SchoolId == subjectAddModel.SchoolId));
            var existSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(subjectAddModel.SchoolId) ?? throw new NotExistsException($"School is not existed with id {subjectAddModel.SchoolId}");

            if (existSubject.FirstOrDefault() != null)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Subject name {subjectAddModel.SubjectName} already exists in school {existSchool.Name}!"
                };
            }

            var baseAbbreviation = subjectAddModel.Abbreviation.ToLower();
            var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation)&&(t.SchoolId == subjectAddModel.SchoolId)));

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
                Message = "Create subject success",
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
                    var skippedSubjects = new List<string>();

                    var usedAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


                    foreach (var createSubjectModel in subjectAddModel)
                    {
                        var existSubject = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.SubjectName.ToLower() == createSubjectModel.SubjectName.ToLower()) && (t.SchoolId == schoolId));
                        if (existSubject.FirstOrDefault() != null)
                        {
                            skippedSubjects.Add($"Subject {createSubjectModel.SubjectName} is already existed");
                            continue;
                        }

                        var baseAbbreviation = createSubjectModel.Abbreviation.ToLower();
                        var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => (t.Abbreviation.ToLower().StartsWith(baseAbbreviation) && (t.SchoolId == schoolId)));

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
                        addedSubjects.Add($"{createSubjectModel.SubjectName} is added");
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status201Created,
                        Message = "Operation completed",
                        Result = new
                        {
                            AddedTeachers = addedSubjects,
                            SkippedTeachers = skippedSubjects
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

        #region get subject list by school id
        public async Task<BaseResponseModel> GetSubjectBySchoolId(int schoolId, string? schoolName, bool includeDeleted, int pageIndex, int pageSize)
        {
            if (schoolId != 0)
            {
                var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");
            }
            var subject = await _unitOfWork.SubjectRepo.ToPaginationIncludeAsync(
                pageSize, pageIndex,
                filter: t => ( schoolId == 0 || t.SchoolId == schoolId) && (includeDeleted ? true : t.IsDeleted == false) && (schoolName == null || t.School.Name.ToLower().Contains(schoolName.ToLower())),
                include: query => query.Include(t => t.School));
            if (subject.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Not found subject with school id {schoolId}"
                };
            }
            var result = _mapper.Map<Pagination<SubjectViewModel>>(subject);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Get subject list successful",
                Result = result
            };
        }
        #endregion
    }
}
