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
            var teacheableSubject = await _unitOfWork.TeachableSubjectRepo.GetByIdAsync(model.TeachableSubjectId,
                include: query => query.Include(ts => ts.Teacher).Include(ts => ts.Subject))
                ?? throw new NotExistsException(ConstantResponse.TEACHABLE_SUBJECT_NOT_EXIST);

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
            //var studentClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(classId)
            //    ?? throw new Exception(ConstantResponse.CLASS_NOT_EXIST);



            //var assignedList = assignments.Items.Where(a => a.TeachableSubject.TeacherId)
            throw new NotImplementedException();

        }

        public Task<BaseResponseModel> UpdateAssignment(int assignmentId)
        {
            throw new NotImplementedException();
        }
    }
}
