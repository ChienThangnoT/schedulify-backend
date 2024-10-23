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

        public async Task<BaseResponseModel> AddTermBySchoolId(int schoolId, TermAdjustModel termAddModel)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)termAddModel.SchoolYearId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var termAdd = _mapper.Map<Term>(termAddModel);
            termAdd.SchoolId = schoolId;
            termAdd.StartDate = termAdd.StartDate.ToUniversalTime();
            termAdd.EndDate = termAdd.EndDate.ToUniversalTime();
            await _unitOfWork.TermRepo.AddAsync(termAdd);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.CREATE_TERM_SUCCESS
            };
        }

        public async Task<BaseResponseModel> UpdateTermBySchoolId(int termId, TermAdjustModel termAddModel)
        {
            if (termAddModel.SchoolYearId != null)
            {
                var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)termAddModel.SchoolYearId)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            }

            var term = await _unitOfWork.TermRepo.GetByIdAsync(termId)
                ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);

            if (!string.IsNullOrEmpty(termAddModel.Name))
            {
                term.Name = termAddModel.Name;
            }
            if (termAddModel.StartDate != null)
            {
                term.StartDate = termAddModel.StartDate.Value.ToUniversalTime();
            }
            if (termAddModel.EndDate != null)
            {
                term.EndDate = termAddModel.EndDate.Value.ToUniversalTime();
            }
            if (termAddModel.SchoolYearId != null)
            {
                term.SchoolYearId = termAddModel.SchoolYearId.Value;
            }

            _unitOfWork.TermRepo.Update(term);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_TERM_SUCCESS
            };
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
                    Message = ConstantResponse.TERM_NOT_EXIST
                };
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
