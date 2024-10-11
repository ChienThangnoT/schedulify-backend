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

        public async Task<BaseResponseModel> GetSchools(int pageIndex, int pageSize, int districtId, int provinceId, SchoolStatus schoolStatus)
        {
            Expression<Func<School, bool>>? filter = schoolStatus switch
            {
                // if districtId = 0, filter by schoolStatus
                0 when districtId == 0 => null,

                // if districtId != 0 và dont have schoolStatus, only filter DistrictId
                //0 => u => u.DistrictId == districtId,

                // if districtId != 0 and exist schoolStatus, filter districtId & status
                _ when districtId == 0 => u => u.Status == (int)schoolStatus,

                // filter with districtId & status
               // _ => u => u.DistrictId == districtId && u.Status == (int)schoolStatus
            };

            var schools = await _unitOfWork.SchoolRepo.ToPaginationIncludeAsync(
                pageIndex, pageSize,
                //query => query.Include(s => s.District),
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
