using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
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

        #region get subjects
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
                && (grade == null  || t.Grade == (int)grade)
                &&  t.IsDeleted == includeDeleted,
                pageIndex: pageIndex,
                pageSize: pageSize
                );
            if(subjects.Items.Count == 0)
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
    }
}
