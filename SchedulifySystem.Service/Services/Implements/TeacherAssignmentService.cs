using AutoMapper;
using Google.OrTools.Sat;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
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

        private async Task<Dictionary<string, List<string>>> CheckAssignmentErrors(IEnumerable<StudentClass> classes, SchoolYear schoolYear, IEnumerable<Teacher> teachers, IEnumerable<TeacherAssignment> assignmentsDb)
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
                var subjectsInAssignment = assignmentsDb.Where(s => !s.Subject.IsTeachedByHomeroomTeacher).Select(t => t.SubjectId).Distinct().ToList();
                foreach (var sia in subjectsInAssignment)
                {
                    if (!teachableSubjects.Contains(sia))
                    {
                        errorDictionary["Giáo viên"].Add($"Môn học {assignmentsDb.First(a => a.SubjectId == sia).Subject.SubjectName} chưa có giáo viên nào đảm nhiệm.");
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
                filter: a => classIds.Contains(a.StudentClassId) && a.TeacherId == null && !a.Subject.IsTeachedByHomeroomTeacher,
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

        public async Task<BaseResponseModel> AutoAssignTeachers(int schoolId, int yearId, AutoAssignTeacherModel model)
        {
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                 filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId,
                 include: query => query.Include(cls => cls.TeacherAssignments));

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(yearId, filter: y => !y.IsDeleted
                , include: query => query.Include(y => y.Terms));

            var classIds = classes.Select(selector => selector.Id).ToList();
            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && !a.IsDeleted,
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

            // Kiểm tra tính hợp lệ của các `fixedAssignments`
            var invalidAssignments = new List<FixedTeacherAssignmentModel>();

            if (model.fixedAssignment != null)
            {
                foreach (var fixedAssignment in model.fixedAssignment)
                {
                    var assignment = assignmentsDb.FirstOrDefault(a => a.Id == fixedAssignment.AssignmentId);
                    var teacher = teachers.FirstOrDefault(t => t.Id == fixedAssignment.TeacherId);

                    if (assignment == null || teacher == null)
                    {
                        // Nếu không tìm thấy assignment hoặc giáo viên, thêm vào danh sách không hợp lệ
                        invalidAssignments.Add(fixedAssignment);
                        continue;
                    }

                    // Kiểm tra giáo viên có khả năng dạy môn học và lớp này không
                    if (teacherCapabilities.ContainsKey(teacher.Id))
                    {
                        var teachableSubjectss = teacherCapabilities[teacher.Id]
                            .Where(ts => ts.SubjectId == assignment.SubjectId && ts.Grade == assignment.StudentClass?.Grade)
                            .ToList();

                        if (!teachableSubjectss.Any())
                        {
                            invalidAssignments.Add(fixedAssignment);
                        }
                    }
                    else
                    {
                        invalidAssignments.Add(fixedAssignment);
                    }
                }
            }

            if (invalidAssignments.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Giáo viên hoặc phân công không tồn tại!",
                    Result = invalidAssignments
                };
            }

            foreach (var term in terms)
            {
                if (term.Id == terms.First().Id)
                {
                    // Phân công giáo viên cho kỳ đầu tiên
                    await AssignTeachers(assignmentFirsts,
                        teachers.ToList(), teacherCapabilities,
                        homeroomTeachers, model.fixedAssignment,
                        model.classCombinations);
                }
                else
                {
                    // Sao chép kết quả từ kỳ đầu tiên cho kỳ tiếp theo
                    foreach (var assignment in assignmentFirsts)
                    {
                        // Tìm nhiệm vụ trong kỳ hiện tại tương ứng với môn học đã phân công ở kỳ đầu tiên
                        var foundAssignment = assignmentsDb.FirstOrDefault(a => a.TermId == term.Id && a.SubjectId == assignment.SubjectId && a.StudentClassId == assignment.StudentClassId);

                        if (foundAssignment != null && assignment.TeacherId != null)
                        {
                            foundAssignment.TeacherId = assignment.TeacherId;
                            foundAssignment.Teacher = assignment.Teacher;
                        }
                    }
                }
            }


            var result = new List<TeacherAssignmentTermViewModel>();
            foreach (var term in terms)
            {
                var assignmentByTerm = assignmentsDb.Where(a => a.TermId == term.Id).OrderBy(a => a.StudentClass.Name).ThenBy(a => a.Subject.SubjectName);
                var teacherPeriodCounts = assignmentByTerm.Where(a => a.TeacherId != null)
                .GroupBy(a => a.TeacherId)
                .Select(g =>
                {
                    var teacher = teachers.First(t => t.Id == g.Key);
                    return new TeacherPeriodCountViewModel()
                    {
                        TeacherId = (int)g.Key,
                        TeacherAbbreviation = teacher.Abbreviation,
                        TeacherName = teacher.FirstName + " " + teacher.LastName,
                        TotalPeriodsPerWeek = g.Sum(a => a.PeriodCount)
                    };
                })
                .ToList();
                var minimalData = assignmentByTerm.Select(a => new TeacherAssignmentMinimalData() { AssignmentId = a.Id, TeacherId = a.TeacherId }).ToList();

                result.Add(new TeacherAssignmentTermViewModel()
                {
                    TermId = term.Id,
                    TermName = term.Name,
                    Assignments = _mapper.Map<List<TeacherAssignmentViewModel>>(assignmentByTerm),
                    TeacherPeriodsCount = teacherPeriodCounts,
                    AssignmentMinimalData = minimalData
                });
            }


            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Phân công giáo viên thành công.",
                Result = result
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
                , include: query => query.Include(ta => ta.Subject).Include(ta => ta.Teacher));

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
    Dictionary<int, int> homeroomTeachers,
    List<FixedTeacherAssignmentModel>? fixedAssignments,
    List<ClassCombination>? classCombinations)
        {
            // Bước 1: Phân công giáo viên cố định trước
            if (fixedAssignments != null)
            {
                foreach (var fixedAssignment in fixedAssignments)
                {
                    var assignment = assignments.FirstOrDefault(a => a.Id == fixedAssignment.AssignmentId);
                    var teacher = teachers.FirstOrDefault(t => t.Id == fixedAssignment.TeacherId);
                    assignment.TeacherId = fixedAssignment.TeacherId;
                    assignment.Teacher = teacher;
                }
            }

            // Bước 2: Lọc danh sách các nhiệm vụ chưa được phân công giáo viên
            var remainingAssignments = assignments.Where(a => a.TeacherId == null).ToList();
            int numRemainingAssignments = remainingAssignments.Count;
            int numTeachers = teachers.Count;

            // Bước 3: Khởi tạo mô hình CP (Constraint Programming) để tối ưu hóa phân công giáo viên
            CpModel model = new CpModel();
            BoolVar[,] assignmentMatrix = new BoolVar[numRemainingAssignments, numTeachers];

            // Tạo biến nhị phân cho từng nhiệm vụ và giáo viên (1 nếu được phân công, 0 nếu không)
            for (int i = 0; i < numRemainingAssignments; i++)
            {
                for (int j = 0; j < numTeachers; j++)
                {
                    assignmentMatrix[i, j] = model.NewBoolVar($"assign_{i}_{j}");
                }
            }

            // Bước 4: Ràng buộc - Mỗi nhiệm vụ chỉ được phân công cho một giáo viên
            for (int i = 0; i < numRemainingAssignments; i++)
            {
                model.Add(LinearExpr.Sum(from j in Enumerable.Range(0, numTeachers) select assignmentMatrix[i, j]) == 1);
            }

            // Bước 5: Ràng buộc - Lớp gộp sẽ chỉ phân công cho 1 giáo viên 
            if (classCombinations != null)
            {
                foreach (var combination in classCombinations)
                {
                    // lấy ra các assign của lớp gộp 
                    var relatedAssignments = remainingAssignments
                        .Where(a => combination.ClassIds.Contains(a.StudentClassId) && a.SubjectId == combination.SubjectId)
                        .ToList();

                    if (relatedAssignments.Any())
                    {
                        var assignmentIndices = relatedAssignments.Select(a => remainingAssignments.IndexOf(a)).ToList();

                        foreach (int teacherIndex in Enumerable.Range(0, numTeachers))
                        {
                            // tạo 1 biến đại diện cho giáo viên sẽ được assign cho toàn bộ assignment trong lớp gộp 
                            BoolVar teacherAssignedToCombination = model.NewBoolVar($"teacher_{teacherIndex}_combination_{combination.SubjectId}");

                            // kiểm tra xem các phân công có trong lớp gộp có được gán cùng 1 giáo viên 
                            foreach (var assignmentIndex in assignmentIndices)
                            {
                                model.Add(assignmentMatrix[assignmentIndex, teacherIndex] == teacherAssignedToCombination);
                            }
                        }
                    }
                }
            }

            // Bước 6: Khởi tạo từ điển để lưu tổng số tiết của mỗi giáo viên
            Dictionary<int, IntVar> totalLoadDict = new Dictionary<int, IntVar>();
            List<IntVar> overloadList = new List<IntVar>();

            // Ràng buộc số tiết tối đa là 17 cho mỗi giáo viên
            foreach (var teacher in teachers)
            {
                int teacherIndex = teachers.IndexOf(teacher);
                List<IntVar> teacherLoad = new List<IntVar>();

                // Tính tổng số tiết mà giáo viên được phân công
                for (int i = 0; i < numRemainingAssignments; i++)
                {
                    IntVar assignedLoad = model.NewIntVar(0, remainingAssignments[i].PeriodCount, $"load_{i}_{teacherIndex}");
                    model.Add(assignedLoad == remainingAssignments[i].PeriodCount * assignmentMatrix[i, teacherIndex]);
                    teacherLoad.Add(assignedLoad);
                }

                // Tính tổng số tiết cho giáo viên hiện tại
                IntVar totalLoad = model.NewIntVar(0, 100, $"totalLoad_{teacherIndex}");
                model.Add(totalLoad == LinearExpr.Sum(teacherLoad));
                totalLoadDict[teacherIndex] = totalLoad;

                // Tạo biến Boolean để kiểm tra nếu giáo viên có thể dạy không vượt quá 17 tiết
                BoolVar canAssignWithinLimit = model.NewBoolVar($"canAssignWithinLimit_{teacherIndex}");
                model.Add(totalLoad <= 17).OnlyEnforceIf(canAssignWithinLimit);

                // Tạo biến cho số tiết vượt quá giới hạn 17 tiết
                IntVar overload = model.NewIntVar(0, 100, $"overload_{teacherIndex}");
                model.Add(overload >= totalLoad - 17);
                model.Add(overload >= 0);
                overloadList.Add(overload);

                // Chỉ cho phép vượt quá 17 tiết nếu không còn lựa chọn nào khác
                model.Add(overload == 0).OnlyEnforceIf(canAssignWithinLimit.Not());
            }

            // Bước 7: Thiết lập hàm mục tiêu để tối ưu hóa việc phân công giáo viên
            // Tối ưu hóa phân công để đảm bảo tất cả các nhiệm vụ được thực hiện
            LinearExpr objectiveExpr = LinearExpr.Sum(
                from i in Enumerable.Range(0, numRemainingAssignments)
                from j in Enumerable.Range(0, numTeachers)
                select assignmentMatrix[i, j]
            ) * 1000;

            // Bước 8: Ưu tiên giáo viên chủ nhiệm và giáo viên có `IsMain` và `AppropriateLevel` cao
            for (int i = 0; i < numRemainingAssignments; i++)
            {
                var assignment = remainingAssignments[i];
                var studentClassId = assignment.StudentClassId;
                int? homeroomTeacherId = homeroomTeachers.ContainsKey(studentClassId) ? homeroomTeachers[studentClassId] : null;

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

                            // Nếu giáo viên chủ nhiệm có thể dạy môn `IsMain`, ưu tiên phân công
                            if (homeroomTeacherId == teacher.Id && isMain)
                            {
                                objectiveExpr += assignmentMatrix[i, j] * 200;
                            }

                            // Nếu không, ưu tiên giáo viên có `IsMain` và `AppropriateLevel` cao
                            if (isMain)
                            {
                                objectiveExpr += assignmentMatrix[i, j] * 20;
                            }
                            objectiveExpr += assignmentMatrix[i, j] * maxAppropriateLevel;
                        }
                    }
                }
            }

            // Bước 9: Giảm thiểu số tiết vượt quá 17 tiết
            objectiveExpr -= LinearExpr.Sum(overloadList) * 100;

            // Thiết lập hàm mục tiêu
            model.Maximize(objectiveExpr);

            // Bước 10: Sử dụng solver để tìm giải pháp tối ưu
            CpSolver solver = new CpSolver();
            solver.StringParameters = "max_time_in_seconds:300 log_search_progress:true";
            CpSolverStatus status = solver.Solve(model);

            // Bước 11: Cập nhật kết quả nếu tìm thấy lời giải khả thi
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                for (int i = 0; i < numRemainingAssignments; i++)
                {
                    for (int j = 0; j < numTeachers; j++)
                    {
                        if (solver.Value(assignmentMatrix[i, j]) == 1)
                        {
                            remainingAssignments[i].TeacherId = teachers[j].Id;
                            remainingAssignments[i].Teacher = teachers[j];
                        }
                    }
                }
            }
            else
            {
                // Thông báo nếu không tìm thấy lời giải khả thi
                Console.WriteLine("Không tìm thấy lời giải khả thi.");
            }
        }


        public Task<BaseResponseModel> UpdateAssignment(int assignmentId)
        {
            throw new NotImplementedException();
        }


    }
}
