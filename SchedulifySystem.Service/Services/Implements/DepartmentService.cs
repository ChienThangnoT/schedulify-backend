using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<BaseResponseModel> AddDepartment(int schoolId, List<DepartmentAddModel> models)
        {
            var checkSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId , filter: t=> t.Status == (int)SchoolStatus.Active) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            //check duplicate in list
            var duplicateName = models.GroupBy(d => d.Name, StringComparer.OrdinalIgnoreCase).Where(d => d.Count() > 1).SelectMany(g => g).ToList();
            var duplicateCode = models.GroupBy(d => d.DepartmentCode, StringComparer.OrdinalIgnoreCase).Where(d => d.Count() > 1).SelectMany(g => g).ToList();

            if (duplicateName.Count != 0)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.DEPARTMENT_NAME_DUPLICATE, Result = duplicateName };
            }

            if (duplicateCode.Count != 0)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.DEPARTMENT_CODE_DUPLICATE, Result = duplicateCode };
            }
            //check duplicate in database 
            var names = models.Select(d => d.Name.ToLower()).ToList();
            var code = models.Select(d => d.DepartmentCode.ToLower()).ToList();
            var duplicateInDb = (await _unitOfWork.DepartmentRepo.GetV2Async(
                filter: d => names.Contains(d.Name.ToLower()) || code.Contains(d.DepartmentCode.ToLower()))).ToList();

            if (duplicateInDb.Count != 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.DEPARTMENT_NAME_OR_CODE_EXISTED,
                    Result = _mapper.Map<List<DepartmentAddModel>>(duplicateInDb)
                };
            }

            //add to db
            models.ForEach(d => d.SchoolId = schoolId);
            var data = _mapper.Map<List<Department>>(models);
            _unitOfWork.DepartmentRepo.AddRangeAsync(data);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_DEPARTMENT_SUCCESS };
        }

        public async Task<BaseResponseModel> DeleteDepartment(int departmentId)
        {
            var existed = await _unitOfWork.DepartmentRepo.GetByIdAsync(departmentId)
                ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);

            existed.IsDeleted = true;
            _unitOfWork.DepartmentRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_DEPARTMENT_SUCCESS };
        }

        public async Task<BaseResponseModel> GetDepartments(int schoolId, int pageIndex = 1, int pageSize = 20)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                 ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var departments = await _unitOfWork.DepartmentRepo
                .ToPaginationIncludeAsync(pageIndex, pageSize, filter: (f => f.SchoolId == schoolId && !f.IsDeleted));
            var result = _mapper.Map<Pagination<DepartmentViewModel>>(departments);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_DEPARTMENT_SUCCESS, Result = result };
        }

        public async Task<BaseResponseModel> UpdateDepartment(int departmentId, int schoolId, DepartmentUpdateModel model)
        {
            var existed = await _unitOfWork.DepartmentRepo.GetByIdAsync(departmentId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if(model.Name != null || model.DepartmentCode != null)
            {
                var check = (await _unitOfWork.DepartmentRepo.GetV2Async(
                                filter: d => d.SchoolId == schoolId && (model.Name == null || d.Name.ToLower().Equals(model.Name.ToLower()))
                                && (model.DepartmentCode == null || d.DepartmentCode.ToLower().Equals(model.DepartmentCode.ToLower())) )).ToList();
                if (check.Count != 0)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.DEPARTMENT_NAME_OR_CODE_EXISTED,
                    };
                }
            }

            if(model.Name != null)
            {
                existed.Name = model.Name;
            }
            if (model.DepartmentCode != null)
            {
                existed.DepartmentCode = model.DepartmentCode;
            } 

            if(model.Description != null)
            {
                existed.Description = model.Description;
            }

            _unitOfWork.DepartmentRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_DEPARTMENT_SUCCESS };
        }
    }
}
