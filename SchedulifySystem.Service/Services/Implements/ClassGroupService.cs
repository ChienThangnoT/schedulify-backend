using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Service.BusinessModels.ClassGroupBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class ClassGroupService : IClassGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int ROOT = 0;

        public ClassGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region GetGrades
        public async Task<BaseResponseModel> GetGrades()
        {
            var grades = await _unitOfWork.ClassGroupRepo.ToPaginationIncludeAsync(filter: cg => cg.ParentId == ROOT);
            var response = _mapper.Map<Pagination<GradeViewModel>>(grades);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get grades success!", Result = response };
        }
        #endregion
    }
}
