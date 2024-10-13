using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.SchoolBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class SchoolService : ISchoolService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SchoolService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> GetSchools(int pageIndex, int pageSize, int? districtCode, int? provinceId, SchoolStatus? schoolStatus)
        {
            Expression<Func<School, bool>>? filter = s =>
                (provinceId == null || provinceId == 0 || s.ProvinceId == provinceId) && //skip if  ProvinceId is null or 0
                (districtCode == null || districtCode == 0 || s.DistrictCode == districtCode) &&  // skip if DistrictCode is null or 0
                (schoolStatus == null || s.Status == (int)schoolStatus || schoolStatus == 0); // skip if schoolStatus null

            var schools = await _unitOfWork.SchoolRepo.ToPaginationIncludeAsync(
                pageIndex, pageSize,
                query => query.Include(s => s.Province),
                filter: filter
            );

            if (schools.Items.Count == 0)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "No schools found for the given criteria."
                };
            }

            var result = _mapper.Map<Pagination<SchoolViewModel>>(schools);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Result = result
            };
        }

    }
}
