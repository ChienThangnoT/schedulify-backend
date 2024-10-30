using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using static System.Collections.Specialized.BitVector32;

namespace SchedulifySystem.Service.Services.Implements
{
    public class TimeTableService : ITimetableService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Random _random = new();

        private static readonly int AVAILABLE_SLOT_PER_WEEK = 61;
        private readonly int NUMBER_OF_GENERATIONS = int.MaxValue;
        private readonly int INITIAL_NUMBER_OF_INDIVIDUALS = 1000;
        private readonly float MUTATION_RATE = 0.1f;
        private readonly ESelectionMethod SELECTION_METHOD = ESelectionMethod.RankSelection;
        private readonly ECrossoverMethod CROSSOVER_METHOD = ECrossoverMethod.SinglePoint;
        private readonly EChromosomeType CHROMOSOME_TYPE = EChromosomeType.ClassChromosome;
        private readonly EMutationType MUTATION_TYPE = EMutationType.Default;
        private static readonly List<string> DAY_OF_WEEKS = new List<string>() { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };
        private static readonly List<string> SLOTS = new List<string>() { "Tiết 1", "Tiết 2", "Tiết 3", "Tiết 4", "Tiết 5", "Tiết 6", "Tiết 7", "Tiết 8", "Tiết 9", "Tiết 10" };

        public TimeTableService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Generate
        public async Task<BaseResponseModel> Generate(GenerateTimetableModel parameters)
        {
            var (classes, teachers, subjects, assignments, timetableFlags) = await GetData(parameters);

            var root = CreateRootIndividual(classes, teachers, assignments, subjects, timetableFlags, parameters);

            // tạo quần thể ban đầu ( các các thể sẽ có data ngẫu nhiên khác nhau ) và tính toán độ thích nghi
            //bao gồm tkb root và các tp của tkb
            var timetablePopulation = CreateInitialPopulation(root, parameters);

            //timetableIdBacklog là biến để theo dõi xem cá thể tốt nhất có bị lặp lại qua nhiều vòng lặp liên tiếp hay không,
            //nhằm phát hiện hiện tượng mắc kẹt tại một giải pháp tối ưu cục bộ
            var timetableIdBacklog = timetablePopulation.First().Id;
            var backlogCount = 0;
            var backlogCountMax = 0;
            var currentId = 1;
            /*timetableIdBacklog, backlogCount, và backlogCountMax: Các biến dùng để theo dõi trạng thái lặp lại của quần thể để tránh 
             * việc mắc kẹt trong tối ưu cục bộ.timetableIdBacklog, backlogCount, và backlogCountMax: 
              Các biến dùng để theo dõi trạng thái lặp lại của quần thể để tránh việc mắc kẹt trong tối ưu cục bộ.*/


            for (var step = 1; step <= NUMBER_OF_GENERATIONS; step++)
            {
                // nếu cá thể tốt nhất trong quần thể có độ thích nghi (Adaptability) nhỏ hơn 1000, quá trình tiến hóa sẽ dừng lại sớm
                if (timetablePopulation.First().Adaptability < 1000)
                    break;

                // lai tạo
                /* Tournament */
                var timetableChildren = new List<TimetableIndividual>();
                var tournamentList = new List<TimetableIndividual>();

                //chọn ra 10 cá thể từ cha mẹ và chọn ra 1 cá thể tốt nhát để lai tạo
                for (var i = 0; i < timetablePopulation.Count; i++)
                    tournamentList.Add(timetablePopulation.Shuffle().Take(10).OrderBy(i => i.Adaptability).First());

                for (var k = 0; k < tournamentList.Count - 1; k += 2)
                {
                    var parent1 = tournamentList[k];
                    var parent2 = tournamentList[k + 1];
                    // lai
                    var children = Crossover(root, [parent1, parent2], parameters);
                    foreach (var child in children)
                    {
                        child.Id = currentId++;
                    }
                    timetableChildren.AddRange(children);
                }

                // Chọn lọc
                timetablePopulation.AddRange(timetableChildren);
                //TabuSearch(timetablePopulation[0], parameters);
                timetablePopulation = timetablePopulation.Where(u => u.Age < u.Longevity).OrderBy(i => i.Adaptability).Take(100).ToList();

                var best = timetablePopulation.First();
                best.ConstraintErrors = [.. best.ConstraintErrors.OrderBy(e => e.Code)];

                Console.SetCursorPosition(0, 0);
                Console.Clear();
                Console.WriteLine(
                    $"step {step}, " +
                    $"best score {best.Adaptability}\n" +
                    $"errors: ");
                var errors = best.ConstraintErrors.Where(error => error.IsHardConstraint == true).ToList();
                foreach (var error in errors.Take(20))
                    Console.WriteLine("  " + error.Description);
                if (errors.Count > 20)
                    Console.WriteLine("  ...");
                Console.WriteLine("warning: ");
                var warnings = best.ConstraintErrors.Where(error => error.IsHardConstraint == false).ToList();
                foreach (var error in warnings.Take(20))
                    Console.WriteLine("  " + error.Description);
                if (warnings.Count > 20)
                    Console.WriteLine("  ...");


                /*kiểm tra xem id cá thể tốt nhất hiện tại có = timetableIdBacklog ko
                 nếu bằng tức là cá thể tốt nhất chưa thay đổi, thuật toán đang mắc kẹt ở vòng lặp này
                backlogCount sẽ đếm số vòng lặp liên tiếp giữ nguyên của cá thể tốt nhất
                 */
                if (timetableIdBacklog == best.Id)
                {
                    backlogCount++;
                    if (backlogCount > 500)
                    {
                        timetablePopulation = CreateInitialPopulation(root, parameters);
                        backlogCountMax = backlogCount;
                        backlogCount = 0;
                        timetableIdBacklog = -1;
                    }
                }
                else
                {
                    timetableIdBacklog = best.Id;
                    backlogCountMax = backlogCountMax < backlogCount ? backlogCount : backlogCountMax;
                    backlogCount = 0;
                }

                Console.WriteLine($"backlog count:  {backlogCount}\t max: {backlogCountMax}");
                //Console.WriteLine("time: " + sw.Elapsed.ToString());
            }

            var timetableFirst = timetablePopulation.OrderBy(i => i.Adaptability).First();

            //var timetableDb = _mapper.Map<Timetable>(timetableFirst);
            //timetableFirst.Id = timetableDb.Id = Guid.NewGuid();
            //timetableFirst.StartYear = timetableDb.StartYear = parameters.StartYear;
            //timetableFirst.EndYear = timetableDb.EndYear = parameters.EndYear;
            //timetableFirst.Semester = timetableDb.Semester = parameters.Semester;
            //timetableFirst.Name = timetableDb.Name = "Thời khóa biểu mới";
            //foreach (var unit in timetableDb.TimetableUnits)
            //    unit.TimetableId = timetableDb.Id;
            //JsonSerializerOptions jso = new JsonSerializerOptions();
            //jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            //timetableDb.Parameters = JsonSerializer.Serialize(parameters, jso);
            //_context.Add(timetableDb);
            //_context.SaveChanges();

            //sw.Stop();
            //Console.SetCursorPosition(0, 0);
            //Console.Clear();
            //Console.WriteLine(sw.Elapsed.ToString() + ", " + backlogCountMax);

            timetableFirst.ToCsv();
            timetableFirst.TimetableFlag.ToCsv(timetableFirst.Classes);

            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = timetableFirst };
        }
        #endregion

        #region Convert Data 
        public async void ConvertData(GenerateTimetableModel parameters)
        {
            parameters.NoAssignTimetablePeriods = _mapper.Map<List<ClassPeriodScheduleModel>>(parameters.NoAssignPeriodsPara);
            parameters.FreeTimetablePeriods = _mapper.Map<List<ClassPeriodScheduleModel>>(parameters.FreeTimetablePeriodsPara);
        }
        #endregion

        #region GetData -- Thắng
        public async Task<(
            List<ClassScheduleModel>,
            List<TeacherScheduleModel>,
            List<SubjectScheduleModel>,
            List<TeacherAssigmentScheduleModel>,
            ETimetableFlag[,]
            )> GetData(GenerateTimetableModel parameters)
        {
            ConvertData(parameters);

            var classes = new List<ClassScheduleModel>();
            var teachers = new List<TeacherScheduleModel>();
            var subjects = new List<SubjectScheduleModel>();
            var assignments = new List<TeacherAssigmentScheduleModel>();
            var timetableUnits = new List<ClassPeriodScheduleModel>();

            ETimetableFlag[,] timetableFlags = null!;

            var classesDb = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: t => t.SchoolId == parameters.SchoolId &&
                             t.SchoolYearId == parameters.SchoolYearId &&
                             t.IsDeleted == false,
                include: query => query.Include(c => c.SubjectGroup)
                           .ThenInclude(sg => sg.SubjectInGroups).ThenInclude(sig => sig.Subject));

            var groupIds = classesDb.Select(c => c.SubjectGroup.Id).ToList();
            var subjectsDb = (await _unitOfWork.SubjectInGroupRepo.GetV2Async(
                filter: t => t.IsDeleted == false && groupIds.Contains(t.SubjectGroupId),
                            include: query => query.Include(sig => sig.Subject))).ToList();

            //run parallel
            //await Task.WhenAll(classTask, subjectTask).ConfigureAwait(false);

            // Lấy kết quả của các task song song
            //var classesDb = await classTask;
            //var subjectsDb = await subjectTask;


            if (classesDb == null || !classesDb.Any())
            {
                throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            }

            var classesDbList = classesDb.ToList();

            var subjectInClassesDb = classesDb
                .Where(c => c.SubjectGroup != null) // Lọc những lớp có SubjectGroup
                .SelectMany(c => c.SubjectGroup.SubjectInGroups) // Lấy danh sách SubjectInGroup từ SubjectGroup
                .ToList();

            // add vào classes
            /*Tạo đối tượng ClassTCDTO cho từng lớp học từ dữ liệu lấy được và thêm vào danh sách classes
              Nếu số lượng lớp học trong danh sách không khớp với số lớp học yêu cầu từ tham số, phương thức sẽ ném ngoại lệ.
            */
            for (var i = 0; i < classesDbList.Count; i++)
                classes.Add(new ClassScheduleModel(classesDbList[i]));

            /*khởi tạo mảng hai chiều timetableFlags với số dòng là số lớp học và số cột là 61
              số lượng 61 có thể đại diện cho số tiết học trong một kỳ hoặc một tuần học
            */
            timetableFlags = new ETimetableFlag[classes.Count, AVAILABLE_SLOT_PER_WEEK];

            subjects = _mapper.Map<List<SubjectScheduleModel>>(subjectsDb);

            var assignmentTask = _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: t => classesDb.Select(c => c.Id).Contains(t.StudentClassId) && t.IsDeleted == false
                     && t.TermId == parameters.TermId,
                include: query => query.Include(a => a.Teacher));
            //await assignmentTask;

            var assignmentsDb = await assignmentTask.ConfigureAwait(false);
            var assignmentsDbList = assignmentsDb.ToList();

            //get teacher từ assigntmment db
            var teacherIds = assignmentsDb.Select(a => a.TeacherId).Distinct().ToList();

            var teacherTask = _unitOfWork.TeacherRepo.GetAsync(
                filter: t => teacherIds.Contains(t.Id) && t.Status == (int)TeacherStatus.HoatDong && t.IsDeleted == false);

            var teachersDb = await teacherTask.ConfigureAwait(false);
            var teachersDbList = teachersDb.ToList();

            for (var i = 0; i < teachersDbList.Count; i++)
                teachers.Add(new TeacherScheduleModel(teachersDbList[i]));



            // tạo danh sách các assignment
            /*Duyệt qua danh sách các phân công (assignmentsDb), tìm lớp học, môn học, và giáo viên tương ứng cho từng phân công.
             Tạo đối tượng AssignmentTCDTO và thêm vào danh sách assignments.
            */

            for (var i = 0; i < assignmentsDbList.Count; i++)
            {
                var studentClass = classes.FirstOrDefault(c => c.Id == assignmentsDbList[i].StudentClassId);
                var subject = subjects.FirstOrDefault(s => s.SubjectId == assignmentsDbList[i].SubjectId);
                var teacher = teachers.FirstOrDefault(t => t.Id == assignmentsDbList[i].TeacherId);

                // Check if any of the elements are null and handle accordingly
                if (studentClass == null)
                {
                    throw new DefaultException($"Class with Id {assignmentsDbList[i].StudentClassId} not found.");
                }
                if (subject == null)
                {
                    throw new DefaultException($"Subject with Id {assignmentsDbList[i].SubjectId} not found.");
                }
                if (teacher == null)
                {
                    throw new DefaultException($"Teacher with Id {assignmentsDbList[i].TeacherId} not found.");
                }

                // If all exist, proceed with adding to the assignments list
                assignments.Add(new TeacherAssigmentScheduleModel(assignmentsDbList[i], teacher, subject, studentClass));
            }



            // Kiểm tra xem tất cả các lớp đã được phân công đầy đủ hay chưa
            /*Kiểm tra từng lớp học và môn học xem có phân công phù hợp hay không. Với mỗi lớp, kiểm tra rằng:
             Số lượng tiết học của mỗi môn trong phân công có khớp với số tiết học được yêu cầu cho môn đó không (PeriodCount).
             Giáo viên được phân công có hợp lệ không.
            */
            // Kiểm tra xem tất cả các lớp đã được phân công đầy đủ hay chưa
            for (var i = 0; i < classesDbList.Count; i++)
            {
                var periodCount = 0; // Tổng số tiết học trong lớp
                var classPeriodCount = classesDbList[i].PeriodCount; // tổng số tiết yêu cầu của lớp học trong 1 tuần

                var subjectInGroups = classesDbList[i].SubjectGroup.SubjectInGroups.ToList();
                // duyệt qua từng môn học trong lớp
                for (var j = 0; j < subjectInGroups.Count; j++)
                {
                    var subjectClass = subjectInGroups[j];

                    // tìm phân công giáo viên cho môn học
                    var assignment = assignmentsDbList.FirstOrDefault(a =>
                        a.SubjectId == subjectClass.SubjectId);

                    // kiểm tra xem có phân công hay không, nếu không thì ném ngoại lệ
                    if (assignment == null)
                    {
                        var subjectName = subjects.First(s => s.SubjectId == subjectClass.SubjectId).SubjectName;
                        throw new DefaultException($"Lớp {classesDbList[i].Name} chưa được phân công môn {subjectName}.");
                    }

                    // kiểm tra số tiết học có khớp với yêu cầu không
                    if (assignment.PeriodCount != (subjectClass.MainSlotPerWeek + subjectClass.SubSlotPerWeek))
                    {
                        throw new DefaultException($"Số tiết học cho môn {subjects.First(s => s.SubjectId == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} không khớp.");
                    }

                    // kiểm tra xem giáo viên có được phân công không
                    if (assignment.TeacherId == null || assignment.TeacherId == 0)
                    {
                        throw new DefaultException($"Môn {subjects.First(s => s.SubjectId == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} chưa được phân công giáo viên.");
                    }

                    // cộng số tiết của môn vào tổng số tiết của lớp
                    periodCount += (subjectClass.MainSlotPerWeek + subjectClass.SubSlotPerWeek);
                }

                // kiểm tra tổng số tiết của lớp
                if (periodCount != classPeriodCount)
                {
                    throw new DefaultException($"Tổng số tiết học cho lớp {classesDbList[i].Name} không khớp với số yêu cầu.");
                }
            }

            // update fixed period in para
            List<ClassPeriodScheduleModel> fixedPeriods = new List<ClassPeriodScheduleModel>();
            for (int i = 0; i < parameters.FixedPeriodsPara.Count(); i++)
            {
                var fixedPeriod = parameters.FixedPeriodsPara[i];
                var founded = assignments.Where(a => a.Subject.SubjectId == fixedPeriod.SubjectId && a.StudentClass.Id == fixedPeriod.ClassId).FirstOrDefault();
                if (founded == null)
                {
                    throw new NotExistsException($"Tiết cố định không hợp lệ!. Môn học id {fixedPeriod.SubjectId} và lớp id {fixedPeriod.ClassId} không có trong bảng phân công.");
                }
                var period = new ClassPeriodScheduleModel(founded);
                period.StartAt = fixedPeriod.StartAt;
                period.Priority = EPriority.Fixed;
                period.Session = IsMorningSlot(fixedPeriod.StartAt) ? MainSession.Morning : MainSession.Afternoon;
                fixedPeriods.Add(period);
            }
            parameters.FixedPeriods = fixedPeriods;

            // lấy ra ds các phòng học có môn thực hành
            var roomLabsDb = await _unitOfWork.RoomSubjectRepo.GetV2Async(
                filter: r => r.Room.Building.SchoolId == parameters.SchoolId && !r.IsDeleted, include: query => query.Include(r => r.Room));

            // nhóm theo phòng
            var groupByRoom = roomLabsDb
                .GroupBy(r => r.RoomId)
                .Select(g => new RoomSubjectScheduleModel()
                {
                    RoomId = g.Key ?? 0,
                    TeachableSubjectIds = g.Select(r => r.SubjectId ?? 0).ToList(),
                    MaxClassPerTime = g.First().Room?.MaxClassPerTime ?? 1,
                    RoomCode = g.First().Room?.RoomCode ?? "",
                    Name = g.First().Room?.Name ?? ""
                })
                .ToList();
            parameters.PracticeRoomWithSubjects = groupByRoom;

            return (classes, teachers, subjects, assignments, timetableFlags);
        }
        #endregion

        #region TimetableRootIndividual -- long
        /*
         * phương thức này sẽ tạo ra cá thể gốc ban đầu làm nền tảng cho quá trình lai ghép đột biến với tham số bao gồm 
         * danh sách lớp học, giáo viên, phân công tiết học, và timetableFlags là Mảng trạng thái thời khóa biểu cho từng lớp,
         * thể hiện trạng thái của tiết học (còn trống, đã được xếp, v.v.). parameters là Các thông số quy định thêm 
         * vd ds các tiết học đã được xếp cố định, ds lịch bận giáo viên , ds các tiết không xếp, ds các môn có tiết đôi,
         * ds phòng học thực hành vs các môn có thể học trong phòng đó,....
         */
        private static TimetableRootIndividual CreateRootIndividual(
           List<ClassScheduleModel> classes,
           List<TeacherScheduleModel> teachers,
           List<TeacherAssigmentScheduleModel> assignments,
           List<SubjectScheduleModel> subjects,
           ETimetableFlag[,] timetableFlags,
           GenerateTimetableModel parameters)
        {
            for (var i = 0; i < classes.Count; i++)
            {
                // ca sáng thì a sẽ bắt đầu từ 0 (tương ứng tiết 1 trong ngày) còn ca chiều bắt đầu từ 5 (tương ứng tiết 6 trong ngày)
                var a = classes[i].IsFullDay ? 0 : classes[i].MainSession == (int)MainSession.Morning ? 0 : 5;
                // j sẽ là index cho mỗi ngày, (max một tuần 60 tiết), mỗi vòng tăng 10 tức sang ngày mới
                int maxSlot = classes[i].IsFullDay ? 10 : 5;
                for (var j = 1; j < AVAILABLE_SLOT_PER_WEEK; j += 10)
                    // trong ngày j đánh dấu tiết khả dụng để xếp 
                    for (var k = j; k < j + maxSlot; k++)
                        timetableFlags[i, k + a] = ETimetableFlag.Unfilled;

                var list = parameters.FreeTimetablePeriods.Where(u => u.ClassId == classes[i].Id).ToList();
                //Đánh dấu các tiết trong FreeTimetable vs trạng thái none là các tiết k xếp 
                for (var j = 0; j < list.Count; j++)
                    timetableFlags[i, list[j].StartAt] = ETimetableFlag.None;
            }

            // kiểm tra tiết cố định có trùng tiết trống cố định 
            var freePeriodIds = parameters.FreeTimetablePeriods.Select(u => u.StartAt).ToList();
            var fixedPeriodsInvalid = parameters.FixedPeriods.Where(u => freePeriodIds.Contains(u.StartAt));
            if (fixedPeriodsInvalid.Any())
            {
                throw new DefaultException("Tiết cố định và tiết trống cố định trùng nhau! " +
                    $"Tiết cố định {string.Join(", ", fixedPeriodsInvalid.Select(p => $"[{p.SubjectName} - {GetDayAndPeriodString(p.StartAt)}]"))}");
            }


            // Tạo danh sách tiết được xếp sẵn trước
            var timetableUnits = new List<ClassPeriodScheduleModel>();
            timetableUnits.AddRange(parameters.FixedPeriods);

            // Đánh dấu các tiết này vào timetableFlags là các tiết cố định và xếp các tiết này vào slot bao nhiêu  
            for (var i = 0; i < timetableUnits.Count; i++)
            {
                // lấy ra index của class dựa vào class list vs điều kiện là tìm thấy class này trong timetableUnit (các tiết cố định)
                var classIndex = classes.IndexOf(classes.First(c => c.Id == timetableUnits[i].ClassId));
                var startAt = timetableUnits[i].StartAt;
                timetableFlags[classIndex, startAt] = ETimetableFlag.Fixed;
            }

            // Thêm các tiết phân công chưa được xếp vào sau 
            for (var i = 0; i < assignments.Count; i++)
            {
                // đếm ra số tiết đã phân công theo tiết cố định 
                var fixedPeriods = parameters.FixedPeriods.Where(u => u.SubjectId == assignments[i].Subject.SubjectId && u.ClassId == assignments[i].StudentClass.Id).ToList();
                var fixedMorningCount = fixedPeriods.Count(u => u.Session == MainSession.Morning);
                var fixedAfternoonCount = fixedPeriods.Count(u => u.Session == MainSession.Afternoon);
                int fixedMainCount = 0;
                int fixedSubCount = 0;
                if (assignments[i].StudentClass.MainSession == (int)MainSession.Morning)
                {
                    fixedMainCount = fixedMorningCount;
                    fixedSubCount = fixedAfternoonCount;
                }
                else
                {
                    fixedMainCount = fixedAfternoonCount;
                    fixedSubCount = fixedMorningCount;
                }
                if (assignments[i].Subject.MainSlotPerWeek < fixedMainCount)
                {
                    throw new DefaultException($"Tiết cố định không hợp lệ!, lớp {assignments[i].StudentClass.Name} có {assignments[i].Subject.MainSlotPerWeek} tiết {assignments[i].Subject.SubjectName} học vào buổi {(assignments[i].StudentClass.MainSession == (int)MainSession.Morning ? "Sáng" : "Chiều")}");
                }
                if (assignments[i].Subject.SubSlotPerWeek < fixedSubCount)
                {
                    throw new DefaultException($"Tiết cố định không hợp lệ!, lớp {assignments[i].StudentClass.Name} có {assignments[i].Subject.SubSlotPerWeek} tiết {assignments[i].Subject.SubjectName} học vào buổi {(assignments[i].StudentClass.MainSession == (int)MainSession.Morning ? "Chiều" : "Sáng")}");
                }
                // phân công các tiết còn lại chưa đc xắp cố định vào tkb 
                for (var j = 0; j < assignments[i].Subject.MainSlotPerWeek - fixedMainCount; j++)
                {
                    var period = new ClassPeriodScheduleModel(assignments[i]);
                    period.Session = (MainSession)assignments[i].StudentClass.MainSession;
                    timetableUnits.Add(period);
                }

                for (var j = 0; j < assignments[i].Subject.SubSlotPerWeek - fixedAfternoonCount; j++)
                {
                    var period = new ClassPeriodScheduleModel(assignments[i]);
                    period.Session = assignments[i].StudentClass.MainSession == (int)MainSession.Morning ? MainSession.Afternoon : MainSession.Morning;
                    timetableUnits.Add(period);
                }

            }

            // Danh sách các môn có tiết đôi 
            var doubleSubjects = subjects.Where(s => s.IsDoublePeriod).ToList();

            for (var i = 0; i < classes.Count; i++)
            {
                //lấy ra ds tiết học của lớp đó trong timetableUnits
                var classTimetableUnits = timetableUnits.Where(u => u.ClassId == classes[i].Id).ToList();
                var mainSession = classes[i].MainSession;
                for (var j = 0; j < doubleSubjects.Count; j++)
                {
                    //lấy ra ds tiết học đôi  theo chính khóa 

                    var mainPeriods = classTimetableUnits
                        .Where(u => doubleSubjects[j].SubjectId == u.SubjectId && (int)u.Session == mainSession).Take(TakeNumberDoubleSlot(doubleSubjects[j].MainSlotPerWeek)).ToList();

                    var subPeriods = classTimetableUnits
                        .Where(u => doubleSubjects[j].SubjectId == u.SubjectId && (int)u.Session != mainSession).Take(TakeNumberDoubleSlot(doubleSubjects[j].SubSlotPerWeek)).ToList();

                    mainPeriods.AddRange(subPeriods);
                    //đặt ưu tiên là tiết đôi 
                    for (var k = 0; k < mainPeriods.Count; k++)
                    {
                        mainPeriods[k].Priority = EPriority.Double;
                    }
                }
            }
            //sắp xếp lại danh sách timetableUnits theo thứ tự tên lớp học và tạo ra một danh sách mới với thứ tự đã được sắp xếp
            timetableUnits = [.. timetableUnits.OrderBy(u => u.ClassName)];

            return new TimetableRootIndividual(timetableFlags, timetableUnits, classes, teachers, doubleSubjects);
        }


        #endregion 

        #region Clone
        /*
        *Tạo một bản sao của cá thể gốc (root) để làm cá thể mới. đơn giản là data giống cá thể cũ nhưng là một instance mới 
        */
        private TimetableIndividual Clone(TimetableIndividual src)
        {
            // lấy ra số lượng lớp (0) tức là lấy ra số lượng của mảng thứ nhất tức theo trục x 
            var classCount = src.TimetableFlag.GetLength(0);
            // tương tự lấy ra số lượng slot (1) lấy mảng thứ 2 tức là trục y
            var periodCount = src.TimetableFlag.GetLength(1);
            //tạo ra instance mới 
            var timetableFlag = new ETimetableFlag[classCount, periodCount];
            // chép data qua mảng rỗng 
            for (var i = 0; i < classCount; i++)
                for (var j = 0; j < periodCount; j++)
                    timetableFlag[i, j] = src.TimetableFlag[i, j];
            // tạo instance mới 
            var timetableUnits = new List<ClassPeriodScheduleModel>();
            //chép data qua 
            for (var i = 0; i < src.TimetableUnits.Count; i++)
                //loop qua ds các phần tử trong mảng và dùng cú pháp with {} trong c# để tạo ra 1 instance mới nhưng vẫn giữ nguyên data
                timetableUnits.Add(src.TimetableUnits[i] with { });
            // trả về cá thể mới với tuổi = 1 và tuổi thọ ngẫu nhiên từ 1 đến 5
            return new TimetableIndividual(timetableFlag, timetableUnits, src.Classes, src.Teachers, src.DoubleSubjects) { Age = 1, Longevity = _random.Next(1, 5) };
        }
        #endregion

        #region RandomlyAssign thắng
        /*
         RandomlyAssign() có nhiệm vụ phân bổ ngẫu nhiên các tiết học (timetable units) vào các vị trí trống trong thời khóa biểu cho mỗi lớp học. 
         Điều này giúp tạo ra sự đa dạng trong quần thể ban đầu bằng cách sắp xếp các tiết học một cách ngẫu nhiên.
         Phương thức này sẽ tìm các tiết trống trong thời khóa biểu (Unfilled) và sau đó gán ngẫu nhiên các tiết học vào những vị trí này.
         Shuffle(): Hàm ngẫu nhiên hóa thứ tự của danh sách tiết học và vị trí trống, đảm bảo rằng các tiết học sẽ được gán một cách ngẫu nhiên.
         */
        private async void RandomlyAssign(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            for (var i = 0; i < src.TimetableFlag.GetLength(0); i++)
            {
                // danh sách chứa tất cả các tiết học chưa được lấp đầy (có cờ trạng thái là Unfilled) của lớp hiện tại
                var morningStartAts = new List<int>();
                var afternoonStartAts = new List<int>();
                for (var j = 1; j < src.TimetableFlag.GetLength(1); j++)
                    if (src.TimetableFlag[i, j] == ETimetableFlag.Unfilled)
                    {
                        if (IsMorningSlot(j))
                        {
                            morningStartAts.Add(j);
                        }
                        else afternoonStartAts.Add(j);
                    }

                /*mục tiêu: tìm các cặp tiết liên tiếp từ danh sách startAts
                consecs: danh sách các cặp tiết liên tiếp as 
                quan trọng để đảm bảo các tiết đôi được xếp vào các vị trí liên tiếp nhau*/

                var morningConsecs = new List<(int, int)>();
                var afternoonConsecs = new List<(int, int)>();

                for (var index = 0; index < morningStartAts.Count - 1; index++)
                {
                    if (morningStartAts[index + 1] - morningStartAts[index] == 1)
                    {
                        morningConsecs.Add((morningStartAts[index], morningStartAts[index + 1]));
                    }
                }

                for (var index = 0; index < afternoonStartAts.Count - 1; index++)
                {
                    if (afternoonStartAts[index + 1] - afternoonStartAts[index] == 1)
                    {
                        afternoonConsecs.Add((afternoonStartAts[index], afternoonStartAts[index + 1]));
                    }
                }

                // rãi các tiết đôi vàoo các slot liên tiếp
                var fClass = src.Classes[i];
                var mainSession = fClass.MainSession;
                for (var j = 0; j < src.DoubleSubjects.Count; j++)
                {
                    var mainNumPeriod = TakeNumberDoubleSlot(src.DoubleSubjects[j].MainSlotPerWeek);
                    var subNumPeriod = TakeNumberDoubleSlot(src.DoubleSubjects[j].SubSlotPerWeek);

                    var mainPeriods = src.TimetableUnits
                                .Where(u => u.ClassId == fClass.Id &&
                                            u.SubjectId == src.DoubleSubjects[j].SubjectId &&
                                            u.Priority == EPriority.Double && (int)u.Session == mainSession)
                                .Take(mainNumPeriod)
                                .ToList();

                    var subPeriods = src.TimetableUnits
                       .Where(u => u.ClassId == fClass.Id &&
                                   u.SubjectId == src.DoubleSubjects[j].SubjectId &&
                                   u.Priority == EPriority.Double && (int)u.Session != mainSession)
                       .Take(subNumPeriod)
                       .ToList();

                    if (fClass.IsFullDay)
                    {

                        if (fClass.MainSession == (int)MainSession.Morning)
                        {
                            RandomConsec(morningConsecs, morningStartAts, i, mainPeriods, src);
                            RandomConsec(afternoonConsecs, afternoonStartAts, i, subPeriods, src);
                        }
                        else
                        {
                            RandomConsec(morningConsecs, morningStartAts, i, subPeriods, src);
                            RandomConsec(afternoonConsecs, afternoonStartAts, i, mainPeriods, src);
                        }
                    }
                    else
                    {
                        if (fClass.MainSession == (int)MainSession.Morning)
                        {
                            RandomConsec(morningConsecs, morningStartAts, i, mainPeriods, src);
                        }
                        else
                        {
                            RandomConsec(afternoonConsecs, afternoonStartAts, i, mainPeriods, src);
                        }
                    }
                }

                // rải các tiết đơn còn lại 
                var morningPeriods = src.TimetableUnits
                   .Where(u => u.ClassId == src.Classes[i].Id && u.StartAt == 0 && u.Session == MainSession.Morning)
                   .Shuffle()
                   .ToList();
                var afternoonPeriods = src.TimetableUnits
                   .Where(u => u.ClassId == src.Classes[i].Id && u.StartAt == 0 && u.Session == MainSession.Afternoon)
                   .Shuffle()
                   .ToList();

                if (morningStartAts.Count < morningPeriods.Count) throw new DefaultException("Số lượng tiết trống buổi sáng trong tuần không đủ xếp tiết học! tối đa 30 tiết / buổi / tuần (bao gồm cả tiết không xếp và tiết xếp sẵn)!"); // số lượng tiết khả dụng không đủ chỗ để xếp tiết học 
                if (afternoonStartAts.Count < afternoonPeriods.Count) throw new DefaultException("Số lượng tiết trống buổi sáng trong tuần không đủ xếp tiết học! tối đa 30 tiết / buổi / tuần (bao gồm cả tiết không xếp và tiết xếp sẵn)!");                                                                       // dải ngẫu nhiên assignment vào các tiết 
                for (var j = 0; j < morningPeriods.Count; j++)
                    morningPeriods[j].StartAt = morningStartAts[j];
                for (var j = 0; j < afternoonPeriods.Count; j++)
                    afternoonPeriods[j].StartAt = afternoonStartAts[j];
            }
        }

        private void RandomConsec(List<(int, int)> consecs, List<int> startAts, int i, List<ClassPeriodScheduleModel> periods, TimetableIndividual src)
        {
            for (var j = 0; j < periods.Count; j++)
            {
                if (j % 2 != 0) continue;

                var randConsecIndex = consecs.IndexOf(consecs.Shuffle().First());// index của các cặp sau khi shuffle
                periods[j].StartAt = consecs[randConsecIndex].Item1;
                periods[j + 1].StartAt = consecs[randConsecIndex].Item2;
                src.TimetableFlag[i, periods[j].StartAt] = ETimetableFlag.Filled;// update slot đó đã được filled
                src.TimetableFlag[i, periods[j + 1].StartAt] = ETimetableFlag.Filled;//update slot thứ 2 trong tiết đôi đã được filled

                if (randConsecIndex > 0 && randConsecIndex < consecs.Count - 1)
                    consecs.RemoveRange(randConsecIndex - 1, 3);
                else if (randConsecIndex == consecs.Count - 1)
                    consecs.RemoveRange(randConsecIndex - 1, 2);
                else
                    consecs.RemoveRange(randConsecIndex, 2);
                startAts.Remove(periods[j].StartAt);
                startAts.Remove(periods[j+1].StartAt);
            }
        }

        #endregion

        #region Fitness Function - long
        /*
         * CalculateAdaptability có nhiệm vụ tính toán độ thích nghi của một cá thể thời khóa biểu dựa trên các tiêu chí và ràng buộc nhất định
         * isMinimized = true => chế độ tối thiểu hóa, = false chế độ tối ưu hóa tức tính toán cả soft constraint 
         */
        private static void CalculateAdaptability(TimetableIndividual src, GenerateTimetableModel parameters, bool isMinimized = false)
        {
            //var rate = isMinimized ? 0 : 1; //được đặt là 0 nếu bài toán tối thiểu hóa, và 1 nếu không
            //Mỗi cá thể có thể có danh sách các lỗi vi phạm ràng buộc (constraint errors).
            //Trước khi tính toán lại điểm thích nghi, hệ thống cần xóa tất cả các lỗi cũ.
            src.TimetableUnits.ForEach(u => u.ConstraintErrors.Clear());//Xóa các lỗi vi phạm cũ của từng tiết học (timetable unit).
            src.ConstraintErrors.Clear();//Xóa các lỗi vi phạm cũ của toàn bộ thời khóa biểu.
            src.Adaptability =
                CheckHC01(src, parameters) * 10000
                + CheckHC02(src) * 1000
                + CheckHC03(src, parameters) * 1000
                + CheckHC05(src) * 10000
                + CheckHC07(src, parameters) * 1000
                + CheckHC08(src) * 1000
                + CheckHC09(src, parameters) * 1000;

            if (!isMinimized)
            {
                src.Adaptability +=
                 CheckSC01(src)
                + CheckSC02(src)
                + CheckSC03(src)
                + CheckSC04(src)
                + CheckSC07(src)
                + CheckSC10(src);
            }
            src.GetConstraintErrors();
        }
        #region CheckHC01
        /*
         * HC01: Ràng buộc đụng độ phòng học
         * Mỗi phòng học chỉ được xếp một phân công trong cùng một thời điểm.
         */
        private static int CheckHC01(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            // đếm số lượng vi phạm
            var count = 0;

            foreach (RoomSubjectScheduleModel roomSubject in parameters.PracticeRoomWithSubjects)
            {
                // lấy ra các tiết học có môn thực hành 
                var timetableUnits = src.TimetableUnits
                    .Where(u => roomSubject.TeachableSubjectIds.Contains(u.SubjectId ?? 0))
                    .ToList();
                // nhóm lại theo tiết học 
                var groups = timetableUnits.GroupBy(u => u.StartAt).ToList();

                foreach (var group in groups)
                {
                    var units = group.Select(g => g).ToList();
                    if (units.Count > roomSubject.MaxClassPerTime)
                    {
                        var (day, period) = GetDayAndPeriod(group.Key);
                        units.ForEach(u => u.ConstraintErrors.Add(new ConstraintErrorModel()
                        {
                            Code = "HC01",
                            ClassName = u.ClassName,
                            SubjectName = u.SubjectName,
                            Description = $"Đụng độ phòng thực hành môn {u.SubjectName} tại tiết {period} vào thứ {day}"
                        }));
                        count += units.Count;
                    }
                }
            }
            return count;
        }

        #endregion

        #region CheckHC02
        
        /*
         * HC02: Ràng buộc đụng độ giáo viên
         * Các phân công khác nhau ứng với các lớp học khác nhau của cùng một giáo viên,
         * không được xếp vào cùng một khung giờ học trong cùng một ngày.
         */
        private static int CheckHC02(TimetableIndividual src)
        {
            // số lượng vi phạm 
            var count = 0;
            for (var i = 0; i < src.Teachers.Count; i++)
            {
                // lấy ra số tiết trong tkb có giáo viên đó 
                var timetableUnits = src.TimetableUnits.Where(u => u.TeacherId == src.Teachers[i].Id).ToList();
                var errorMessage = "";

                for (var j = 0; j < timetableUnits.Count; j++)
                    // kiểm tra đụng độ 
                    if (timetableUnits.Count(u => u.StartAt == timetableUnits[j].StartAt) > 1)
                    {
                        var units = timetableUnits
                            .Where(u => u.StartAt == timetableUnits[j].StartAt)
                            .OrderBy(u => u.SubjectName)
                            .ToList();
                        var (day, period) = GetDayAndPeriod(timetableUnits[j].StartAt);
                        errorMessage =
                            $"Giáo viên {timetableUnits[j].TeacherAbbreviation}: " +
                            $"dạy môn {string.Join(", ", units.Select(u => u.SubjectName))} " +
                            $"cùng lúc tại tiết {period} vào thứ {day}";
                        var error = new ConstraintErrorModel()
                        {
                            Code = "HC02",
                            TeacherName = timetableUnits[j].TeacherAbbreviation,
                            SubjectName = timetableUnits[j].SubjectName,
                            Description = errorMessage
                        };
                        timetableUnits[j].ConstraintErrors.Add(error);
                        count++;
                    }
            }
            return count;
        }

        #endregion

        #region CheckHC03
        /*
         * HC03: Ràng buộc phân công được xếp sẵn
         * Các phân công được xếp sẵn (được khóa) vào một vị trí tiết học thì được ưu tiên xếp đầu tiên 
         * và các phân công khác cần phải tránh xếp vào vị trí tiết học này.(SHL, SHDC)
         */
        private static int CheckHC03(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            //số lượng vi phạm
            var count = 0;
            //lấy ra ds tiết cố định
            var fixedPeriods = src.TimetableUnits.Where(u => u.Priority == EPriority.Fixed).ToList();
            // tiết cố định không khớp vs tham số
            if (fixedPeriods.Count != parameters.FixedPeriodsPara.Count) throw new DefaultException("Lỗi: HC03 - Số lượng tiết cố định tkb không khớp với tham số!");

            // loop qua các tiết cố định kiểm tra nó có xếp đúng vị trí hay không 
            for (var i = 0; i < fixedPeriods.Count; i++)
                if (!parameters.FixedPeriods
                    .Any(u => u.ClassId == fixedPeriods[i].ClassId && u.SubjectId == fixedPeriods[i].SubjectId && u.StartAt == fixedPeriods[i].StartAt))
                {
                    var errorMessage =
                        $"Lớp {fixedPeriods[i].ClassName}: " +
                        $"Môn {fixedPeriods[i].SubjectName} " +
                        $"xếp không đúng vị trí được định sẵn";
                    var error = new ConstraintErrorModel()
                    {
                        Code = "HC03",
                        ClassName = fixedPeriods[i].ClassName,
                        SubjectName = fixedPeriods[i].SubjectName,
                        Description = errorMessage
                    };
                    fixedPeriods[i].ConstraintErrors.Add(error);
                    count++;
                }
            return count;
        }
        #endregion

        #region CheckHC05
        /*
         * HC05: Ràng buộc về tiết học liên tiếp (tiết đôi)
         * Các môn học được chỉ định là có tiết đôi phải có một và chỉ một cặp phân công có khung giờ học liên tiếp trong cùng một ngày.
         */
        private static int CheckHC05(TimetableIndividual src)
        {
            // số lượng tiết vi phạm 
            var count = 0;
            for (var classIndex = 0; classIndex < src.Classes.Count; classIndex++)
            {
                // lấy ra các tiết học của lớp đó 
                var classTimetableUnits = src.TimetableUnits.Where(u => u.ClassId == src.Classes[classIndex].Id).ToList();

                // loop qua ds tiết đôi 
                for (var subjectIndex = 0; subjectIndex < src.DoubleSubjects.Count; subjectIndex++)
                {
                    // lấy ra ds tiết tiết đôi của môn có trong class đó 
                    var doublePeriodUnits = classTimetableUnits
                        .Where(u => u.SubjectId == src.DoubleSubjects[subjectIndex].SubjectId &&
                                    u.Priority == EPriority.Double)
                        .OrderBy(u => u.StartAt)
                        .ToList();
                    // nếu k nằm kề nhau
                    if (doublePeriodUnits[0].StartAt != doublePeriodUnits[1].StartAt - 1)
                    {
                        var errorMessage =
                            $"Lớp {doublePeriodUnits[0].ClassName}: " +
                            $"Môn {doublePeriodUnits[0].SubjectName} " +
                            $"chưa được phân công tiết đôi";
                        var error = new ConstraintErrorModel()
                        {
                            Code = "HC05",
                            ClassName = doublePeriodUnits[0].ClassName,
                            SubjectName = doublePeriodUnits[0].SubjectName,
                            Description = errorMessage
                        };
                        doublePeriodUnits[0].ConstraintErrors.Add(error);
                        doublePeriodUnits[1].ConstraintErrors.Add(error);
                        count += 2;
                    }
                }
            }
            return count;
        }
        #endregion

        #region CheckHC07
        /*
         * HC07: Ràng buộc về môn học có số lượng tiết dạy đồng thời
         * Trong cùng một khung giờ học, số tiết dạy của môn học không vượt quá số phòng thực hành có sẵn. 
         */
        private static int CheckHC07(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            var count = 0;

            // Sử dụng dictionary để lưu số lượng tiết học của từng môn tại mỗi thời điểm
            var subjectUsage = new Dictionary<int, Dictionary<int, List<ClassPeriodScheduleModel>>>();

            // Lặp qua tất cả các tiết học
            foreach (var unit in src.TimetableUnits)
            {
                // Lưu tiết học vào subjectUsage để kiểm tra số lượng tiết của môn tại từng thời điểm
                if (!subjectUsage.ContainsKey(unit.StartAt))
                {
                    subjectUsage[unit.StartAt] = new Dictionary<int, List<ClassPeriodScheduleModel>>();
                }

                if (!subjectUsage[unit.StartAt].ContainsKey((int)unit.SubjectId))
                {
                    subjectUsage[unit.StartAt][(int)unit.SubjectId] = new List<ClassPeriodScheduleModel>();
                }

                subjectUsage[unit.StartAt][(int)unit.SubjectId].Add(unit);
            }

            // Kiểm tra số lượng tiết học của từng môn tại từng thời điểm
            foreach (var timeEntry in subjectUsage)
            {
                var time = timeEntry.Key;
                var subjectsAtSameTime = timeEntry.Value;

                foreach (var subjectEntry in subjectsAtSameTime)
                {
                    var subjectId = subjectEntry.Key;
                    var unitsForSubject = subjectEntry.Value;

                    // Tìm tất cả các phòng có thể dạy môn học này và đảm bảo không vượt quá số phòng
                    var availableRooms = parameters.PracticeRoomWithSubjects
                        .Where(room => room.TeachableSubjectIds.Contains(subjectId))
                        .ToList();

                    // Kiểm tra số phòng có sẵn và xem liệu số tiết học của môn có vượt quá số phòng có sẵn hay không
                    foreach (var room in availableRooms)
                    {
                        // Lọc ra các tiết học của môn trong phòng tại cùng thời điểm và kiểm tra MaxClassPerTime
                        var unitsInRoom = unitsForSubject
                            .Where(u => room.TeachableSubjectIds.Contains((int)u.SubjectId))
                            .ToList();

                        if (unitsInRoom.Count > room.MaxClassPerTime)
                        {
                            var (day, period) = GetDayAndPeriod(time);

                            // Thêm lỗi cho tất cả các tiết vi phạm
                            unitsInRoom.ForEach(u =>
                            {
                                u.ConstraintErrors.Add(new ConstraintErrorModel
                                {
                                    Code = "H08",
                                    ClassName = u.ClassName,
                                    SubjectName = u.SubjectName,
                                    Description = $"Phòng {room.Name} vượt quá giới hạn {room.MaxClassPerTime} lớp trong cùng một thời điểm tại tiết {period} vào thứ {day}."
                                });
                            });

                            count += unitsInRoom.Count;
                        }
                    }
                }
            }

            return count;
        }

        #endregion

        #region CheckHC08
        /*
         * HC08: Ràng buộc môn học chỉ học 1 lần trong một buổi 
         * Trong một buổi học, các phân công được xếp phải là các phân công có môn học khác nhau (trừ cặp phân công được chỉ định là tiết đôi)
         */
        private static int CheckHC08(TimetableIndividual src)
        {
            // count num error
            var count = 0;
            for (var classIndex = 0; classIndex < src.Classes.Count; classIndex++)
            {
                var classObj = src.Classes[classIndex];
                // lấy ra các tiết học của lớp đó 
                var classSingleTimetableUnits = src.TimetableUnits
                    .Where(u => u.ClassId == classObj.Id)
                    .OrderBy(u => u.StartAt)
                    .ToList();

                List<MainSession> sessions = classObj.IsFullDay
                ? new List<MainSession> { MainSession.Morning, MainSession.Afternoon }
                : new List<MainSession> { (MainSession)classObj.MainSession };


                for (var day = 2; day <= 7; day++)
                {
                    foreach (MainSession session in sessions)
                    {
                        // slot buổi sáng 1 -> 5, buổi chiều 6 -> 10 
                        var startSlot = session == MainSession.Morning ? 1 : 6;
                        var endSlot = session == MainSession.Morning ? 5 : 10;

                        // lấy các tiết đơn trong buổi 
                        var singlePeriodUnits = classSingleTimetableUnits
                        .Where(u => u.StartAt >= (day - 2) * 10 + startSlot &&
                                    u.StartAt <= (day - 2) * 10 + endSlot &&
                                    u.Priority != EPriority.Double &&
                                    u.Session == session)
                        .ToList();

                        // lấy các tiết đôi trong buổi 
                        var doublePeriodUnits = classSingleTimetableUnits
                            .Where(u => u.StartAt >= (day - 2) * 10 + startSlot &&
                                        u.StartAt <= (day - 2) * 10 + endSlot &&
                                        u.Priority == EPriority.Double &&
                                        u.Session == session)
                            .ToList();

                        // kiểm tra 
                        for (var i = 0; i < singlePeriodUnits.Count; i++)

                            // nếu có tiết đơn > 1 or tiết đơn trùng tiết đôi 
                            if (singlePeriodUnits
                                .Count(u => u.SubjectName == singlePeriodUnits[i].SubjectName) > 1 ||
                                doublePeriodUnits.Select(s => s.SubjectName).Contains(singlePeriodUnits[i].SubjectName))
                            {
                                var errorMessage =
                                $"Lớp {singlePeriodUnits[i].ClassName}: " +
                                $"Môn {singlePeriodUnits[i].SubjectName} " +
                                $"chỉ được học một lần trong một buổi";
                                var error = new ConstraintErrorModel()
                                {
                                    Code = "HC08",
                                    ClassName = singlePeriodUnits[i].ClassName,
                                    SubjectName = singlePeriodUnits[i].SubjectName,
                                    Description = errorMessage
                                };
                                singlePeriodUnits[i].ConstraintErrors.Add(error);
                                count += 1;
                            }
                    }

                }
            }
            return count;
        }

        #endregion

        #region CheckHC09
        /*
         * HC09: Ràng buộc về tiết không xếp
         * Tiết không xếp là một vị trí tiết học được chỉ định cho một giáo viên hoặc một môn học 
         * mà phân công của giáo viên hoặc phân công có môn học đó không được xếp vào vị trí tiết học này.
         */
        private static int CheckHC09(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            var count = 0;
            for (var i = 0; i < parameters.NoAssignTimetablePeriods.Count; i++)
            {
                var param = parameters.NoAssignTimetablePeriods[i];

                // lấy ra tiết k xếp tại vị trí k xếp  
                var unit = src.TimetableUnits
                    .FirstOrDefault(u => (param.TeacherId == null || u.TeacherId == param.TeacherId) &&
                                u.ClassId == param.ClassId &&
                                (param.SubjectId == null || u.SubjectId == param.SubjectId) &&
                                u.StartAt == param.StartAt);
                // nếu có 
                if (unit != null)
                {
                    var (day, period) = GetDayAndPeriod(unit.StartAt);
                    var errorMessage =
                            $"Lớp {unit.ClassName}: " +
                            $"Môn {unit.SubjectName} " +
                            $"không được xếp tại tiết {period} vào thứ {day}";
                    var error = new ConstraintErrorModel()
                    {
                        Code = "HC09",
                        ClassName = unit.ClassName,
                        SubjectName = unit.SubjectName,
                        Description = errorMessage
                    };
                    unit.ConstraintErrors.Add(error);
                    count++;
                }

            }
            return count;
        }
        #endregion

        #region
        public int CheckHC10(TimetableIndividual src)
        {
            var count = 0;

            // Lặp qua tất cả các lớp trong thời khóa biểu
            foreach (var classItem in src.Classes)
            {
                // Lấy tất cả các tiết học của lớp, sắp xếp theo thời gian bắt đầu
                var timetableUnits = src.TimetableUnits
                    .Where(u => u.ClassId == classItem.Id)
                    .OrderBy(u => u.StartAt)
                    .ToList();

                // Nhóm các tiết học theo buổi (sáng hoặc chiều)
                var sessionGroups = timetableUnits
                    .GroupBy(u => u.Session);

                // Duyệt qua các nhóm buổi học để kiểm tra số lượng tiết
                foreach (var sessionGroup in sessionGroups)
                {
                    var subjectGroups = sessionGroup.GroupBy(u => u.SubjectId);

                    // Duyệt qua từng nhóm môn học trong buổi học
                    foreach (var subjectGroup in subjectGroups)
                    {
                        var lessonCount = subjectGroup.Count();
                        var firstSubject = subjectGroup.First();

                        (string slot, string day) = GetDayAndPeriodString(firstSubject.StartAt);

                        // Kiểm tra vi phạm nếu số lượng tiết lớn hơn 2
                        if (lessonCount > 2)
                        {
                            count += AddConstraintErrors(subjectGroup, "HC10", $"Môn {firstSubject.SubjectName} có quá nhiều tiết trong buổi {(firstSubject.Session == MainSession.Morning ? "Sáng" : "Chiều")} vào thứ {day}.");
                        }
                        // Nếu là 2 tiết, kiểm tra xem đó có phải là tiết đôi không
                        else if (lessonCount == 2)
                        {
                            var invalidDoubleLessons = subjectGroup.Where(u => u.Priority != EPriority.Double).ToList();
                            if (invalidDoubleLessons.Any())
                            {
                                count += AddConstraintErrors(subjectGroup, "HC10", $"Môn {firstSubject.SubjectName} có quá nhiều tiết trong buổi {(firstSubject.Session == MainSession.Morning ? "Sáng" : "Chiều")} vào thứ {day}.");
                            }
                        }
                    }
                }
            }

            return count;
        }

        private int AddConstraintErrors(IEnumerable<ClassPeriodScheduleModel> units, string errorCode, string description)
        {
            var count = 0;
            foreach (var unit in units)
            {
                unit.ConstraintErrors.Add(new ConstraintErrorModel
                {
                    Code = errorCode,
                    ClassName = unit.ClassName,
                    SubjectName = unit.SubjectName,
                    Description = description
                });
                count++;
            }
            return count;
        }
        #endregion

        #region CheckSC01
        /*
         * SC01: Ràng buộc về các tiết đôi không có giờ ra chơi xem giữa
         * Cặp phân công được chỉ định là tiết đôi được ưu tiên tránh các cặp tiết học (2, 3) vào buổi sáng và (3, 4) vào buổi chiểu.
         */
        private static int CheckSC01(TimetableIndividual src)
        {
            // đếm số vi phạm 
            var count = 0;

            // lấy ra các tiết đôi 
            var doublePeriods = src.TimetableUnits
                .Where(u => u.Priority == EPriority.Double)
                .OrderBy(u => u.ClassId)
                .ThenBy(u => u.StartAt)
                .ToList();

            // ds start at k hợp lệ 
            var invalidStartAts = new List<int>() { 2, 3, 8, 9 };

            for (var i = 0; i < doublePeriods.Count - 1; i++)
            {
                var current = doublePeriods[i];
                var next = doublePeriods[i + 1];

                // lấy ra tiết học 
                var currentPeriod = GetPeriod(current.StartAt);
                var nextPeriod = GetPeriod(next.StartAt);

                // kiểm tra nếu là 2 tiết liền kề , cùng môn, cùng lớp và là cặp invalid start at 
                if (invalidStartAts.Contains(currentPeriod) &&
                    invalidStartAts.Contains(nextPeriod) &&
                    current.ClassId == next.ClassId &&
                    current.SubjectId == next.SubjectId &&
                    nextPeriod - currentPeriod == 1)
                {
                    current.ConstraintErrors.Add(new()
                    {
                        Code = "SC01",
                        IsHardConstraint = false,
                        TeacherName = current.TeacherAbbreviation,
                        ClassName = current.ClassName,
                        SubjectName = current.SubjectName,
                        Description = $"Lớp {current.ClassName}: " +
                        $"Môn {current.SubjectName} " +
                        $"nên tránh xếp vào các tiết 2,3 buổi sáng và 3,4 buổi chiều",
                    });
                    next.ConstraintErrors.Add(new()
                    {
                        Code = "SC01",
                        IsHardConstraint = false,
                        TeacherName = next.TeacherAbbreviation,
                        ClassName = next.ClassName,
                        SubjectName = next.SubjectName,
                        Description = $"Lớp {next.ClassName}: " +
                        $"Môn {next.SubjectName} " +
                        $"nên tránh xếp vào các tiết 2,3 buổi sáng và 3,4 buổi chiều",
                    });
                    count += 2;
                }
            }

            return count;
        }
        #endregion

        #region CheckSC02
        /*
         * SC02: Ràng buộc về số lượng buổi dạy của giáo viên
         * Xếp các phân công sao cho số buổi dạy của giáo viên là ít nhất.
         */
        private static int CheckSC02(TimetableIndividual src)
        {
            // tổng số buổi tất cả gv phải đi dạy ( số này càng bé càng tốt )
            var count = 0;

            for (var i = 0; i < src.Teachers.Count; i++)
            {
                // ds tiết học của gv đó 
                var teacherPeriods = src.TimetableUnits
                    .Where(u => u.TeacherId == src.Teachers[i].Id)
                    .ToList();
                for (var j = 1; j < 60; j += 10)
                {
                    // nếu gv có tiết dạy 
                    if (teacherPeriods.Any(p => p.StartAt >= j && p.StartAt < j + 10))
                        count++;
                }
            }
            return count;
        }
        #endregion

        #region CheckSC03
        /*
         * SC03: Ràng buộc về tiết trống của giáo viên trong trong một buổi học
         * Hạn chế tối đa tiết trống (gap) của giáo viên trong một buổi học.
         */
        private static int CheckSC03(TimetableIndividual src)
        {
            // đếm số vi phạm  
            var count = 0;
            for (var i = 0; i < src.Teachers.Count; i++)
            {
                // lấy ra ds tiết học của gv đó 
                var teacherPeriods = src.TimetableUnits
                    .Where(u => u.TeacherId == src.Teachers[i].Id)
                    .OrderBy(u => u.StartAt)
                    .ToList();

                // loop qua theo buổi trong tuần 
                for (var j = 1; j < 60; j += 5)
                {
                    // lấy ra ds tiết trong buổi đó 
                    var periods = teacherPeriods
                        .Where(p => p.StartAt < j + 5 && p.StartAt >= j)
                        .OrderBy(p => p.StartAt)
                        .ToList();

                    // kiểm tra tiết học liên tiếp 
                    for (var k = 0; k < periods.Count - 1; k++)
                        if (periods[k].StartAt != periods[k + 1].StartAt - 1)
                            count++;
                }
            }
            return count;
        }
        #endregion

        #region CheckSC04
        /*
         * SC04:  Ràng buộc về thời gian nghỉ giữa hai buổi của giáo viên
         * Đối với giáo viên dạy cả hai buổi trong một ngày, hạn chế việc xếp các phân công vào tiết cuối buổi sáng và tiết đầu buổi chiều.
         */
        private static int CheckSC04(TimetableIndividual src)
        {
            // đếm số vi phạm 
            var count = 0;
            for (var i = 0; i < src.Teachers.Count; i++)
            {
                // lấy ra ds các tiết của gv đó 
                var teacherPeriods = src.TimetableUnits
                    .Where(u => u.TeacherId == src.Teachers[i].Id)
                    .OrderBy(u => u.StartAt)
                    .ToList();

                // loop theo ngày 
                for (var j = 1; j < 60; j += 10)
                {
                    // kiểm tra gv có dạy cả buổi sáng và buổi chiều trong cùng 1 ngày 
                    if (teacherPeriods.Any(p => p.StartAt >= j && p.StartAt < j + 5) &&
                        teacherPeriods.Any(p => p.StartAt >= j + 5 && p.StartAt < j + 10))
                    {
                        // lấy ra ds tiết học có trong ngày 
                        var periods = teacherPeriods
                            .Where(p => p.StartAt < j + 10 && p.StartAt >= j)
                            .ToList();
                        // kiểm tra có tiết nào rơi vào tiết năm hoặc tiết 6 k 
                        if (periods.Any(p => p.StartAt % 10 == 5 && p.StartAt % 10 == 6))
                            count++;
                    }
                }
            }
            return count;
        }
        #endregion

        #region CheckSC07
        /*
         * SC07: Ràng buộc về các tiết cách ngày
         * Trong thời khóa biểu, các phân công có cùng môn học không được xếp vào hai ngày liên tiếp.
         */
        private static int CheckSC07(TimetableIndividual src)
        {
            var count = 0;

            // Lặp qua từng lớp trong thời khóa biểu
            foreach (var classItem in src.Classes)
            {
                // Nhóm các tiết học theo môn học của lớp hiện tại
                var subjectGroups = src.TimetableUnits
                    .Where(u => u.ClassId == classItem.Id)
                    .GroupBy(u => u.SubjectId);

                // Lặp qua từng nhóm môn học
                foreach (var subjectGroup in subjectGroups)
                {
                    // Lấy danh sách các tiết học của môn, sắp xếp theo thứ tự thời gian
                    var timetableUnits = subjectGroup.OrderBy(u => u.StartAt).ToList();

                    // Kiểm tra nếu có các tiết bị xếp vào hai ngày liên tiếp
                    for (var i = 0; i < timetableUnits.Count - 1; i++)
                    {
                        var currentUnit = timetableUnits[i];
                        var nextUnit = timetableUnits[i + 1];

                        // Tính toán ngày từ vị trí tiết học (mỗi ngày có 10 tiết)
                        var currentDay = (currentUnit.StartAt - 1) / 10 + 1;
                        var nextDay = (nextUnit.StartAt - 1) / 10 + 1;

                        if (nextDay == currentDay + 1) // Nếu hai ngày là liên tiếp
                        {
                            count++;

                            currentUnit.ConstraintErrors.Add(new ConstraintErrorModel
                            {
                                Code = "SC07",
                                IsHardConstraint = false,
                                ClassName = currentUnit.ClassName,
                                SubjectName = currentUnit.SubjectName,
                                Description = $"Lớp {currentUnit.ClassName}: " +
                                              $"Môn {currentUnit.SubjectName} bị xếp vào hai ngày liên tiếp."
                            });

                            nextUnit.ConstraintErrors.Add(new ConstraintErrorModel
                            {
                                Code = "SC07",
                                IsHardConstraint = false,
                                ClassName = nextUnit.ClassName,
                                SubjectName = nextUnit.SubjectName,
                                Description = $"Lớp {nextUnit.ClassName}: " +
                                              $"Môn {nextUnit.SubjectName} bị xếp vào hai ngày liên tiếp."
                            });
                        }
                    }
                }
            }

            return count;
        }

        #endregion

        #region CheckSC10
        /*
         * SC10: Ràng buộc về giáo viên có buổi nghỉ trong tuần
         * Mỗi giáo viên có ít nhất một ngày nghỉ hoàn toàn (cả sáng lẫn chiều) trong tuần
         */
        private static int CheckSC10(TimetableIndividual src)
        {
            var count = 0;

            // Lặp qua từng giáo viên  trong thời khóa biểu
            foreach (var teacher in src.Teachers)
            {
                // Mảng đánh dấu xem giáo viên có dạy vào các ngày thứ 2 -> chủ nhật hay không
                var daysTaught = new bool[7];

                // Duyệt qua các tiết học của giáo viên này
                foreach (var unit in src.TimetableUnits.Where(u => u.TeacherId == teacher.Id))
                {
                    // Tính ra ngày (từ thứ 2 -> thứ 7, tương ứng với day từ 0 -> 6)
                    var day = (unit.StartAt - 1) / 10;
                    daysTaught[day] = true;
                }

                // Kiểm tra xem có ít nhất một ngày mà giáo viên không phải dạy
                if (!daysTaught.Contains(false))
                {
                    count++;

                    // Ghi lại lỗi ràng buộc mềm cho tất cả các tiết của giáo viên này
                    foreach (var unit in src.TimetableUnits.Where(u => u.TeacherId == teacher.Id))
                    {
                        unit.ConstraintErrors.Add(new ConstraintErrorModel
                        {
                            Code = "SC10",
                            IsHardConstraint = false,
                            TeacherName = teacher.Abbreviation,
                            Description = $"Giáo viên {teacher.FirstName} {teacher.LastName} không có ngày nghỉ trong tuần."
                        });
                    }
                }
            }

            return count; // Trả về số lượng vi phạm ràng buộc
        }

        #endregion

        #endregion

        #region Crossover Methods
        /*
         *  thực hiện việc lai tạo giữa hai cá thể bố mẹ để tạo ra hai cá thể con 
         *  Mục tiêu là tạo ra các cá thể con có sự kết hợp các đặc điểm tốt từ bố mẹ
         */
        public List<TimetableIndividual> Crossover(
            TimetableRootIndividual root,
            List<TimetableIndividual> parents,
            GenerateTimetableModel parameters)
        {
            //khởi tạo ds cá thể con 
            var children = new List<TimetableIndividual> { Clone(root), Clone(root) };
            children[0] = Clone(root);
            children[1] = Clone(root);

            //sử dụng một kỹ thuật lai tạo nhất định để kết hợp các đặc điểm từ bố mẹ
            switch (CROSSOVER_METHOD)
            {
                //Single-Point Crossover: Trong phương pháp này, một điểm ngẫu nhiên được chọn trong dữ liệu thời khóa biểu (chromosome)
                //và toàn bộ dữ liệu trước điểm đó sẽ được lấy từ bố mẹ thứ nhất, còn dữ liệu sau điểm đó sẽ được lấy từ bố mẹ thứ hai.
                //EChromosomeType.ClassChromosome: Đây là loại nhiễm sắc thể đại diện cho dữ liệu cần lai tạo.
                //Ở đây, nó có thể đại diện cho cấu trúc của thời khóa biểu (thời gian học của các lớp, giáo viên, v.v.).
                case ECrossoverMethod.SinglePoint:
                    SinglePointCrossover(parents, children, EChromosomeType.ClassChromosome);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Sau khi thực hiện lai tạo, cần đánh dấu lại trạng thái của thời khóa biểu cho các cá thể con
            RemarkTimetableFlag(children[0]);
            RemarkTimetableFlag(children[1]);

            // Đây là bước đột biến, nơi một số phần của cá thể con có thể được thay đổi ngẫu nhiên để tăng tính đa dạng cho quần thể
            // và tránh bị mắc kẹt ở các giải pháp tối ưu cục bộ.
            Mutate(children, CHROMOSOME_TYPE, MUTATION_RATE);

            // Tính toán độ thích nghi
            CalculateAdaptability(children[0], parameters, true);
            CalculateAdaptability(children[1], parameters, true);

            return [children[0], children[1]];
        }
        #endregion


        #region SinglePointCrossover
        /*
         * SinglePointCrossover thực hiện lai tạo điểm đơn (Single-Point Crossover) giữa hai cá thể bố mẹ để tạo ra hai cá thể con. 
         * Trong quá trình này, một điểm ngẫu nhiên được chọn trên chuỗi "nhiễm sắc thể" (chromosome), 
         * sau đó các phần trước và sau điểm đó sẽ được trao đổi giữa hai bố mẹ để tạo ra các cá thể con.
         */
        private List<TimetableIndividual> SinglePointCrossover(
            List<TimetableIndividual> parents,
            List<TimetableIndividual> children,
            EChromosomeType chromosomeType)
        {
            // startIndex và endIndex: Hai biến này sẽ xác định phạm vi của các tiết học (Timetable Units) được lai tạo giữa hai bố mẹ.
            var startIndex = 0;
            var endIndex = 0;
            //xác định cách lai tạo
            switch (chromosomeType)
            {
                // nhiễm sắc thể đại diện cho lớp học
                case EChromosomeType.ClassChromosome:
                    SortChromosome(children[0], EChromosomeType.ClassChromosome);
                    SortChromosome(children[1], EChromosomeType.ClassChromosome);
                    SortChromosome(parents[0], EChromosomeType.ClassChromosome);
                    SortChromosome(parents[1], EChromosomeType.ClassChromosome);
                    //Chọn ngẫu nhiên một lớp từ cá thể bố mẹ đầu tiên. Tên của lớp này sẽ được dùng để xác định các tiết học liên quan đến lớp đó
                    var className = parents[0].Classes[_random.Next(parents[0].Classes.Count)].Name;
                    //Vị trí bắt đầu của lớp học được chọn trong danh sách các tiết học
                    startIndex = parents[0].TimetableUnits.IndexOf(parents[0].TimetableUnits.First(u => u.ClassName == className));
                    //Điểm kết thúc là vị trí bắt đầu cộng với số lượng tiết học mà lớp học đó có trong tuần
                    endIndex = startIndex + parents[0].Classes.First(c => c.Name == className).PeriodCount - 1;
                    break;
                case EChromosomeType.TeacherChromosome:
                    throw new NotImplementedException();
                // break;
                default:
                    throw new NotImplementedException();
            }

            //Trao đổi dữ liệu giữa cha mẹ và con
            // var randIndex = /*rand.Next(0, parents[0].TimetableUnits.Count)*/ parents[0].TimetableUnits.Count / 2;

            for (var i = 0; i < parents[0].TimetableUnits.Count; i++)
            {
                //nếu i nằm ngoài khoảng từ startIndex đến endIndex, nghĩa là phần này sẽ được sao chép trực tiếp từ cha mẹ sang con.
                if (i < startIndex || i > endIndex)
                    for (var j = 0; j < children.Count; j++)
                    {
                        children[j].TimetableUnits[i].StartAt = parents[j].TimetableUnits[i].StartAt;
                        children[j].TimetableUnits[i].Priority = parents[j].TimetableUnits[i].Priority;
                    }
                //nếu i nằm trong khoảng startIndex đến endIndex, dữ liệu sẽ được trao đổi giữa hai cha mẹ để tạo ra hai cá thể con
                else
                    for (var j = 0; j < children.Count; j++)
                    {
                        children[j].TimetableUnits[i].StartAt = parents[children.Count - 1 - j].TimetableUnits[i].StartAt;
                        children[j].TimetableUnits[i].Priority = parents[children.Count - 1 - j].TimetableUnits[i].Priority;
                    }
            }

            return children;
        }
        #endregion

        #region Mutation

        private void Mutate(List<TimetableIndividual> individuals, EChromosomeType type, float mutationRate)
        {
            for (var i = 0; i < individuals.Count; i++)
            {
                if (_random.Next(0, 100) > mutationRate * 100)
                    continue;

                var className = "";
                var teacherAbbre = "";
                List<int> randNumList = null!;
                List<ClassPeriodScheduleModel> timetableUnits = null!;

                switch (type)
                {
                    case EChromosomeType.ClassChromosome:
                        /*chọn ngẫu nhiên một lớp học từ cá thể hiện tại
                        lấy danh sách các tiết học của lớp này nhưng không bao gồm các tiết fixed
                        Tạo danh sách ngẫu nhiên randNumList để chọn ngẫu nhiên các tiết học.
                         */
                        className = individuals[i].Classes[_random.Next(0, individuals[i].Classes.Count)].Name;
                        timetableUnits = individuals[i].TimetableUnits
                            .Where(u => u.ClassName == className && u.Priority != EPriority.Fixed).ToList();
                        randNumList = Enumerable.Range(0, timetableUnits.Count).Shuffle().ToList();

                        if (timetableUnits[randNumList[0]].Priority != EPriority.Double)
                            TimeTableUtils.Swap(timetableUnits[randNumList[0]], timetableUnits[randNumList[1]]);
                        else
                        {
                            var doublePeriods = new List<ClassPeriodScheduleModel>()
                            {
                                timetableUnits[randNumList[0]],
                                timetableUnits.First(
                                    u => u.SubjectName == timetableUnits[randNumList[0]].SubjectName &&
                                         u.Priority == timetableUnits[randNumList[0]].Priority)
                            };
                            randNumList.Remove(timetableUnits.IndexOf(doublePeriods[0]));
                            randNumList.Remove(timetableUnits.IndexOf(doublePeriods[1]));

                            doublePeriods = [.. doublePeriods.OrderBy(p => p.StartAt)];

                            var consecs = new List<(int, int)>();
                            randNumList.Sort();
                            for (var index = 0; index < randNumList.Count - 1; index++)
                                if (randNumList[index + 1] - randNumList[index] == 1)
                                    consecs.Add((randNumList[index], randNumList[index + 1]));

                            var randConsecIndex = consecs.IndexOf(consecs.Shuffle().First());

                            TimeTableUtils.Swap(doublePeriods[0], timetableUnits[randConsecIndex]);
                            TimeTableUtils.Swap(doublePeriods[1], timetableUnits[randConsecIndex + 1]);
                        }
                        //for (var j = 0; j < randNumList.Count - rand.Next(1, randNumList.Count - 1); j++)
                        //    Swap(timetableUnits[randNumList[j]], timetableUnits[randNumList[j + 1]]);
                        break;
                    case EChromosomeType.TeacherChromosome:
                        //fix lai ở đây 
                        teacherAbbre = individuals[i].Teachers[_random.Next(0, individuals[i].Teachers.Count)].Abbreviation;
                        timetableUnits = individuals[i].TimetableUnits
                            .Where(u => u.TeacherAbbreviation == teacherAbbre && u.Priority != EPriority.Fixed).ToList();
                        randNumList = Enumerable.Range(0, timetableUnits.Count).Shuffle().ToList();

                        for (var j = 0; j < randNumList.Count - _random.Next(1, randNumList.Count - 1); j++)
                            TimeTableUtils.Swap(timetableUnits[randNumList[j]], timetableUnits[randNumList[j + 1]]);
                        break;
                }


            }
        }
        #endregion

        #region Enhance Solution

        /* Đừng có đổi 1 tiết 1 lần, đổi 1 mớ tiết 1 lần đi */
        private List<TimetableIndividual> TabuSearch(TimetableIndividual src, GenerateTimetableModel parameters, string code)
        {
            var solutions = new List<TimetableIndividual>();
            switch (code)
            {
                case "H11":

                    break;
            }
            return solutions;
        }

        #endregion

        #region Utils

        private static void FixTimetableAfterUpdate(TimetableIndividual src, GenerateTimetableModel parameters)
        {

        }

        private static int TakeNumberDoubleSlot(int count)
        {
            return count % 2 == 0 ? count : count - 1;
        }

        private void ValidateTimetableParameters(GenerateTimetableModel parameters)
        {

        }

        private List<TimetableIndividual> CreateInitialPopulation(
            TimetableRootIndividual root,
            GenerateTimetableModel parameters)
        {
            // tạo ra 1 ds rỗng để lưu các cá thể tkb 
            var timetablePopulation = new List<TimetableIndividual>();
            //lặp qua INITIAL_NUMBER_OF_INDIVIDUALS số cá thể cần tạo 
            int currentId = 1;
            for (var i = 0; i < INITIAL_NUMBER_OF_INDIVIDUALS; i++)
            {
                // sao chép cá thể gốc để tạo ra cá thể mới ( new instance)
                var individual = Clone(root);
                //gán ngẫu nhiên các tiết học, giáo viên và phòng học cho cá thể vừa được sao chép
                RandomlyAssign(individual, parameters);
                individual.Id = currentId++;
                //tính toán độ thích nghi (adaptability) của cá thể đó dựa trên các ràng buộc trong parameters
                CalculateAdaptability(individual, parameters, false);
                //cá thể vừa được tạo ra và tính toán độ thích nghi sẽ được thêm vào danh sách
                timetablePopulation.Add(individual);
            }
            return timetablePopulation;
        }

        private static void SortChromosome(TimetableIndividual src, EChromosomeType type)
        {
            src.TimetableUnits = type switch
            {
                EChromosomeType.ClassChromosome => [.. src.TimetableUnits.OrderBy(u => u.ClassName)],
                EChromosomeType.TeacherChromosome => [.. src.TimetableUnits.OrderBy(u => u.TeacherAbbreviation)],
                _ => throw new NotImplementedException(),
            };
        }

        //cập nhật lại flag dựa trên timetableUnits 
        private static void RemarkTimetableFlag(TimetableIndividual src)
        {
            // ngoại trừ các tiết k xếp thì mark lại là unfill
            for (var i = 0; i < src.Classes.Count; i++)
                for (var j = 1; j < AVAILABLE_SLOT_PER_WEEK; j++)
                    if (src.TimetableFlag[i, j] != ETimetableFlag.None)
                        src.TimetableFlag[i, j] = ETimetableFlag.Unfilled;

            // mark các tiết khác dựa trên timetable unit 
            for (var i = 0; i < src.TimetableUnits.Count; i++)
            {
                var classIndex = src.Classes.IndexOf(src.Classes.First(c => c.Name == src.TimetableUnits[i].ClassName));
                if (src.TimetableUnits[i].Priority == (int)EPriority.Fixed)
                    src.TimetableFlag[classIndex, src.TimetableUnits[i].StartAt] = ETimetableFlag.Fixed;
                else
                    src.TimetableFlag[classIndex, src.TimetableUnits[i].StartAt] = ETimetableFlag.Filled;
            }

        }

        private static (int day, int period) GetDayAndPeriod(int startAt)
        {
            // 1 ngày 10 slot 
            var day = startAt / 10 + 1;
            // 1 buổi 5 tiết 
            var period = (startAt - 1) % 5 + 1;
            return (day, period);
        }

        private static (string day, string period) GetDayAndPeriodString(int startAt)
        {

            var day = startAt / 10;
            var period = (startAt - 1) % 10 + 1;
            return (DAY_OF_WEEKS[day], SLOTS[period]);
        }

        private static int GetPeriod(int startAt)
        {
            return (startAt - 1) % 10 + 1;
        }
        private static bool IsMorningSlot(int startAt)
        {
            return GetPeriod(startAt) <= 5;
        }

        #endregion
        public Task<BaseResponseModel> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> Check(Guid timetableId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> Update(TimetableIndividual timetable)
        {
            throw new NotImplementedException();
        }
    }
}
