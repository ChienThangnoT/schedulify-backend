using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
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
    public class CurriculumDetailService : ICurriculumDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CurriculumDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> UpdateCurriculumDetail(int schoolId, int yearId, int curriculumId, int termId, List<CurriculumDetailUpdateModel> curriculumDetailUpdateModel)
        {


            var curriculumDetailIds = curriculumDetailUpdateModel.Select(x => x.CurriculumDetailId).ToList();

            var curriculumDetails = await _unitOfWork.CurriculumDetailRepo.GetAsync(
                filter: t => curriculumDetailIds.Contains(t.Id) && t.IsDeleted == false
                && t.TermId == termId && schoolId == t.Curriculum.SchoolId && t.CurriculumId == curriculumId);

            if (curriculumDetails == null || !curriculumDetails.Any())
                throw new NotExistsException(ConstantResponse.SUBJECT_IN_GROUP_NOT_FOUND);

            foreach (var subject in curriculumDetailUpdateModel)
            {
                var curriculumDetail = curriculumDetails.FirstOrDefault(s => s.Id == subject.CurriculumDetailId);
                if (curriculumDetail != null)
                {
                    _mapper.Map(subject, curriculumDetail);
                    _unitOfWork.CurriculumDetailRepo.Update(curriculumDetail);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_SUBJECT_IN_GROUP_SUCCESS
            };
        }
    }
}
