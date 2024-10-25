using AutoMapper;
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
    public class SubjectInGroupService : ISubjectInGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectInGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> GetSubjectInGroup(int schoolId, int? termId, int schoolYearId, int? subjectGroupId,int? subbjectInGroupId, int pageIndex, int pageSize)
        {
            var schoool = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.IsDeleted == false) 
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var schooolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => t.IsDeleted == false) 
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            if(termId != null)
            {
                var _ = await _unitOfWork.TermRepo.GetByIdAsync((int)termId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
            }

            if(subbjectInGroupId != null)
            {
                var _ = await _unitOfWork.SubjectInGroupRepo.GetByIdAsync((int)subbjectInGroupId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SUBJECT_IN_GROUP_NOT_FOUND);
            }

            if(subjectGroupId != null)
            {
                var _ = await _unitOfWork.SubjectGroupRepo.GetByIdAsync((int)subjectGroupId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SUBJECT_IN_GROUP_NOT_FOUND);
            }


        }
    }
}
