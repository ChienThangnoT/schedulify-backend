using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeachableSubjectBusinessModels;
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
    public class TeachableSubjectService : ITeachableSubjectService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TeachableSubjectService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseModel> GetBySubjectId(int schoolId, int id)
        {
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(id) 
                ?? throw  new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

            var TeachableSubjects = await _unitOfWork.TeachableSubjectRepo.GetV2Async(
                filter: ts => ts.SubjectId == id && ts.Subject.SchoolId == schoolId,
                include: query => query.Include(ts => ts.Teacher).Include(ts => ts.Subject)) ;

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TEACHABLE_SUBJECT_SUCCESS,
                Result = _mapper.Map<List<TeachableSubjectDetailsViewModel>>(TeachableSubjects)
            };
        }

        public async Task<BaseResponseModel> GetByTeacherId(int schoolId, int id)
        {
            var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id) ??
                throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);

            var TeachableSubjects = await _unitOfWork.TeachableSubjectRepo.GetV2Async(
                filter: ts => ts.TeacherId == id && ts.Teacher.SchoolId == schoolId,
                include: query => query.Include(ts => ts.Teacher).Include(ts => ts.Subject));

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TEACHABLE_SUBJECT_SUCCESS,
                Result = _mapper.Map<List<TeachableSubjectDetailsViewModel>>(TeachableSubjects)
            };
        }
    }
}
