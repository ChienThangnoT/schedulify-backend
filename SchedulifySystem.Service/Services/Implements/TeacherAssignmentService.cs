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

        public async Task<BaseResponseModel> AssignTeacherForAsignments(List<AssignTeacherAssignmentModel> models)
        {
            foreach (var model in models)
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(model.TeacherId) ?? throw new NotExistsException($"Giáo viên id {model.TeacherId} không tồn tại!");
                var assignment = await _unitOfWork.TeacherAssignmentRepo.GetByIdAsync(model.Id, include: query => query.Include(ta => ta.Subject)) ?? throw new NotExistsException($"Phân công id {model.Id} không tồn tại!");
                var teachableSubject = (await _unitOfWork.TeachableSubjectRepo.GetAsync(filter: ts => ts.TeacherId == model.TeacherId && ts.SubjectId == assignment.SubjectId)).FirstOrDefault();
                if (teachableSubject == null)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Giáo viên {teacher.FirstName} {teacher.LastName} không thể dạy được môn {assignment.Subject.SubjectName}."
                    };
                }
                assignment.TeacherId = model.TeacherId;
                _unitOfWork.TeacherAssignmentRepo.Update(assignment);
            }
           
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


            var assignments = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: t => t.StudentClassId == classId && t.IsDeleted == false && (termId == null || t.TermId == termId)
                ,include: query => query.Include(ta => ta.Subject).Include(ta => ta.Teacher));
           
            var teacherNotAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(assignments.Where(a => a.TeacherId == null));
            var teacherAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(assignments.Where(a => a.TeacherId != null));

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
