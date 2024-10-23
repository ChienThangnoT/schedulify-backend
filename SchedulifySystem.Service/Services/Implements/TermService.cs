using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TermBusinessModels;
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
    public class TermService : ITermService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TermService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> GetTermBySchoolId(int schoolId)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId,
                filter: t => t.IsDeleted == false) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var term = await _unitOfWork.TermRepo.GetAsync(
                filter: t => t.SchoolId == schoolId && t.IsDeleted == false,
                includeProperties: "SchoolYear");
            if (term == null || !term.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.TERM_NOT_EXIST                };
            }
            var result = _mapper.Map<List<TermViewModel>>(term);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TERM_SUCCESS,
                Result = result
            };
        }
    }
}
