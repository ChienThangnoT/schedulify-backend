using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
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
using System.Threading.Tasks;

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

        public TimeTableService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Generate
        public async Task<BaseResponseModel> Generate(GenerateTimetableModel paraModel)
        {
            var (classes, teachers, subjects, assignments, timetableFlags) = await GetData(paraModel);

            var result = CreateRootIndividual(classes, teachers, assignments, subjects, timetableFlags, paraModel);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = result };
        }
        #endregion

        #region Convert Data 
        public async void ConvertData(GenerateTimetableModel parameters)
        {
            parameters.NoAssignTimetablePeriods = _mapper.Map<List<ClassPeriodScheduleModel>>(parameters.NoAssignPeriodsPara);
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
                    if (assignment.PeriodCount != (subjectClass.MoringSlotPerWeek + subjectClass.AfternoonSlotPerWeek))
                    {
                        throw new DefaultException($"Số tiết học cho môn {subjects.First(s => s.SubjectId == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} không khớp.");
                    }

                    // kiểm tra xem giáo viên có được phân công không
                    if (assignment.TeacherId == null || assignment.TeacherId == 0)
                    {
                        throw new DefaultException($"Môn {subjects.First(s => s.SubjectId == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} chưa được phân công giáo viên.");
                    }

                    // cộng số tiết của môn vào tổng số tiết của lớp
                    periodCount += (subjectClass.MoringSlotPerWeek + subjectClass.AfternoonSlotPerWeek);
                }

                // kiểm tra tổng số tiết của lớp
                if (periodCount != classPeriodCount)
                {
                    throw new DefaultException($"Tổng số tiết học cho lớp {classesDbList[i].Name} không khớp với số yêu cầu.");
                }
            }

            // update fixed period in para
            List<ClassPeriodScheduleModel> fixedPeriods = new List<ClassPeriodScheduleModel>();
            foreach(FixedPeriodScheduleModel fixedPeriod in parameters.FixedPeriodsPara)
            {
                var founded = assignments.Where(a => a.Subject.SubjectId == fixedPeriod.SubjectId && a.StudentClass.Id == fixedPeriod.ClassId).FirstOrDefault();
                if (founded == null)
                {
                    throw new NotExistsException($"Tiết cố định không hợp lệ!. Môn học id {fixedPeriod.SubjectId} và lớp id {fixedPeriod.ClassId} không có trong bảng phân công.");
                }
                var period = new ClassPeriodScheduleModel(founded);
                period.StartAt = fixedPeriod.StartAt;
            }
            parameters.FixedPeriods = fixedPeriods;
            
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
                int maxSlot = classes[i].IsFullDay ? 5 : 10;
                for (var j = 1; j < AVAILABLE_SLOT_PER_WEEK; j += 10)
                    // trong ngày j đánh dấu tiết khả dụng để xếp 
                    for (var k = j; k < j + maxSlot; k++)
                        timetableFlags[i, k + a] = ETimetableFlag.Unfilled;

                //Đánh dấu các tiết trong FreeTimetable vs trạng thái none là các tiết k xếp  
                var list = parameters.NoAssignPeriodsPara.Where(u => u.ClassId == classes[i].Id).ToList();
                for (var j = 0; j < list.Count; j++)
                    timetableFlags[i, list[j].StartAt] = ETimetableFlag.None;
            }

            /* Tạo danh sách tiết được xếp sẵn trước*/
            var timetableUnits = new List<ClassPeriodScheduleModel>();
            timetableUnits.AddRange(parameters.FixedPeriods);

            /* Đánh dấu các tiết này vào timetableFlags là các tiết cố định và xếp các tiết này vào slot bao nhiêu  */
            for (var i = 0; i < timetableUnits.Count; i++)
            {
                // lấy ra index của class dựa vào class list vs điều kiện là tìm thấy class này trong timetableUnit (các tiết cố định)
                var classIndex = classes.IndexOf(classes.First(c => c.Id == timetableUnits[i].ClassId));
                var startAt = timetableUnits[i].StartAt;
                timetableFlags[classIndex, startAt] = ETimetableFlag.Fixed;
            }

            /* Thêm các tiết phân công chưa được xếp vào sau */
            for (var i = 0; i < assignments.Count; i++)
            {
                // đếm ra số tiết đã phân công theo tiết cố định 
                var count = parameters.FixedPeriods.Count(u => u.SubjectId == assignments[i].Subject.SubjectId && u.ClassId == assignments[i].StudentClass.Id);
                // phân công các tiết còn lại chưa đc xắp cố định vào tkb 
                for (var j = 0; j < assignments[i].PeriodCount - count; j++)
                    timetableUnits.Add(new ClassPeriodScheduleModel(assignments[i]));
            }

            /* Tạo danh sách các tiết đôi */
            var doublePeriodSubjects = subjects.Where(s => s.IsDoublePeriod).ToList();

            for (var i = 0; i < classes.Count; i++)
            {
                //lấy ra ds tiết học của lớp đó trong timetableUnits
                var classTimetableUnits = timetableUnits.Where(u => u.ClassId == classes[i].Id).ToList();

                for (var j = 0; j < doublePeriodSubjects.Count; j++)
                {
                    //lấy ra ds tiết học có short name = môn tại vị trí j trong tham số môn đôi, take 2 để lấy 2 tiết đầu tiên 
                    var dPeriods = classTimetableUnits
                        .Where(u => doublePeriodSubjects[j].SubjectId == u.SubjectId).Take(2).ToList();

                    //đặt ưu tiên là tiết đôi 
                    for (var k = 0; k < dPeriods.Count; k++)
                    {
                        dPeriods[k].Priority = EPriority.Double;
                    }
                }
            }
            //sắp xếp lại danh sách timetableUnits theo thứ tự tên lớp học và tạo ra một danh sách mới với thứ tự đã được sắp xếp
            timetableUnits = [.. timetableUnits.OrderBy(u => u.ClassName)];

            return new TimetableRootIndividual(timetableFlags, timetableUnits, classes, teachers);
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
            return new TimetableIndividual(timetableFlag, timetableUnits, src.Classes, src.Teachers) { Age = 1, Longevity = _random.Next(1, 5) };
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
                var startAts = new List<int>();
                for (var j = 1; j < src.TimetableFlag.GetLength(1); j++)
                    if (src.TimetableFlag[i, j] == ETimetableFlag.Unfilled)
                        startAts.Add(j);
                /*mục tiêu: tìm các cặp tiết liên tiếp từ danh sách startAts
                consecs: danh sách các cặp tiết liên tiếp as 
                quan trọng để đảm bảo các tiết đôi được xếp vào các vị trí liên tiếp nhau*/
                var consecs = new List<(int, int)>();
                for (var index = 0; index < startAts.Count - 1; index++)
                {
                    if (startAts[index + 1] - startAts[index] == 1)
                    {
                        consecs.Add((startAts[index], startAts[index + 1]));
                    }
                }

                var classTask = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: t => t.SchoolId == parameters.SchoolId &&
                             t.SchoolYearId == parameters.SchoolYearId &&
                             t.IsDeleted == false,
                orderBy: q => q.OrderBy(s => s.Name),
                include: query => query.Include(c => c.SubjectGroup)
                           .ThenInclude(sg => sg.SubjectInGroups)
                           .ThenInclude(sb => sb.Subject));
                var classesDbList = classTask.ToList();

                var subjectInClassesDb = classesDbList
                    .Where(c => c.SubjectGroup != null) // Lọc những lớp có SubjectGroup
                    .SelectMany(c => c.SubjectGroup.SubjectInGroups) // Lấy danh sách SubjectInGroup từ SubjectGroup
                    .ToList();

                // rãi các tiết đôi vàoo các slot liên tiếp
                for (var j = 0; j < subjectInClassesDb.Count; j++)
                {

                    var periods = src.TimetableUnits
                        .Where(u => u.ClassName == src.Classes[i].Name &&
                                    u.SubjectAbbreviation == subjectInClassesDb[j].Subject.Abbreviation &&
                                    u.Priority == EPriority.Double)
                        .Take(2) // lấy 2 tiết đầu tiên, vì tiết đôi cần 2 tiết
                        .ToList();

                    var randConsecIndex = consecs.IndexOf(consecs.Shuffle().First());// index của các cặp sau khi shuffle
                    periods[0].StartAt = consecs[randConsecIndex].Item1;
                    periods[1].StartAt = consecs[randConsecIndex].Item2;
                    src.TimetableFlag[i, periods[0].StartAt] = ETimetableFlag.Filled;// update slot đó đã được filled
                    src.TimetableFlag[i, periods[1].StartAt] = ETimetableFlag.Filled;//update slot thứ 2 trong tiết đôi đã được filled

                    if (randConsecIndex > 0 && randConsecIndex < consecs.Count - 1)
                        consecs.RemoveRange(randConsecIndex - 1, 3);
                    else if (randConsecIndex == consecs.Count - 1)
                        consecs.RemoveRange(randConsecIndex - 1, 2);
                    else
                        consecs.RemoveRange(randConsecIndex, 2);
                    startAts.Remove(periods[0].StartAt);
                    startAts.Remove(periods[1].StartAt);
                }

                var timetableUnits = src.TimetableUnits
                    .Where(u => u.ClassName == src.Classes[i].Name && u.StartAt == 0)
                    .Shuffle()
                    .ToList();
                startAts = startAts.Shuffle().ToList();
                if (startAts.Count != timetableUnits.Count) throw new Exception();
                for (var j = 0; j < timetableUnits.Count; j++)
                    timetableUnits[j].StartAt = startAts[j];
            }
        }
        #endregion

        #region Fitness Function
        /*
         * CalculateAdaptability có nhiệm vụ tính toán độ thích nghi của một cá thể thời khóa biểu dựa trên các tiêu chí và ràng buộc nhất định
         * isMinimized = true => chế độ tối thiểu hóa, = false chế độ tối ưu hóa tức tính toán cả soft constraint 
         */
        private static void CalculateAdaptability(TimetableIndividual src, GenerateTimetableModel parameters, bool isMinimized = false)
        {
            throw new NotImplementedException();
        }
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

        /*
         * SinglePointCrossover thực hiện lai tạo điểm đơn (Single-Point Crossover) giữa hai cá thể bố mẹ để tạo ra hai cá thể con. 
         * Trong quá trình này, một điểm ngẫu nhiên được chọn trên chuỗi "nhiễm sắc thể" (chromosome), 
         * sau đó các phần trước và sau điểm đó sẽ được trao đổi giữa hai bố mẹ để tạo ra các cá thể con.
         */
        #region SinglePointCrossover
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
            for (var i = 0; i < INITIAL_NUMBER_OF_INDIVIDUALS; i++)
            {
                // sao chép cá thể gốc để tạo ra cá thể mới ( new instance)
                var individual = Clone(root);
                //gán ngẫu nhiên các tiết học, giáo viên và phòng học cho cá thể vừa được sao chép
                RandomlyAssign(individual, parameters);
                //tính toán độ thích nghi (adaptability) của cá thể đó dựa trên các ràng buộc trong parameters
                CalculateAdaptability(individual, parameters, true);
                //cá thể vừa được tạo ra và tính toán độ thích nghi sẽ được thêm vào danh sách
                timetablePopulation.Add(individual);
            }
            return timetablePopulation;
        }

        private static void SortChromosome(TimetableIndividual src, EChromosomeType type)
        {

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
            var day = startAt / 10 + 1;
            var period = (startAt - 1) % 5 + 1;
            return (day, period);
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
