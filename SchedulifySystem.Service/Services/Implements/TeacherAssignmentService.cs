using AutoMapper;
using Google.OrTools.LinearSolver;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
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
using static Google.OrTools.LinearSolver.Solver;

namespace SchedulifySystem.Service.Services.Implements
{
    public class TeacherAssignmentService : ITeacherAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int MaxPeriodsPerTeacher = 17;

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

        public async Task<BaseResponseModel> AutoAssignTeachers(int schoolId, int yearId)
        {
            // ** Step:1 load and checking data **
            // load all class in db
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId, 
                include: query => query.Include(cls => cls.TeacherAssignments));

            // checking all class has choiced a subject group
            var missingAssignment = classes.Where(cls => cls.TeacherAssignments.IsNullOrEmpty())
                .Select(cls => cls.Name).ToList();
            
            if(missingAssignment.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Lớp {string.Join(", ", missingAssignment)} chưa áp dụng tổ hợp nào!",
                };
            }
            var classIds = classes.Select(selector => selector.Id).ToList();
            // load assignment
            var asssignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && a.TermId == 1,
                include: query => query.Include(a => a.Subject).Include(a => a.StudentClass), 
                orderBy: query => query.OrderBy(a => a.StudentClassId).ThenBy(a => a.SubjectId) );

            //load teacher
            var teachers = await _unitOfWork.TeacherRepo
                .GetV2Async(filter: t => !t.IsDeleted && t.Status == (int)TeacherStatus.HoatDong && t.SchoolId == schoolId,
                include: query => query.Include(t => t.TeachableSubjects),
                orderBy: query => query.OrderBy(t => t.FirstName));

            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();
            var subjectsInAssignment = asssignmentsDb.Select(t => t.SubjectId).Distinct().ToList();

            subjectsInAssignment.ForEach(sia =>
            {
                if (!teachableSubjects.Contains(sia))
                {
                    throw new Exception($"chưa có giáo viên nào có thể dạy môn {sia}");
                }
            });

            List<TeacherAssignment> assignments = new List<TeacherAssignment>();
            var solver = Solver.CreateSolver("SCIP");
            if (solver == null) throw new Exception("Unable to create solver.");

            // Tạo biến quyết định cho mỗi giáo viên, lớp học và môn học
            var assignmentVars = new Dictionary<(int teacherId, int classId, int subjectId), Variable>();

            // Tạo biến quyết định cho mỗi giáo viên, lớp học và môn học
            foreach (var teacher in teachers)
            {
                foreach (var assignment in asssignmentsDb)
                {
                     // Kiểm tra xem giáo viên có thể dạy môn học cho lớp với Grade tương ứng không
                        if (teacher.TeachableSubjects.Any(ts =>
                            ts.SubjectId == assignment.SubjectId && ts.Grade == assignment.StudentClass.Grade))
                        {
                            var key = (teacher.Id, assignment.StudentClassId, assignment.SubjectId);
                            assignmentVars[key] = solver.MakeBoolVar($"assign_{key}");
                        }
                
                }
            }

            // Ràng buộc: Một môn học trong 1 lớp  chỉ có thể được phân công cho một giáo viên
            foreach (var classItem in classes)
            {
                foreach (var assignment in asssignmentsDb.Where(a => a.StudentClassId == classItem.Id))
                {
                    // Lấy tất cả các biến quyết định liên quan đến môn học này trong lớp này
                    List<Variable> assignmentVarsForSubject = assignmentVars
                        .Where(x => x.Key.subjectId == assignment.SubjectId && x.Key.classId == classItem.Id)
                        .Select(x => x.Value)
                        .ToList();

                    if (assignmentVarsForSubject.Count > 0)
                    {
                        // Thiết lập ràng buộc: chỉ có duy nhất một giáo viên được phân công cho môn học này
                        Variable sumVar = solver.MakeIntVar(0, 1, $"sum_{assignment.SubjectId}_{classItem.Id}");

                        // Tạo tổng thủ công bằng cách sử dụng một vòng lặp
                        foreach (var var in assignmentVarsForSubject)
                        {
                            solver.Add(sumVar == sumVar + var);
                        }

                        // Đảm bảo rằng tổng chỉ bằng 1
                        solver.Add(sumVar == 1);
                    }
                }
            }

            // Ràng buộc: Số tiết dạy của mỗi giáo viên không vượt quá 17 tiết (nếu có thể)
            foreach (var teacher in teachers)
            {
                // Khởi tạo một LinearExpr với giá trị ban đầu là 0
                LinearExpr totalTeacherPeriods = solver.MakeIntVar(0, MaxPeriodsPerTeacher * 10, $"totalPeriods_{teacher.Id}");

                // Tính tổng số tiết mà giáo viên được phân công
                foreach (var kvp in assignmentVars.Where(x => x.Key.teacherId == teacher.Id))
                {
                    // Lấy số tiết của môn học tương ứng
                    var subjectPeriods = asssignmentsDb
                        .FirstOrDefault(s => s.SubjectId == kvp.Key.subjectId)?.PeriodCount ?? 0;

                    // Cộng dồn số tiết vào LinearExpr
                    totalTeacherPeriods += kvp.Value * subjectPeriods;
                }

                // Ràng buộc tổng số tiết của giáo viên không vượt quá MaxPeriodsPerTeacher
                solver.Add(totalTeacherPeriods <= MaxPeriodsPerTeacher);
            }

            // Hàm mục tiêu: Tối đa hóa mức độ phù hợp
            var objective = solver.Objective();

            foreach (var kvp in assignmentVars)
            {
                var (teacherId, classId, subjectId) = kvp.Key;
                var teacher = teachers.FirstOrDefault(t => t.Id == teacherId);
                if (teacher == null) continue;

                var teachableSubject = teacher.TeachableSubjects
                    .FirstOrDefault(ts => ts.SubjectId == subjectId && ts.Grade == classes.FirstOrDefault(c => c.Id == classId)?.Grade);

                if (teachableSubject != null)
                {
                    objective.SetCoefficient(kvp.Value, teachableSubject.AppropriateLevel);
                }
            }
            objective.SetMaximization();

            var resultStatus = solver.Solve();

            // Kiểm tra trạng thái lời giải
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                throw new Exception("Không thể tìm thấy giải pháp tối ưu.");
            }

            // Trả về danh sách phân công
            foreach (var kvp in assignmentVars)
            {
                if (kvp.Value.SolutionValue() == 1)
                {
                    assignments.Add(new TeacherAssignment
                    {
                        TeacherId = kvp.Key.teacherId,
                        StudentClassId = kvp.Key.classId,
                        SubjectId = kvp.Key.subjectId,
                        TermId = asssignmentsDb
                            .First(s => s.SubjectId == kvp.Key.subjectId).PeriodCount
                    });
                }
            }

            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = assignments};
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
