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

        private async Task<Dictionary<string, List<string>>> CheckAssignmentErrors(IEnumerable<StudentClass> classes,SchoolYear schoolYear, IEnumerable<Teacher> teachers, IEnumerable<TeacherAssignment> assignmentsDb)
        {
            // Khởi tạo dictionary để lưu lỗi theo từng thực thể
            var errorDictionary = new Dictionary<string, List<string>>()
                {
                    { "Giáo viên", new List<string>() },
                    { "Lớp học", new List<string>() },
                    { "Năm học", new List<string>() },
                    { "Môn học", new List<string>() }
                };

            // Kiểm tra lớp học
            if (!classes.Any())
            {
                errorDictionary["Lớp học"].Add("Không có lớp học nào tồn tại.");
            }

            var missingAssignment = classes.Where(cls => cls.TeacherAssignments.IsNullOrEmpty())
                .Select(cls => cls.Name).ToList();
            if (missingAssignment.Any())
            {
                errorDictionary["Lớp học"].Add($"Lớp {string.Join(", ", missingAssignment)} chưa áp dụng tổ hợp nào!");
            }

            // check exist homeroom teacher
            var classesWithoutHomeroomTeacher = classes.Where(cls => cls.HomeroomTeacherId == null).Select(cls => cls.Name).ToList();
            if (classesWithoutHomeroomTeacher.Any())
            {
                errorDictionary["Lớp học"].Add($"Lớp {string.Join(", ", classesWithoutHomeroomTeacher)} chưa có giáo viên chủ nhiệm.");
            }

            // Kiểm tra năm học và kỳ học

            if (schoolYear == null)
            {
                errorDictionary["Năm học"].Add("Năm học không tồn tại hoặc đã bị xóa.");
            }
           

            // Kiểm tra giáo viên
           
            if (!teachers.Any())
            {
                errorDictionary["Giáo viên"].Add("Không có giáo viên nào để phân công.");
            }
            else
            {
                // Kiểm tra teachableSubjects của từng giáo viên không rỗng
                foreach (var teacher in teachers)
                {
                    if (teacher.TeachableSubjects == null || !teacher.TeachableSubjects.Any())
                    {
                        errorDictionary["Giáo viên"].Add($"Giáo viên {teacher.FirstName} {teacher.LastName} chưa đươc phân công môn để dạy.");
                    }
                }
            }

            // Kiểm tra nhiệm vụ giảng dạy
            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();
            if (!assignmentsDb.Any())
            {
                errorDictionary["Lớp học"].Add("Không có nhiệm vụ giảng dạy nào tồn tại cho các lớp học được chọn.");
            }
            else
            {
                // Kiểm tra giáo viên có thể dạy các môn yêu cầu
                var subjectsInAssignment = assignmentsDb.Select(t => t.SubjectId).Distinct().ToList();
                foreach (var sia in subjectsInAssignment)
                {
                    if (!teachableSubjects.Contains(sia))
                    {
                        errorDictionary["Môn học"].Add($"Chưa có giáo viên nào được phân công dạy môn {assignmentsDb.First(a => a.SubjectId == sia).Subject.SubjectName}");
                    }
                }
            }
            return errorDictionary;
        }

        public async Task<BaseResponseModel> CheckTeacherAssignment(int schoolId, int yearId)
        {
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId,
                include: query => query.Include(cls => cls.TeacherAssignments));

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(yearId, filter: y => !y.IsDeleted
                , include: query => query.Include(y => y.Terms));

            var classIds = classes.Select(selector => selector.Id).ToList();
            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && a.TeacherId == null,
                include: query => query.Include(a => a.Subject).Include(a => a.StudentClass).Include(a => a.Term));

            var teachers = await _unitOfWork.TeacherRepo.GetV2Async(
                filter: t => !t.IsDeleted && t.Status == (int)TeacherStatus.HoatDong && t.SchoolId == schoolId,
                include: query => query.Include(t => t.TeachableSubjects));

            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();

            // Kiểm tra lỗi trước khi phân công
            var errors = await CheckAssignmentErrors(classes, schoolYear, teachers, assignmentsDb);
            bool hasErrors = errors.Any(kv => kv.Value.Any());

            return new BaseResponseModel
            {
                Status = hasErrors ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                Message = hasErrors ? "Phát hiện lỗi trong phân công." : "Không có lỗi nào.",
                Result = errors 
            };
        }

        public async Task<BaseResponseModel> AutoAssignTeachers(int schoolId, int yearId)
        {
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                 filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId,
                 include: query => query.Include(cls => cls.TeacherAssignments));

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(yearId, filter: y => !y.IsDeleted
                , include: query => query.Include(y => y.Terms));

            var classIds = classes.Select(selector => selector.Id).ToList();
            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && a.TeacherId == null,
                include: query => query.Include(a => a.Subject).Include(a => a.StudentClass).Include(a => a.Term));

            var teachers = await _unitOfWork.TeacherRepo.GetV2Async(
                filter: t => !t.IsDeleted && t.Status == (int)TeacherStatus.HoatDong && t.SchoolId == schoolId,
                include: query => query.Include(t => t.TeachableSubjects));

            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();

            // Kiểm tra lỗi trước khi phân công
            var errors = await CheckAssignmentErrors(classes, schoolYear, teachers, assignmentsDb);

            // Nếu có lỗi, trả về danh sách lỗi theo thực thể
            if (errors.Any(kv => kv.Value.Any()))
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Phát hiện lỗi trong phân công.",
                    Result = errors
                };
            }

            var teacherCapabilities = teachers.ToDictionary(
                teacher => teacher.Id,
                teacher => teacher.TeachableSubjects.ToList()
            );
            var homeroomTeachers = classes.ToDictionary(
                sclass => sclass.Id,
                sclass => (int)sclass.HomeroomTeacherId);

            var terms = schoolYear.Terms.Where(t => !t.IsDeleted);
            var assignmentFirsts = assignmentsDb.Where(a => a.TermId == terms.First().Id).ToList();
            foreach (var term in terms)
            {
                if(term.Id == terms.First().Id)
                    await AssignTeachers(assignmentFirsts, teachers.ToList(), teacherCapabilities, homeroomTeachers);
                else
                    foreach(var item in assignmentFirsts)
                    {
                        var found = assignmentsDb.Where(a => a.TermId == term.Id).FirstOrDefault(a => a.SubjectId == item.SubjectId);
                        found.TeacherId = item.TeacherId;
                        found.Teacher = item.Teacher;
                    }
            }
            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Result = _mapper.Map<List<TeacherAssignmentTermViewModel>>(assignmentsDb)
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
    Dictionary<int, List<TeachableSubject>> teacherCapabilities,
    Dictionary<int, int> homeroomTeachers) 
        {
            // 1. Khởi tạo CpModel và các biến
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

            // 2. Ràng buộc: Mỗi nhiệm vụ chỉ được phân cho một giáo viên
            for (int i = 0; i < numAssignments; i++)
            {
                model.Add(LinearExpr.Sum(from j in Enumerable.Range(0, numTeachers) select assignmentMatrix[i, j]) == 1);
            }

            // 3. Ràng buộc: Giáo viên chỉ có thể dạy các môn và khối lớp mà họ có thể dạy
            for (int i = 0; i < numAssignments; i++)
            {
                var assignment = assignments[i];
                for (int j = 0; j < numTeachers; j++)
                {
                    var teacher = teachers[j];

                    if (teacherCapabilities.ContainsKey(teacher.Id))
                    {
                        var teachableSubjects = teacherCapabilities[teacher.Id]
                            .Where(ts => ts.SubjectId == assignment.SubjectId && ts.Grade == assignment.StudentClass?.Grade)
                            .ToList();

                        if (!teachableSubjects.Any())
                        {
                            model.Add(assignmentMatrix[i, j] == 0);
                        }
                    }
                    else
                    {
                        model.Add(assignmentMatrix[i, j] == 0);
                    }
                }
            }

            // 4. Ràng buộc mềm: Số tiết tối đa của mỗi giáo viên là 17, nhưng có thể vượt quá nếu cần
            List<IntVar> overloadList = new List<IntVar>();
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

                IntVar totalLoad = model.NewIntVar(0, 100, $"totalLoad_{teacherIndex}");
                model.Add(totalLoad == LinearExpr.Sum(teacherLoad));

                IntVar overload = model.NewIntVar(0, 100, $"overload_{teacherIndex}");
                model.Add(overload >= totalLoad - 17);
                model.Add(overload >= 0);
                overloadList.Add(overload);
            }

            // 5. Thiết lập hàm mục tiêu để ưu tiên phân công tất cả các assignments
            LinearExpr objectiveExpr = LinearExpr.Sum(
                from i in Enumerable.Range(0, numAssignments)
                from j in Enumerable.Range(0, numTeachers)
                select assignmentMatrix[i, j]
            ) * 1000; // Trọng số lớn để đảm bảo tất cả các assignments được phân công trước

            // Ưu tiên giáo viên chủ nhiệm và các giáo viên có `IsMain`
            for (int i = 0; i < numAssignments; i++)
            {
                var assignment = assignments[i];
                for (int j = 0; j < numTeachers; j++)
                {
                    var teacher = teachers[j];
                    if (teacherCapabilities.ContainsKey(teacher.Id))
                    {
                        var teachableSubjects = teacherCapabilities[teacher.Id]
                            .Where(ts => ts.SubjectId == assignment.SubjectId && ts.Grade == assignment.StudentClass?.Grade)
                            .ToList();

                        if (teachableSubjects.Any())
                        {
                            var maxAppropriateLevel = teachableSubjects.Max(ts => ts.AppropriateLevel);
                            var isMain = teachableSubjects.Any(ts => ts.IsMain);

                            // Ưu tiên giáo viên chủ nhiệm dạy môn chuyên môn
                            if (homeroomTeachers.ContainsKey(assignment.StudentClassId) &&
                                homeroomTeachers[assignment.StudentClassId] == teacher.Id &&
                                isMain)
                            {
                                objectiveExpr += assignmentMatrix[i, j] * 200;
                            }

                            // Ưu tiên giáo viên có `IsMain = true` và `AppropriateLevel` cao hơn
                            objectiveExpr += assignmentMatrix[i, j] * (isMain ? 20 : 0);
                            objectiveExpr += assignmentMatrix[i, j] * maxAppropriateLevel;
                        }
                    }
                }
            }

            model.Maximize(objectiveExpr);

            // 6. Tạo solver và giải bài toán
            CpSolver solver = new CpSolver();
            solver.StringParameters = "max_time_in_seconds:300 log_search_progress:true";
            CpSolverStatus status = solver.Solve(model);

            // 7. Cập nhật kết quả nếu có lời giải khả thi
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                for (int i = 0; i < numAssignments; i++)
                {
                    for (int j = 0; j < numTeachers; j++)
                    {
                        if (solver.Value(assignmentMatrix[i, j]) == 1)
                        {
                            assignments[i].TeacherId = teachers[j].Id;
                            assignments[i].Teacher = teachers[j];
                        }
                    }
                }
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
