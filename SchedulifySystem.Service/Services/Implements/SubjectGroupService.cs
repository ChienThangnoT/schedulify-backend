using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels;
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

        public async Task<BaseResponseModel> CreateSubjectGroupList(int schoolId, SubjectGroupAddModel subjectGroupAddModel)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");

            var existSubjectGroupType = await _unitOfWork.SubjectGroupTypeRepo.GetByIdAsync(subjectGroupAddModel.SubjectGroupTypeId)
                ?? throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_TYPE_NOT_EXISTED);
            var checkExistSubjectGroup = await _unitOfWork.SubjectGroupRepo.GetAsync(
                filter: t => t.GroupName.Equals(subjectGroupAddModel.GroupName, StringComparison.OrdinalIgnoreCase) ||
                        t.GroupCode.Equals(subjectGroupAddModel.GroupCode, StringComparison.OrdinalIgnoreCase));
            if (checkExistSubjectGroup.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.SUBJECT_GROUP_NAME_OR_CODE_EXISTED
                };
            }


            var subjectGroupAdd = _mapper.Map<SubjectGroup>(subjectGroupAddModel);
            await _unitOfWork.SubjectGroupRepo.AddAsync(subjectGroupAdd);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.ADD_SUBJECT_GROUP_SUCCESS,
                Result = subjectGroupAdd
            };
        }
    }
}
