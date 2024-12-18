﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class SubjectGroupService : ISubjectGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int REQUIRE_ELECTIVE_SUBJECT = 4;
        private const int REQUIRE_SPECIALIZED_SUBJECT = 3;
        public SubjectGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region create subject group
        public async Task<BaseResponseModel> CreateSubjectGroup(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, include: query => query.Include(q => q.Terms))
                        ?? throw new NotExistsException($"School not found with id {schoolId}");
                    var schoolYear = (await _unitOfWork.SchoolYearRepo.ToPaginationIncludeAsync(
                        filter: sy => sy.Id == subjectGroupAddModel.SchoolYearId,
                        include: query => query.Include(sy => sy.Terms))).Items.FirstOrDefault()
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

                    var checkExistSubjectGroup = await _unitOfWork.SubjectGroupRepo.GetAsync(
                        filter: t => t.GroupName.ToLower() == subjectGroupAddModel.GroupName.ToLower()
                        || t.GroupCode.ToLower() == subjectGroupAddModel.GroupCode.ToLower());

                    if (checkExistSubjectGroup.Any())
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.SUBJECT_GROUP_NAME_OR_CODE_EXISTED
                        };
                    }
                    // check enough number subject
                    if (subjectGroupAddModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT
                        || subjectGroupAddModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    var subjectGroupAdd = _mapper.Map<SubjectGroup>(subjectGroupAddModel);
                    subjectGroupAdd.SchoolId = schoolId;

                    await _unitOfWork.SubjectGroupRepo.AddAsync(subjectGroupAdd);
                    //save to have group id
                    await _unitOfWork.SaveChangesAsync();
                    //generate data subject

                    //ElectiveSubjects
                    var newSubjectInGroupAdded = new List<SubjectInGroup>();
                    foreach (int subjectId in subjectGroupAddModel.ElectiveSubjectIds)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
                        if (checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.REQUIRE_ELECTIVE_SUBJECT,
                                Result = subjectId
                            };

                        var newSubjectInGroup = new SubjectInGroup
                        {
                            SubjectId = subjectId,
                            SubjectGroupId = subjectGroupAdd.Id,
                            IsSpecialized = subjectGroupAddModel.SpecializedSubjectIds.Contains(subjectId)
                        };

                        foreach (Term term in school.Terms)
                        {
                            newSubjectInGroup.CreateDate = DateTime.UtcNow;
                            newSubjectInGroup.TermId = term.Id;
                            _unitOfWork.SubjectInGroupRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                            newSubjectInGroupAdded.Add(newSubjectInGroup);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    //add required subject
                    var requiredSubjects = await _unitOfWork.SubjectRepo.GetAsync(
                        filter: t => t.SchoolId == schoolId && t.IsDeleted == false && t.IsRequired == true);
                    //var requiredSubjectList = new List<SubjectInGroup>();
                    foreach (var subject in requiredSubjects)
                    {
                        var newSubjectRequiredInGroup = new SubjectInGroup
                        {
                            SubjectId = subject.Id,
                            SubjectGroupId = subjectGroupAdd.Id,
                            IsSpecialized = subjectGroupAddModel.SpecializedSubjectIds.Contains(subject.Id)
                        };

                        foreach (Term term in school.Terms)
                        {
                            newSubjectRequiredInGroup.CreateDate = DateTime.UtcNow;
                            newSubjectRequiredInGroup.TermId = term.Id;
                            await _unitOfWork.SubjectInGroupRepo.AddAsync(newSubjectRequiredInGroup.ShallowCopy());
                            newSubjectInGroupAdded.Add(newSubjectRequiredInGroup);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    //Specialized Subject
                    var specializedSubjectNotInElectiveSubjects = subjectGroupAddModel.SpecializedSubjectIds
                        .Where(s => !subjectGroupAddModel.ElectiveSubjectIds.Contains(s));

                    foreach (int subjectId in specializedSubjectNotInElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (!checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.INVALID_SPECIALIZED_SUBJECT,
                                Result = subjectId
                            };
                    }

                    //var subjectInGroupAdded = newSubjectInGroupAdded.FirstOrDefault(s => subjectGroupAddModel.SpecializedSubjectIds.Contains(s.SubjectId));
                    transaction.Commit();
                    //var subjectInGroupAdded = newSubjectInGroupAdded
                    //    .Where(c => subjectGroupAddModel.SpecializedSubjectIds.Contains(c.SubjectId)).ToList();

                    var updateSpecialSubject = await _unitOfWork.SubjectInGroupRepo.GetAsync(
                        filter: t => t.SubjectGroupId == subjectGroupAdd.Id && subjectGroupAddModel.SpecializedSubjectIds.Contains(t.SubjectId));

                    foreach (var subjectInGroup in updateSpecialSubject)
                    {
                        subjectInGroup.IsSpecialized = true;
                        _unitOfWork.SubjectInGroupRepo.Update(subjectInGroup);
                    }
                    await _unitOfWork.SaveChangesAsync();
                    var result = _mapper.Map<SubjectGroupViewModel>(subjectGroupAdd);
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status201Created,
                        Message = ConstantResponse.ADD_SUBJECT_GROUP_SUCCESS,
                        Result = result
                    };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }
        #endregion

        #region Get Subject Group Detail
        public async Task<BaseResponseModel> GetSubjectGroupDetail(int subjectGroupId)
        {
            var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetV2Async(
                filter: t => t.Id == subjectGroupId && t.IsDeleted == false,
                include: query => query.Include(c => c.SubjectInGroups)
                           .ThenInclude(sg => sg.Subject));

            if (subjectGroup == null || !subjectGroup.Any())
            {
                throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
            }

            var subjectGroupDb = subjectGroup.FirstOrDefault();

            var result = _mapper.Map<SubjectGroupViewDetailModel>(subjectGroupDb);


            result.SubjectSelectiveViews = new List<SubjectViewDetailModel>();
            result.SubjectSpecializedtViews = new List<SubjectViewDetailModel>();
            result.SubjectRequiredViews = new List<SubjectViewDetailModel>();
            result.StudentClassViews = new List<StudentClassViewName>();

            var listSBInGroup = subjectGroupDb.SubjectInGroups.ToList();

            var studentClass = await _unitOfWork.StudentClassesRepo.GetAsync(
                filter: t => t.SubjectGroupId == subjectGroupDb.Id);
            if (studentClass.Any() || studentClass != null)
            {
                foreach (var student in studentClass)
                {
                    var studenClassName = new StudentClassViewName
                    {
                        StudentClassName = student.Name
                    };
                    result.StudentClassViews.Add(studenClassName);
                }
            }

            var specializedSubjectIds = listSBInGroup
                .Where(sig => sig.IsSpecialized)
                .Select(sig => sig.SubjectId)
                .ToList();

            foreach (var item in listSBInGroup)
            {
                var subjectDetail = new SubjectViewDetailModel
                {
                    Id = item.Id,
                    SubjectName = item.Subject?.SubjectName,
                    Abbreviation = item.Subject?.Abbreviation,
                    IsRequired = item.Subject?.IsRequired ?? false,
                    Description = item.Subject?.Description,
                    MainSlotPerWeek = item.MainSlotPerWeek,
                    SubSlotPerWeek = item.SubSlotPerWeek,
                    TootalSlotPerWeek = item.MainSlotPerWeek + item.SubSlotPerWeek,
                    IsSpecialized = item.IsSpecialized,
                    IsDoublePeriod = item.IsDoublePeriod,
                    SlotPerTerm = item.SlotPerTerm,
                    TermId = item.TermId ?? 0,
                    TotalSlotInYear = item.Subject?.TotalSlotInYear,
                    SlotSpecialized = item.Subject?.SlotSpecialized ?? 35,
                    SubjectGroupType = (ESubjectGroupType)(item.Subject?.SubjectGroupType ?? 0)
                };

                if (item.Subject?.IsRequired == true)
                {
                    result.SubjectRequiredViews.Add(subjectDetail);

                    if (specializedSubjectIds.Contains(item.SubjectId))
                    {
                        result.SubjectSpecializedtViews.Add(subjectDetail);
                    }
                }

                else if (item.Subject?.IsRequired == false)
                {
                    result.SubjectSelectiveViews.Add(subjectDetail);

                    if (specializedSubjectIds.Contains(item.SubjectId))
                    {
                        result.SubjectSpecializedtViews.Add(subjectDetail);
                    }
                }
                else if (item.IsSpecialized)
                {
                    result.SubjectSpecializedtViews.Add(subjectDetail);
                }
            }

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_GROUP_DETAIL_SUCCESS,
                Result = result
            };
        }


        #endregion

        #region get subject groups
        public async Task<BaseResponseModel> GetSubjectGroups(int schoolId, int? subjectGroupId, EGrade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (subjectGroupId != null)
            {
                var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetByIdAsync((int)subjectGroupId)
                    ?? throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
            }
            if (schoolYearId != null)
            {
                var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)schoolYearId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            }

            var subjects = await _unitOfWork.SubjectGroupRepo.GetPaginationAsync(
                filter: t => t.SchoolId == schoolId
                && (subjectGroupId == null || t.Id == subjectGroupId)
                && (grade == null || t.Grade == (int)grade)
                && (schoolYearId == null || t.SchoolYearId == schoolYearId)
                && t.IsDeleted == includeDeleted,
                orderBy: q => q.OrderBy(s => s.GroupCode),
                pageIndex: pageIndex,
                pageSize: pageSize
                );
            if (subjects.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.GET_SUBJECT_GROUP_LIST_FAILED
                };
            }
            var result = _mapper.Map<Pagination<SubjectGroupViewModel>>(subjects);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_GROUP_LIST_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region Update Subject Group
        public async Task<BaseResponseModel> UpdateSubjectGroup(int subjectGroupId, SubjectGroupUpdateModel subjectGroupUpdateModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetV2Async(
                    filter: t => t.Id == subjectGroupId && t.IsDeleted == false,
                    include: query => query.Include(c => c.SubjectInGroups)
                               .ThenInclude(sg => sg.Subject).ThenInclude(sc => sc.School)
                               .Include(sg => sg.SchoolYear)
                               .ThenInclude(sy => sy.Terms));

                    if (subjectGroup == null || !subjectGroup.Any())
                    {
                        throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
                    }

                    var subjectGroupDb = subjectGroup.FirstOrDefault();

                    // check duplicate groupName hoặc groupCode
                    var checkExistSubjectGroup = await _unitOfWork.SubjectGroupRepo.GetAsync(
                        filter: t => (t.GroupName.ToLower() == subjectGroupUpdateModel.GroupName.ToLower() ||
                                      t.GroupCode.ToLower() == subjectGroupUpdateModel.GroupCode.ToLower()) &&
                                      t.Id != subjectGroupId
                    );

                    if (checkExistSubjectGroup.Any())
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.SUBJECT_GROUP_NAME_OR_CODE_EXISTED
                        };
                    }

                    // check enough number subject
                    if (subjectGroupUpdateModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT
                        || subjectGroupUpdateModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    // ko update grade nếu đã được sử dụng bởi lớp nào
                    if (subjectGroupUpdateModel.Grade != 0 && subjectGroupUpdateModel.Grade != (EGrade)subjectGroupDb.Grade)
                    {
                        var classes = await _unitOfWork.StudentClassesRepo.GetAsync(
                            filter: t => t.Grade == (int)subjectGroupUpdateModel.Grade && t.SubjectGroupId == subjectGroupId && !t.IsDeleted);

                        if (classes.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.SUBJECT_GROUP_HAS_SUBJECTS_REGISTERED
                            };
                        }

                        subjectGroupDb.Grade = (int)subjectGroupUpdateModel.Grade;
                    }

                    if (!string.IsNullOrEmpty(subjectGroupUpdateModel.GroupName))
                    {
                        subjectGroupDb.GroupName = subjectGroupUpdateModel.GroupName.Trim();
                    }

                    if (!string.IsNullOrEmpty(subjectGroupUpdateModel.GroupCode))
                    {
                        subjectGroupDb.GroupCode = subjectGroupUpdateModel.GroupCode.ToUpper().Trim();
                    }

                    if (!string.IsNullOrEmpty(subjectGroupUpdateModel.GroupDescription))
                    {
                        subjectGroupDb.GroupDescription = subjectGroupUpdateModel.GroupDescription.Trim();
                    }

                    // get list môn học hiện tại trong tổ hợp
                    var existingSubjectIds = subjectGroupDb.SubjectInGroups
                        .Where(query => query.Subject.IsRequired != true)
                        .Select(s => s.SubjectId).ToList();

                    // xác định các môn tự chọn mới và các môn cần xóa bằng except
                    var newElectiveSubjects = subjectGroupUpdateModel.ElectiveSubjectIds.Except(existingSubjectIds).ToList();

                    var subjectsToRemove = existingSubjectIds
                        .Except(subjectGroupUpdateModel.ElectiveSubjectIds)
                        .Except(subjectGroupUpdateModel.SpecializedSubjectIds)
                        .ToList();

                    // xóa các môn không có trong danh sách cập nhật từ SubjectInGroup
                    if (subjectsToRemove.Any() || subjectsToRemove.Count != 0)
                    {
                        var subjectsToRemoveEntities = subjectGroupDb.SubjectInGroups
                            .Where(s => subjectsToRemove.Contains(s.SubjectId))
                            .ToList();
                        _unitOfWork.SubjectInGroupRepo.RemoveRange(subjectsToRemoveEntities);
                    }

                    // thêm các môn tự chọn mới
                    foreach (var subjectId in newElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.REQUIRE_ELECTIVE_SUBJECT,
                                Result = subjectId
                            };

                        foreach (var term in subjectGroupDb.School.Terms)
                        {
                            var newSubjectInGroup = new SubjectInGroup
                            {
                                SubjectId = subjectId,
                                SubjectGroupId = subjectGroupId,
                                IsSpecialized = subjectGroupUpdateModel.SpecializedSubjectIds.Contains(subjectId),
                                TermId = term.Id
                            };
                            await _unitOfWork.SubjectInGroupRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();

                    //Specialized Subject
                    var specializedSubjectNotInElectiveSubjects = subjectGroupUpdateModel.SpecializedSubjectIds
                        .Where(s => !subjectGroupUpdateModel.ElectiveSubjectIds.Contains(s));

                    foreach (int subjectId in specializedSubjectNotInElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (!checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.INVALID_SPECIALIZED_SUBJECT,
                                Result = subjectId
                            };
                    }

                    transaction.Commit();

                    var subjectInDB = await _unitOfWork.SubjectGroupRepo.GetV2Async(
                        filter: t => t.Id == subjectGroupId && t.IsDeleted == false,
                        include: query => query.Include(q => q.SubjectInGroups));

                    var subjectGroupInDb = subjectInDB.FirstOrDefault();

                    var existingSubjectIdsInDB = subjectGroupInDb.SubjectInGroups
                        .Where(query => query.IsSpecialized == true)
                        .Select(s => s.SubjectId).ToList();

                    var newSpecialSubjects = subjectGroupUpdateModel.SpecializedSubjectIds.Except(existingSubjectIdsInDB).ToList();

                    var subjectsSpecialToRemove = existingSubjectIdsInDB
                        .Except(subjectGroupUpdateModel.SpecializedSubjectIds)
                        .ToList();

                    var updateSpecialSubject = await _unitOfWork.SubjectInGroupRepo.GetAsync(
                        filter: t => t.SubjectGroupId == subjectGroupId && newSpecialSubjects.Contains(t.SubjectId));

                    // update false các môn chuyên đề không có trong danh sách cập nhật
                    if (subjectsSpecialToRemove.Count != 0)
                    {
                        var subjectsToRemoveEntities = subjectGroupDb.SubjectInGroups
                            .Where(s => subjectsSpecialToRemove.Contains(s.SubjectId))
                            .ToList();
                        foreach (var subjectInGroup in subjectsToRemoveEntities)
                        {
                            subjectInGroup.IsSpecialized = false;
                            _unitOfWork.SubjectInGroupRepo.Update(subjectInGroup);
                        }
                    }

                    //update new specialized
                    foreach (var subjectInGroup in updateSpecialSubject)
                    {
                        subjectInGroup.IsSpecialized = true;
                        _unitOfWork.SubjectInGroupRepo.Update(subjectInGroup);
                    }

                    _unitOfWork.SubjectGroupRepo.Update(subjectGroupDb);
                    await _unitOfWork.SaveChangesAsync();

                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.UPDATE_SUBJECT_GROUP_SUCCESS,
                    };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        #endregion

        #region DeleteSubjectGroup
        public async Task<BaseResponseModel> DeleteSubjectGroup(int subjectGroupId)
        {
            var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetByIdAsync(subjectGroupId, include: query => query.Include(sg => sg.SubjectInGroups)) ?? throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
            subjectGroup.IsDeleted = true;

            _unitOfWork.SubjectGroupRepo.Update(subjectGroup);
            _unitOfWork.SubjectInGroupRepo.RemoveRange(subjectGroup.SubjectInGroups);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_SUBJECT_GROUP_SUCCESS };
        }
        #endregion
    }
}
