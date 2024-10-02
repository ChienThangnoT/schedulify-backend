using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Exceptions;
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
    public class StudentClassService : IStudentClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region GetStudentClassById
        public async Task<BaseResponseModel> GetStudentClassById(int id)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id) ?? throw new NotExistsException($"Student class id {id} is not found!");
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get class success!", Result = existedClass };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException("School is not found!");
            var studentClasses = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: tc => tc.School.Id == schoolId && (includeDeleted ? true : tc.IsDeleted == false) && (schoolYearId == null ? true : tc.SchoolYearId == schoolYearId),
                include: query => query.Include(tc => tc.Teacher));
            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get student classes success!", Result = studentClassesViewModel };

        }
        #endregion
    }
}
