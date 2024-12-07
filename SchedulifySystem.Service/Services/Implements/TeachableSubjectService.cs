using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeachableSubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
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

        public async Task<BaseResponseModel> GetBySubjectId(int schoolId, int id, EGrade eGrade)
        {
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(id)
                ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

            var teachableSubjects = await _unitOfWork.TeachableSubjectRepo.GetV2Async(
                filter: ts => ts.SubjectId == id && ts.Teacher.SchoolId == schoolId && ts.Grade == (int)eGrade,
                include: query => query.Include(ts => ts.Teacher).Include(ts => ts.Subject));

            if (!teachableSubjects.Any())
            {
                throw new NotExistsException(ConstantResponse.GET_TEACHABLE_BY_SUBJECT_FAILED);
            }

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TEACHABLE_SUBJECT_SUCCESS,
                Result = _mapper.Map<List<TeachableSubjectDetailsViewModel>>(teachableSubjects)
            };
        }

        public async Task<BaseResponseModel> GetByTeacherId(int schoolId, int id)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id,
                    include: query => query.Include(t => t.Department).Include(t => t.TeachableSubjects).ThenInclude(ts => ts.Subject));

                var teacherViewModels = new TeacherViewModel
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Abbreviation = teacher.Abbreviation,
                    Email = teacher.Email,
                    DepartmentId = teacher.DepartmentId,
                    DepartmentName = teacher.Department?.Name,
                    Gender = (Gender)teacher.Gender,
                    Status = teacher.Status,
                    IsDeleted = teacher.IsDeleted,
                    Phone = teacher.Phone,
                    PeriodCount = teacher.PeriodCount,
                    TeachableSubjects = teacher.TeachableSubjects
                    .GroupBy(ts => ts.SubjectId)
                    .Select(group => new TeachableSubjectViewModel
                    {
                        SubjectId = group.Key,
                        SubjectName = group.First().Subject?.SubjectName,
                        Abbreviation = group.First().Subject?.Abbreviation,
                        Id = group.First().Id,
                        IsMain = group.First().IsMain,
                        ListApproriateLevelByGrades = group.Select(ts => new ListApproriateLevelByGrade
                        {
                            AppropriateLevel = (EAppropriateLevel)ts.AppropriateLevel,
                            Grade = (EGrade)ts.Grade
                        }).ToList()
                    }).ToList()
                };
                return teacher != null ? new BaseResponseModel()
                {
                    Status = StatusCodes.Status200OK,
                    Message = ConstantResponse.GET_TEACHABLE_SUBJECT_SUCCESS,
                    Result = teacherViewModels
                }
                :
                new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
    }
}
