using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
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
    public class TeacherAssignmentService : ITeacherAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeacherAssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> AddAssignment(AddTeacherAssignmentModel model)
        {
            var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(model.TeacherId) ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);
            var subject = await _unitOfWork.SubjectRepo.GetByIdAsync(model.SubjectId) ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
            var teachableSubject = (await _unitOfWork.TeachableSubjectRepo.GetAsync(filter: ts => ts.TeacherId == model.TeacherId && ts.SubjectId == model.SubjectId)).FirstOrDefault();

            if (teachableSubject == null)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, 
                    Message = $"Giáo viên {teacher?.Abbreviation} không thể dạy được môn {subject.SubjectName}." };
            }

            var studentClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(model.StudentClassId)
                ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);

            var term = await _unitOfWork.TermRepo.GetByIdAsync(model.TermId) ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);

            var newAssign = _mapper.Map<TeacherAssignment>(model);
            await _unitOfWork.TeacherAssignmentRepo.AddAsync(newAssign);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_TEACHER_ASSIGNMENT_SUCCESS };
        }

        public async Task<BaseResponseModel> GetAssignment(int classId, int? termId)
        {
            var studentClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(classId)
                ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);
            if (termId.HasValue)
            {
                var term = await _unitOfWork.TermRepo.GetByIdAsync((int)termId)
                    ?? throw new NotExistsException(ConstantResponse.TERM_NOT_FOUND);
            }

            var teacherNotAssigntView = new List<TeacherAssignmentViewModel>();
            var teacherAssigntView = new List<TeacherAssignmentViewModel>();

            var teacherNotAssignt = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                filter: t => t.StudentClassId == classId && t.TeacherId == null && t.IsDeleted == false && (termId == null || t.TermId == termId)
                , includeProperties: "Subject");
            if (teacherNotAssignt == null || !teacherNotAssignt.Any())
            {
                teacherNotAssigntView = null;
            }
            teacherNotAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(teacherNotAssignt);


            var teacherAssignt = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                filter: t => t.StudentClassId == classId && t.IsDeleted == false && (termId == null || t.TermId == termId)
                ,includeProperties: "Teacher,Subject");
            if (teacherAssignt == null || !teacherAssignt.Any())
            {
                teacherAssigntView = null;
            }
            teacherAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(teacherAssignt);


            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TEACHER_ASSIGNTMENT_SUCCESS,
                Result = new
                {
                    TeacherAssigntView = teacherAssigntView,
                    TeacherNotAssigntView = teacherNotAssigntView
                }
            };

        }

        public Task<BaseResponseModel> UpdateAssignment(int assignmentId)
        {
            throw new NotImplementedException();
        }
    }
}
