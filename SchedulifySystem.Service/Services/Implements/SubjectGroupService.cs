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

        public SubjectGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region create subject group
        public async Task<BaseResponseModel> CreateSubjectGroup(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");

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

            subjectGroupAddModel.GroupCode = subjectGroupAddModel.GroupCode.ToUpper();
            var subjectGroupAdd = _mapper.Map<SubjectGroup>(subjectGroupAddModel);
            subjectGroupAdd.SchoolId = schoolId;
            await _unitOfWork.SubjectGroupRepo.AddAsync(subjectGroupAdd);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<SubjectGroupViewModel>(subjectGroupAdd);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.ADD_SUBJECT_GROUP_SUCCESS,
                Result = result
            };
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

            if (subjectGroupDb.SubjectInGroups == null || subjectGroupDb.SubjectInGroups.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status200OK,
                    Message = ConstantResponse.GET_SUBJECT_GROUP_DETAIL_SUCCESS,
                    Result = result
                };
            }

            var listSBInGroup = subjectGroupDb.SubjectInGroups.ToList();
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
        public async Task<BaseResponseModel> GetSubjectGroups(int schoolId, int? subjectGroupId, Grade? grade, bool includeDeleted, int pageIndex, int pageSize)
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
