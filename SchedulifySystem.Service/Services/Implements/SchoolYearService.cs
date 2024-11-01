using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels;
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
    public class SchoolYearService : ISchoolYearService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SchoolYearService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> GetSchoolYear()
        {
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetAsync(filter: t => t.IsDeleted == false);

            var result = _mapper.Map<List<SchoolYearViewModel>>(schoolYear);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SCHOOL_YEAR_SUCCESS,
                Result = result
            };
        }
    }
}
