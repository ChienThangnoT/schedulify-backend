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

        private async Task<Dictionary<string, List<string>>> CheckAssignmentErrors(IEnumerable<StudentClass> classes, SchoolYear schoolYear, IEnumerable<Teacher> teachers, IEnumerable<TeacherAssignment> assignmentsDb, AutoAssignTeacherModel model)
        {
            // Khởi tạo dictionary để lưu lỗi theo từng thực thể
            var errorDictionary = new Dictionary<string, List<string>>()
                {
                    { "Giáo viên", new List<string>() },
                    { "Lớp học", new List<string>() },
                    { "Năm học", new List<string>() },
                    { "Môn học", new List<string>() },
                    { "Lớp gộp", new List<string>() }
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

            var teacherCapabilities = teachers.ToDictionary(
               teacher => teacher.Id,
               teacher => teacher.TeachableSubjects.ToList()
            );

            // Kiểm tra tính hợp lệ của các `fixedAssignments`
            var invalidAssignments = new List<FixedTeacherAssignmentModel>();

            if (model.fixedAssignment != null)
            {
                foreach (var fixedAssignment in model.fixedAssignment)
                {
                    var assignment = assignmentsDb.FirstOrDefault(a => a.Id == fixedAssignment.AssignmentId);
                    var teacher = teachers.FirstOrDefault(t => t.Id == fixedAssignment.TeacherId);

                    if (assignment == null)
                    {
                        errorDictionary["Lớp học"].Add("Phân công cố định không tồn tại!.");
                        continue;
                    }

                    if (teacher == null)
                    {
                        errorDictionary["Giáo viên"].Add("Phân công cố định giáo viên không tồn tại!.");
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
                            var teacherObj = teachers.FirstOrDefault(t => t.Id == teacher.Id);
                            errorDictionary["Giáo viên"].Add($"Giáo viên {teacher.FirstName} {teacher.LastName} không thể dạy môn {assignmentsDb.First(a => a.SubjectId == assignment.SubjectId).Subject.SubjectName} khối {(int)assignment.StudentClass?.Grade}!.");
                        }
                    }
                    else
                    {
                        var teacherObj = teachers.FirstOrDefault(t => t.Id == teacher.Id);
                        errorDictionary["Giáo viên"].Add($"Giáo viên {teacher.FirstName} {teacher.LastName} không thể dạy môn {assignmentsDb.First(a => a.SubjectId == assignment.SubjectId).Subject.SubjectName} khối {(int)assignment.StudentClass?.Grade}!.");
                    }
                }
            }

            if (model.classCombinations != null)
            {

                foreach (var combination in model.classCombinations)
                {
                    var curriculum = classes.Select(cls => cls.StudentClassGroup.Curriculum)
                    .Distinct().ToDictionary(c => c.Id, c => c.CurriculumDetails);

                    var clsCombination = classes.Where(s => combination.ClassIds.Contains(s.Id)).ToList();
                    var result = IsClassCombinationable(curriculum, clsCombination, combination.Session, combination.SubjectId);
                    if (!result)
                    {
                        errorDictionary["Lớp gộp"].Add($"không thể tạo lớp gộp cho lớp {string.Join(", ", clsCombination.Select(s => s.Name))} do số lượng tiết môn {assignmentsDb.First(a => a.SubjectId == combination.SubjectId).Subject.SubjectName} không giống nhau!.");
                    }
                }

            }

            return errorDictionary;
        }

        public async Task<BaseResponseModel> CheckTeacherAssignment(int schoolId, int yearId, AutoAssignTeacherModel model)
        {
            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: cls => !cls.IsDeleted && cls.SchoolId == schoolId && cls.SchoolYearId == yearId,
                include: query => query.Include(cls => cls.TeacherAssignments)
                .Include(cls => cls.StudentClassGroup).ThenInclude(cg => cg.Curriculum).ThenInclude(c => c.CurriculumDetails));

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
            var errors = await CheckAssignmentErrors(classes, schoolYear, teachers, assignmentsDb, model);
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
                 include: query => query.Include(cls => cls.TeacherAssignments)
                 .Include(cls => cls.StudentClassGroup).ThenInclude(cg => cg.Curriculum).ThenInclude(c => c.CurriculumDetails));

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(yearId, filter: y => !y.IsDeleted
                , include: query => query.Include(y => y.Terms));

            var classIds = classes.Select(selector => selector.Id).ToList();
            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: a => classIds.Contains(a.StudentClassId) && !a.IsDeleted,
                include: query => query.Include(a => a.Subject).Include(a => a.StudentClass).ThenInclude(a => a.StudentClassGroup).Include(a => a.Term));

            var curriculum = classes.Select(cls => cls.StudentClassGroup.Curriculum)
                .Distinct().ToDictionary(c => c.Id, c => c.CurriculumDetails);

            var teachers = await _unitOfWork.TeacherRepo.GetV2Async(
                filter: t => !t.IsDeleted && t.Status == (int)TeacherStatus.HoatDong && t.SchoolId == schoolId,
                include: query => query.Include(t => t.TeachableSubjects));

            var teachableSubjects = teachers.SelectMany(t => t.TeachableSubjects).Select(t => t.SubjectId).ToList();

            // Kiểm tra lỗi trước khi phân công
            var errors = await CheckAssignmentErrors(classes, schoolYear, teachers, assignmentsDb, model);

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
                if (term.Id == terms.First().Id)
                {
                    // Phân công giáo viên cho kỳ đầu tiên
                    await AssignTeachers(model,assignmentFirsts,
                        teachers.ToList(), teacherCapabilities,
                        homeroomTeachers, model.fixedAssignment,
                        model.classCombinations,
                        curriculum);
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

            var getHomeRoomTeacher = await _unitOfWork.StudentClassesRepo.GetByIdAsync(classId, filter: t => !t.IsDeleted && t.HomeroomTeacherId != null);

            var assignments = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: t => t.StudentClassId == classId && t.IsDeleted == false && (termId == null || t.TermId == termId)
                , include: query => query.Include(ta => ta.Subject).Include(ta => ta.Teacher));

            var teacherNotAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(assignments.Where(a => a.TeacherId == null));
            var teacherAssigntView = _mapper.Map<List<TeacherAssignmentViewModel>>(assignments.Where(a => a.TeacherId != null));
            HomeRoomTeacherView teacherHomeRoom = new HomeRoomTeacherView { TeacherId = getHomeRoomTeacher?.HomeroomTeacherId, Abbreviation = getHomeRoomTeacher?.Teacher?.Abbreviation};
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_TEACHER_ASSIGNTMENT_SUCCESS,
                Result = new
                {
                    HomeRoomTeacherOfClass = teacherHomeRoom,
                    TeacherAssigntView = teacherAssigntView,
                    TeacherNotAssigntView = teacherNotAssigntView
                }
            };

        }

        public async Task AssignTeachers(
            AutoAssignTeacherModel para,
    List<TeacherAssignment> assignments,
    List<Teacher> teachers,
    Dictionary<int, List<TeachableSubject>> teacherCapabilities,
    Dictionary<int, int> homeroomTeachers,
    List<FixedTeacherAssignmentModel>? fixedAssignments,
    List<ClassCombination>? classCombinations,
    Dictionary<int, ICollection<CurriculumDetail>>? curriculums)
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

            if (classCombinations != null)
            {
                var fixedClassCombinations = classCombinations.Where(c => c.TeacherId != null);
                foreach (var combination in fixedClassCombinations)
                {
                    var relatedAssignments = assignments
                        .Where(a => combination.ClassIds.Contains(a.StudentClassId) &&
                                    a.SubjectId == combination.SubjectId &&
                                    IsHaveSubjectInSession(curriculums, a.StudentClass, combination.Session, combination.SubjectId))
                        .ToList();
                    var teacher = teachers.FirstOrDefault(t => t.Id == combination.TeacherId);
                    if (teacher != null)
                    {
                        relatedAssignments.ForEach(a =>
                        {
                            a.TeacherId = teacher.Id;
                            a.Teacher = teacher;
                        });
                    }

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
            IntVar totalGroupViolations = model.NewIntVar(0, 1000, "totalGroupViolations");
            if (classCombinations != null)
            {
                LinearExpr groupViolationExpr = LinearExpr.Sum(new List<LinearExpr>());// Tính tổng số vi phạm lớp gộp

                foreach (var combination in classCombinations.Where(c => c.TeacherId == null))
                {
                    var relatedAssignments = remainingAssignments
                        .Where(a => combination.ClassIds.Contains(a.StudentClassId) &&
                                    a.SubjectId == combination.SubjectId &&
                                    IsHaveSubjectInSession(curriculums, a.StudentClass, combination.Session, combination.SubjectId))
                        .ToList();

                    if (relatedAssignments.Any())
                    {
                        // Tạo biến đại diện cho giáo viên phân công cho lớp gộp
                        IntVar teacherForCombination = model.NewIntVar(0, numTeachers - 1, $"teacher_combination_{combination.SubjectId}");

                        // Đảm bảo mỗi assignment trong lớp gộp được phân công cho cùng một giáo viên
                        foreach (var assignment in relatedAssignments)
                        {
                            int assignmentIndex = remainingAssignments.IndexOf(assignment);
                            model.Add(LinearExpr.Sum(
                                from j in Enumerable.Range(0, numTeachers)
                                select assignmentMatrix[assignmentIndex, j] * j) == teacherForCombination);
                        }
                    }
                }

                // Ràng buộc tổng số vi phạm lớp gộp
                model.Add(totalGroupViolations == groupViolationExpr);
            }

            // Bước 6: Khởi tạo từ điển để lưu tổng số tiết của mỗi giáo viên
            Dictionary<int, IntVar> totalLoadDict = new Dictionary<int, IntVar>();
            List<IntVar> overloadList = new List<IntVar>();
            var fixedAssgm = assignments.Where(a => a.TeacherId != null).ToList();

            // Ràng buộc số tiết tối đa là 17 cho mỗi giáo viên
            foreach (var teacher in teachers)
            {
                int teacherIndex = teachers.IndexOf(teacher);

                // Tính tổng số tiết cố định cho giáo viên hiện tại (nếu có)
                int fixedLoad = fixedAssgm.Where(a => a.TeacherId == teacher.Id).Sum(a => a.PeriodCount);

                // Danh sách để lưu các biến tổng số tiết của giáo viên (cả cố định và mới)
                List<IntVar> teacherLoad = new List<IntVar>();

                // Thêm số tiết cố định (fixedLoad) dưới dạng một giá trị cố định
                if (fixedLoad > 0)
                {
                    IntVar fixedLoadVar = model.NewConstant(fixedLoad);
                    teacherLoad.Add(fixedLoadVar);
                }

                // Tính tổng số tiết mà giáo viên được phân công mới (nhiệm vụ chưa phân công)
                for (int i = 0; i < numRemainingAssignments; i++)
                {
                    IntVar assignedLoad = model.NewIntVar(0, remainingAssignments[i].PeriodCount, $"load_{i}_{teacherIndex}");
                    model.Add(assignedLoad == remainingAssignments[i].PeriodCount * assignmentMatrix[i, teacherIndex]);
                    teacherLoad.Add(assignedLoad);
                }

                // Tính tổng số tiết cho giáo viên hiện tại, bao gồm cả phân công cố định và phân công mới
                IntVar totalLoad = model.NewIntVar(0, 100, $"totalLoad_{teacherIndex}");
                model.Add(totalLoad == LinearExpr.Sum(teacherLoad));
                totalLoadDict[teacherIndex] = totalLoad;

                // Đặt giới hạn cho tổng số tiết của mỗi giáo viên
                model.Add(totalLoad <= para.MaxPeriodsPerWeek);
                model.Add(totalLoad >= para.MinPeriodsPerWeek);
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

                            // Ưu tiên cao hơn cho giáo viên có `IsMain` và mức `AppropriateLevel` cao
                            if (homeroomTeacherId == teacher.Id && isMain)
                            {
                                objectiveExpr += assignmentMatrix[i, j] * 200;
                            }

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

            // Phạt vi phạm lớp gộp
            objectiveExpr -= totalGroupViolations * 500;

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

        public bool IsClassCombinationable(Dictionary<int, ICollection<CurriculumDetail>> curriculum, List<StudentClass> sClasses, MainSession session, int subjectId)
        {
            var firstClass = sClasses.First();
            var fCurriculumDetails = curriculum[(int)firstClass.StudentClassGroup.CurriculumId];
            var fcds = fCurriculumDetails.FirstOrDefault(c => c.SubjectId == subjectId);
            if (fcds == null) return false;
            var fcountPeriod = (int)session == firstClass.MainSession ? fcds.MainSlotPerWeek : fcds.SubSlotPerWeek;
            for (int i = 1; i < sClasses.Count; i++)
            {
                var curriculumDetails = curriculum[(int)firstClass.StudentClassGroup.CurriculumId];
                var cds = curriculumDetails.FirstOrDefault(c => c.SubjectId == subjectId);
                if (cds == null) return false;
                var countPeriod = (int)session == firstClass.MainSession ? cds.MainSlotPerWeek : cds.SubSlotPerWeek;
                if (fcountPeriod != countPeriod && fcountPeriod != 0) return false;
            }
            return true;
        }

        private bool IsHaveSubjectInSession(Dictionary<int, ICollection<CurriculumDetail>> curriculum, StudentClass sClass, MainSession session, int subjectId)
        {
            if (curriculum.ContainsKey((int)sClass.StudentClassGroup.CurriculumId))
            {
                var curriculumDetails = curriculum[(int)sClass.StudentClassGroup.CurriculumId];
                if (sClass.MainSession == (int)session)
                {
                    return curriculumDetails.Any(c => c.SubjectId == subjectId && c.MainSlotPerWeek > 0);
                }
                else if (sClass.IsFullDay)
                {
                    return curriculumDetails.Any(c => c.SubjectId == subjectId && c.SubSlotPerWeek > 0);
                }
            }
            return false;
        }

        public Task<BaseResponseModel> UpdateAssignment(int assignmentId)
        {
            throw new NotImplementedException();
        }


    }
}
