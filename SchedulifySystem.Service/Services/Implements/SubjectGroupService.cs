using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
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
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");
                    var schoolYear = (await _unitOfWork.SchoolYearRepo.ToPaginationIncludeAsync(filter: sy => sy.Id == subjectGroupAddModel.SchoolYearId,
                        include: query => query.Include(sy => sy.Terms))).Items.FirstOrDefault()
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
                    var checkExistSubjectGroup = await _unitOfWork.SubjectGroupRepo.GetAsync(
                        filter: t => t.GroupName.ToLower() == subjectGroupAddModel.GroupName.ToLower() ||
                                     t.GroupCode.ToLower() == subjectGroupAddModel.GroupCode.ToLower()
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
                    if (subjectGroupAddModel.SpecializedSubjectIds.Count() != REQUIRE_SPECIALIZED_SUBJECT || subjectGroupAddModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.INVALID_NUMBER_SUBJECT };
                    }

                    subjectGroupAddModel.SchoolId = schoolId;
                    var subjectGroupAdd = _mapper.Map<SubjectGroup>(subjectGroupAddModel);
                    await _unitOfWork.SubjectGroupRepo.AddAsync(subjectGroupAdd);
                    //save to have group id
                    await _unitOfWork.SaveChangesAsync();
                    //generate data subject

                    //ElectiveSubjects
                    foreach (int subjectId in subjectGroupAddModel.ElectiveSubjectIds)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId) ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
                        if (checkSubject.IsRequired) return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.REQUIRE_ELECTIVE_SUBJECT, Result = subjectId };
                        var newSubjectInGroup = new SubjectInGroup();
                        newSubjectInGroup.SubjectId = subjectId;
                        newSubjectInGroup.SubjectGroupId = subjectGroupAdd.Id;
                        newSubjectInGroup.IsSpecialized = subjectGroupAddModel.SpecializedSubjectIds.Contains(subjectId);
                        foreach (Term term in schoolYear.Terms)
                        {
                            newSubjectInGroup.TermId = term.Id;
                            _unitOfWork.SubjectInGroupRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                        }
                    }

                    //Specialized Subject
                    var specializedSubjectNotInElectiveSubjects = subjectGroupAddModel.SpecializedSubjectIds.Where(s => !subjectGroupAddModel.ElectiveSubjectIds.Contains(s));

                    foreach (int subjectId in specializedSubjectNotInElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId) ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
                        if (!checkSubject.IsRequired) return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.INVALID_SPECIALIZED_SUBJECT, Result = subjectId };
                        var newSubjectInGroup = new SubjectInGroup();
                        newSubjectInGroup.SubjectId = subjectId;
                        newSubjectInGroup.SubjectGroupId = subjectGroupAdd.Id;
                        newSubjectInGroup.IsSpecialized = true;

                        foreach (Term term in schoolYear.Terms)
                        {
                            newSubjectInGroup.TermId = term.Id;
                            _unitOfWork.SubjectInGroupRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                    transaction.Commit();
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
        public async Task<BaseResponseModel> GetSubjectGroupDetail(int subjectGroupId, int? termId)
        {
            var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetV2Async(
                filter: t => t.Id == subjectGroupId && t.IsDeleted == false ,
                include: query => query.Include(c => c.SubjectInGroups)
                           .ThenInclude(sg => sg.Subject));

            if (subjectGroup == null || !subjectGroup.Any())
            {
                throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
            }

            var subjectGroupDb = subjectGroup.FirstOrDefault();

            var result = _mapper.Map<SubjectGroupViewDetailModel>(subjectGroupDb);

           
            var listSBInGroup = termId ==null ? subjectGroupDb.SubjectInGroups.ToList() : subjectGroupDb.SubjectInGroups.Where(sig => sig.TermId == termId);
            var subjectInGroupList = _mapper.Map<List<SubjectInGroupViewDetailModel>>(listSBInGroup);

            result.SubjectInGroups = subjectInGroupList;

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_GROUP_DETAIL_SUCCESS,
                Result = result
            };
        }

        #endregion

        #region get subject groups
        public async Task<BaseResponseModel> GetSubjectGroups(int schoolId, int? subjectGroupId, Grade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (subjectGroupId != null)
            {
                var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetByIdAsync((int)subjectGroupId)
                    ?? throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);
            }

            var subjects = await _unitOfWork.SubjectGroupRepo.GetPaginationAsync(
                filter: t => t.SchoolId == schoolId
                && (subjectGroupId == null || t.Id == subjectGroupId)
                && (grade == null || t.Grade == (int)grade)
                && (schoolYearId == null || t.SchoolYearId == schoolYearId)
                && t.IsDeleted == includeDeleted,
                pageIndex: pageIndex,
                pageSize: pageSize
                );
            if (subjects.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.GET_SUBJECT_GROUP_LIST_SUCCESS
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
        #region
        public async Task<BaseResponseModel> UpdateSubjectGroup(int subjectGroupId, SubjectGroupUpdateModel subjectGroupUpdateModel)
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

            if (subjectGroupUpdateModel.Grade != 0 && subjectGroupUpdateModel.Grade != (Grade)subjectGroupDb.Grade)
            {
                var subjectInGroups = await _unitOfWork.SubjectInGroupRepo.GetAsync(
                    filter: t => t.SubjectGroupId == subjectGroupId && !t.IsDeleted);

                if (subjectInGroups.Any())
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

            _unitOfWork.SubjectGroupRepo.Update(subjectGroupDb);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<SubjectGroupViewModel>(subjectGroup);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_SUBJECT_GROUP_SUCCESS,
                Result = result
            };
        }

        #endregion
    }
}
