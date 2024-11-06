using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
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
        #region add term
        public async Task<BaseResponseModel> AddTerm(TermAdjustModel termAddModel)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)termAddModel.SchoolYearId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var termAdd = _mapper.Map<Term>(termAddModel);
            await _unitOfWork.TermRepo.AddAsync(termAdd);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.CREATE_TERM_SUCCESS
            };
        }
        #endregion

        #region update term
        public async Task<BaseResponseModel> UpdateTermById(int termId, TermAdjustModel termAddModel)
        {
            if (termAddModel.SchoolYearId != null)
            {
                var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)termAddModel.SchoolYearId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            }

            var term = await _unitOfWork.TermRepo.GetByIdAsync(termId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);

            if (!string.IsNullOrEmpty(termAddModel.Name))
            {
                term.Name = termAddModel.Name;
            }
            if (termAddModel.StartWeek != 0)
            {
                term.StartWeek = termAddModel.StartWeek;
            }
            if (termAddModel.EndWeek != null)
            {
                term.EndWeek = termAddModel.EndWeek;
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
        #endregion

        #region get term
        public async Task<BaseResponseModel> GetTerms(int? termId, int schoolYearId, int pageIndex, int pageSize)
        {
            if (schoolYearId != null)
            {
                var school = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)schoolYearId,
                             filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            }

            var term = await _unitOfWork.TermRepo.GetPaginationAsync(
                filter: t => (termId == null || t.Id == termId) && (schoolYearId == null || t.SchoolYearId == schoolYearId) && t.IsDeleted == false,
                includeProperties: "SchoolYear",
                pageIndex: pageIndex,
                pageSize: pageSize);
            if (term.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.TERM_NOT_EXIST
                };
            }
            var result = _mapper.Map<Pagination<TermViewModel>>(term);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TERM_SUCCESS,
                Result = result
            };
        }
        #endregion
    }
}
