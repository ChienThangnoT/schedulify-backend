using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
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

        public async Task<BaseResponseModel> UpdateSubjectInGroup(List<SubjectInGroupUpdateModel> subjectInGroupUpdateModel)
        {
            var subjectInGroupIds = subjectInGroupUpdateModel.Select(x => x.SubjectInGroupId).ToList();

            var subjectInGroups = await _unitOfWork.SubjectInGroupRepo.GetAsync(
                filter: t => subjectInGroupIds.Contains(t.Id) && t.IsDeleted == false);

            if (subjectInGroups == null || !subjectInGroups.Any())
                throw new NotExistsException(ConstantResponse.SUBJECT_IN_GROUP_NOT_FOUND);

            foreach (var subject in subjectInGroupUpdateModel)
            {
                var subjectInGroup = subjectInGroups.FirstOrDefault(s => s.Id == subject.SubjectInGroupId);
                if (subjectInGroup != null)
                {
                    _mapper.Map(subject, subjectInGroup);
                    _unitOfWork.SubjectInGroupRepo.Update(subjectInGroup);
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
