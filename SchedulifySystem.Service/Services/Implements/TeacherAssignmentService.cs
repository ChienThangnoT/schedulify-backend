using AutoMapper;
using Google.OrTools.Sat;
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
        private const int MaxPeriodsPerTeacher = 50;

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
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId,
                include: query => query.Include(cls => cls.TeacherAssignments));

            var missingAssignment = classes.Where(cls => cls.TeacherAssignments.IsNullOrEmpty())
                .Select(cls => cls.Name).ToList();

            if (missingAssignment.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Lớp {string.Join(", ", missingAssignment)} chưa áp dụng tổ hợp nào!",
                };
            }

            var classIds = classes.Select(selector => selector.Id).ToList();
            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && a.TermId == 1,
                include: query => query.Include(a => a.Subject).Include(a => a.StudentClass));

            var teachers = await _unitOfWork.TeacherRepo.GetV2Async(
                filter: t => !t.IsDeleted && t.Status == (int)TeacherStatus.HoatDong && t.SchoolId == schoolId,
                include: query => query.Include(t => t.TeachableSubjects));

            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();
            var subjectsInAssignment = assignmentsDb.Select(t => t.SubjectId).Distinct().ToList();

            foreach (var sia in subjectsInAssignment)
            {
                if (!teachableSubjects.Contains(sia))
                {
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Chưa có giáo viên nào có thể dạy môn {sia}"
                    };
                }
            }

            var teacherCapabilities = teachers.ToDictionary(
                teacher => teacher.Id,
                teacher => teacher.TeachableSubjects.ToList()
            );

            await AssignTeachers(assignmentsDb.ToList(), teachers.ToList(), teacherCapabilities);

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Result = _mapper.Map<List<TeacherAssignmentViewModel>>(assignmentsDb)
            };
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

        public async Task AssignTeachers(
    List<TeacherAssignment> assignments,
    List<Teacher> teachers,
    Dictionary<int, List<TeachableSubject>> teacherCapabilities)
        {
            CpModel model = new CpModel();

            int numAssignments = assignments.Count;
            int numTeachers = teachers.Count;

            // Biến nhị phân đại diện cho việc phân công giáo viên cho từng assignment
            BoolVar[,] assignmentMatrix = new BoolVar[numAssignments, numTeachers];

            for (int i = 0; i < numAssignments; i++)
            {
                for (int j = 0; j < numTeachers; j++)
                {
                    assignmentMatrix[i, j] = model.NewBoolVar($"assign_{i}_{j}");
                }
            }

            // Ràng buộc: Mỗi assignment chỉ được phân cho một giáo viên
            for (int i = 0; i < numAssignments; i++)
            {
                List<ILiteral> possibleAssignments = new List<ILiteral>();
                for (int j = 0; j < numTeachers; j++)
                {
                    possibleAssignments.Add(assignmentMatrix[i, j]);
                }
                model.Add(LinearExpr.Sum(possibleAssignments) == 1);
            }

            // Ràng buộc: Giáo viên chỉ có thể dạy các môn mà họ có thể dạy
            for (int i = 0; i < numAssignments; i++)
            {
                var assignment = assignments[i];
                for (int j = 0; j < numTeachers; j++)
                {
                    var teacher = teachers[j];
                    bool canTeach = teacherCapabilities[teacher.Id]
                        .Any(ts => ts.SubjectId == assignment.SubjectId && ts.Grade == assignment.StudentClass?.Grade);

                    if (!canTeach)
                    {
                        model.Add(assignmentMatrix[i, j] == 0);
                    }
                }
            }

            // Ràng buộc: Số tiết dạy của mỗi giáo viên không vượt quá giới hạn
            foreach (var teacher in teachers)
            {
                int teacherIndex = teachers.IndexOf(teacher);
                List<IntVar> teacherLoad = new List<IntVar>();

                for (int i = 0; i < numAssignments; i++)
                {
                    IntVar assignedLoad = model.NewIntVar(0, assignments[i].PeriodCount, $"load_{i}_{teacherIndex}");
                    model.Add(assignedLoad == assignments[i].PeriodCount * assignmentMatrix[i, teacherIndex]);
                    teacherLoad.Add(assignedLoad);
                }

                // Ràng buộc tổng số tiết của giáo viên không vượt quá PeriodCount của họ
                model.Add(LinearExpr.Sum(teacherLoad) <= 50);
            }

            // Tạo solver và giải bài toán
            CpSolver solver = new CpSolver();
            CpSolverStatus status =  solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Console.WriteLine("Lời giải tối ưu đã được tìm thấy.");

                for (int i = 0; i < numAssignments; i++)
                {
                    for (int j = 0; j < numTeachers; j++)
                    {
                        if (solver.Value(assignmentMatrix[i, j]) == 1)
                        {
                            assignments[i].TeacherId = teachers[j].Id;
                            Console.WriteLine($"Giáo viên {teachers[j].FirstName} {teachers[j].LastName} được phân công dạy môn {assignments[i].Subject?.SubjectName} cho lớp {assignments[i].StudentClass?.Name}");

                            // Cập nhật vào cơ sở dữ liệu
                           // _unitOfWork.TeacherAssignmentRepo.Update(assignments[i]);
                        }
                    }
                }

               // await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Không tìm thấy lời giải khả thi.");
            }
        }



        public Task<BaseResponseModel> UpdateAssignment(int assignmentId)
        {
            throw new NotImplementedException();
        }
    }
}
