using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
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
    }
}
