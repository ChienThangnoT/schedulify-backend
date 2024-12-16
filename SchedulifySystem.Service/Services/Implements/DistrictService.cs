using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.DistrictBusinessModels;
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
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DistrictService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> AddDistricts(int provinceId, List<DistrictAddModel> models)
        {
            var province = await _unitOfWork.ProvinceRepo.GetByIdAsync(provinceId, filter: f => !f.IsDeleted, 
                include: query => query.Include(p => p.Districts.Where(d => !d.IsDeleted).OrderByDescending(d => d.DistrictCode))) ??
                throw new NotExistsException(ConstantResponse.PROVINCE_NOT_EXIST);

            var founded = province.Districts.Where(d => models.Select(s => s.Name.Trim().ToLower()).Contains(d.Name.Trim().ToLower()));
            if (founded.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Quận/huyện {string.Join(", ", founded.Select(s => s.Name))} đã tồn tại trong hệ thống."
                };
            }
            var lastCode = province.Districts.FirstOrDefault()?.DistrictCode ?? 0;
            var districts = _mapper.Map<List<District>>(models);
            foreach (var district in districts)
            {
                district.ProvinceId = provinceId;
                district.DistrictCode = ++lastCode;
            }
            await _unitOfWork.DistrictRepo.AddRangeAsync(districts);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.ADD_DISTRICT_SUCCESS
            };
        }

        public async Task<BaseResponseModel> DeleteDistrict(int provinceId, int districtCode)
        {
            var province = await _unitOfWork.ProvinceRepo.GetByIdAsync(provinceId, filter: f => !f.IsDeleted) ??
                throw new NotExistsException(ConstantResponse.PROVINCE_NOT_EXIST);

            var district = (await _unitOfWork.DistrictRepo.GetV2Async(filter: f => f.Province.Id == provinceId && f.DistrictCode == districtCode && !f.IsDeleted)).FirstOrDefault() ??
                throw new NotExistsException(ConstantResponse.DISTRICT_NOT_EXIST);

            district.IsDeleted = true;
            _unitOfWork.DistrictRepo.Update(district);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.REMOVE_DISTRICT_SUCCESS
            };
        }

        public async Task<BaseResponseModel> GetDistrictByProvinceId(int? provinceId, int pageIndex, int pageSize)
        {
            var districts = await _unitOfWork.DistrictRepo.GetPaginationAsync(
                filter: t => (provinceId == null || t.ProvinceId == provinceId) && !t.IsDeleted,
                orderBy: t => t.OrderBy(a => a.Id),
                pageIndex: pageIndex,
                pageSize: pageSize);

            if (districts.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.DISTRICT_NOT_EXIST);
            }

            var result = _mapper.Map<Pagination<DistrictViewModel>>(districts);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_DISTRICT_SUCCESS,
                Result = result
            };
        }

        public async Task<BaseResponseModel> UpdateDistrict(int provinceId, int districtCode, DistrictUpdateModel model)
        {
            var province = await _unitOfWork.ProvinceRepo.GetByIdAsync(provinceId, filter: f => !f.IsDeleted,
                include: query => query.Include(p => p.Districts.Where(d => !d.IsDeleted).OrderByDescending(d => d.DistrictCode))) ??
                throw new NotExistsException(ConstantResponse.PROVINCE_NOT_EXIST);

            var district = province.Districts.FirstOrDefault(d => d.DistrictCode == districtCode) ??
               throw new NotExistsException(ConstantResponse.DISTRICT_NOT_EXIST);

            var oldName = district.Name.Trim().ToLower();
            var newName = model.Name.Trim().ToLower();

            if(oldName != newName)
            {
                var founded = province.Districts.Where(d => newName == d.Name.Trim().ToLower());
                if (founded.Any())
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Quận/huyện {model.Name} đã tồn tại trong hệ thống."
                    };
                }
                district.Name = model.Name.Trim();
            }
            _unitOfWork.DistrictRepo.Update(district);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_DISTRICT_SUCCESS
            };
        }
    }
}
