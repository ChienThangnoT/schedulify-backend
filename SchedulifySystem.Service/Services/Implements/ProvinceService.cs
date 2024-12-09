using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.DistrictBusinessModels;
using SchedulifySystem.Service.BusinessModels.ProvinceBusinessModels;
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
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> AddProvinces(List<ProvinceAddModel> models)
        {
            var founded = await _unitOfWork.ProvinceRepo.GetV2Async(filter: f => models.Select(s => s.Name.Trim().ToLower()).Contains(f.Name.Trim().ToLower()) && !f.IsDeleted);
            if(founded.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Tỉnh thành {string.Join(", ", founded.Select(s => s.Name))} đã tồn tại trong hệ thống."
                };
            }
            var provinces = _mapper.Map<List<Province>>(models);
            await _unitOfWork.ProvinceRepo.AddRangeAsync(provinces);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.ADD_PROVINCE_SUCCESS
            };
        }

        public async Task<BaseResponseModel> GetProvinces(int? provinceId, int pageIndex = 1, int pageSize = 20)
        {
            var provinces = await _unitOfWork.ProvinceRepo.GetPaginationAsync(
                filter: t => (provinceId == null || t.Id == provinceId) && !t.IsDeleted,
                orderBy: t => t.OrderBy(a => a.Id),
                pageIndex: pageIndex,
                pageSize: pageSize);

            if (provinces.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.PROVINCE_NOT_EXIST);
            }

            var result = _mapper.Map<Pagination<ProvinceViewModel>>(provinces);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_PROVINCE_SUCCESS,
                Result = result
            };
        }

        public async Task<BaseResponseModel> RemoveProvince(int provinceId)
        {
            var province = await _unitOfWork.ProvinceRepo.GetByIdAsync(provinceId, filter: f => !f.IsDeleted) ?? 
                throw new NotExistsException($"Không tìm thấy tỉnh thành Id {provinceId} trong hệ thống.");

            province.IsDeleted = true;
            _unitOfWork.ProvinceRepo.Update(province);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.REMOVE_PROVINCE_SUCCESS,
            };
        }

        public async Task<BaseResponseModel> UpdateProvince(int id, ProvinceUpdateModel model)
        {
            var province = await _unitOfWork.ProvinceRepo.GetByIdAsync(id, filter: f => !f.IsDeleted) ??
                throw new NotExistsException($"Không tìm thấy tỉnh thành Id {id} trong hệ thống.");

            province.UpdateDate = DateTime.UtcNow;
            var oldName = province.Name.Trim().ToLower();
            var newName = model.Name.Trim().ToLower();

            if (!oldName.Equals(newName))
            {
                var found = await _unitOfWork.ProvinceRepo.GetV2Async(filter: f => !f.IsDeleted && f.Id != province.Id && f.Name.Trim().ToLower() == newName);
                if(found.Any()) 
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Tỉnh thành {model.Name} đã tồn tại trong hệ thống."
                    };
                }
                province.Name = model.Name.Trim();
            }

            _unitOfWork.ProvinceRepo.Update(province);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_PROVINCE_SUCCESS 
            };
        }
    }
}
