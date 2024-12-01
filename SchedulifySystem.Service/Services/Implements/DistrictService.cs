using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    }
}
