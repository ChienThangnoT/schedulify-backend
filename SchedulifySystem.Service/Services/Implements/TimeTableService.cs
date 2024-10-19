using AutoMapper;
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
        public Task<BaseResponseModel> Generate(GenerateTimetableModel paraModel)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetData -- Thắng
        private async Task<(
            List<ClassScheduleModel>,
            List<TeacherScheduleModel>,
            List<SubjectScheduleModel>,
            List<TeacherAssigmentScheduleModel>,
            ETimetableFlag[,]
            )> GetData(GenerateTimetableModel parameters)
        {
            // Khởi tạo các biến
            var classes = new List<ClassScheduleModel>();
            var teachers = new List<TeacherScheduleModel>();
            var subjects = new List<SubjectScheduleModel>();
            var assignments = new List<TeacherAssigmentScheduleModel>();
            var timetableUnits = new List<ClassPeriodScheduleModel>();

            ETimetableFlag[,] timetableFlags = null!;

            // lấy danh sách lớp học từ db  
            //var classesDb = _context.Classes
            //        .Where(c => parameters.ClassIds.Contains(c.Id) &&
            //                    c.StartYear == parameters.StartYear &&
            //                    c.EndYear == parameters.EndYear &&
            //                    c.IsDeleted == false)
            //        .Include(c => c.SubjectClasses.Where(@class => @class.IsDeleted == false))
            //            .ThenInclude(sc => sc.Subject)
            //        .OrderBy(c => c.Name)
            //        .AsNoTracking()
            //        .ToList()
            //        ?? throw new Exception();

            var classesDb = await _unitOfWork.StudentClassesRepo.GetAsync(
                filter: t => t.SchoolId == parameters.SchoolId &&
                             t.SchoolYearId == parameters.SchoolYearId &&
                             t.IsDeleted == false,
                orderBy: q => q.OrderBy(s => s.Name));
            if (classesDb == null || !classesDb.Any())
            {
                throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            }

            var classesDbList = classesDb.ToList();

            // add vào classes
            /*Tạo đối tượng ClassTCDTO cho từng lớp học từ dữ liệu lấy được và thêm vào danh sách classes
              Nếu số lượng lớp học trong danh sách không khớp với số lớp học yêu cầu từ tham số, phương thức sẽ ném ngoại lệ.
            */
            for (var i = 0; i < classesDbList.Count; i++)
                classes.Add(new ClassScheduleModel(classesDbList[i]));
            //if (classes.Count != parameters.ClassIds.Count)
            //    throw new Exception();
            /*Khởi tạo mảng hai chiều timetableFlags với số dòng là số lớp học và số cột là 61.
              Số lượng 61 có thể đại diện cho số tiết học trong một kỳ hoặc một tuần học
            */
            timetableFlags = new ETimetableFlag[classes.Count, 61];

            var subjectsDb = await _unitOfWork.SubjectRepo.GetAsync(
                filter: t => t.SchoolId == parameters.SchoolId &&
                            t.IsDeleted == null);

            var subjectsDbList = subjectsDb.ToList();

            for (var i = 0; i < subjectsDbList.Count; i++)
                subjects.Add(new SubjectScheduleModel(subjectsDbList[i]));

            //var assignmentsDb = _context.Assignments
            //    .Where(a => a.StartYear == parameters.StartYear &&
            //                a.EndYear == parameters.EndYear &&
            //                a.Semester == parameters.Semester &&
            //                a.IsDeleted == false &&
            //                classesDb.Select(c => c.Id).Contains(a.ClassId))
            //    .AsNoTracking()
            //    .ToList()
            //    ?? throw new Exception();

            var assignmentsDb = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                filter: t => t.StudentClassId == classesDb.First().Id && t.IsDeleted == false
                             && (parameters.TermId == null || t.TermId == parameters.TermId));
            var assignmentsDbList = assignmentsDb.ToList();
            //get teacher từ assigntmment db
            var teacherIds = assignmentsDb.Select(a => a.TeacherId).Distinct().ToList();
            //var teachersDb = _context.Teachers
            //    .Where(t => teacherIds.Contains(t.Id) && t.IsDeleted == false)
            //    .OrderBy(c => c.LastName)
            //    .AsNoTracking()
            //    .ToList()
            //    ?? throw new Exception();

            var teachersDb = await _unitOfWork.TeacherRepo.GetAsync(
                filter: t => teacherIds.Contains(t.Id) && t.Status == (int)TeacherStatus.HoatDong && t.IsDeleted == false);
            var teachersDbList = teachersDb.ToList();
            for (var i = 0; i < teachersDbList.Count; i++)
                teachers.Add(new TeacherScheduleModel(teachersDbList[i]));



            // tạo danh sách các assignment
            /*Duyệt qua danh sách các phân công (assignmentsDb), tìm lớp học, môn học, và giáo viên tương ứng cho từng phân công.
             Tạo đối tượng AssignmentTCDTO và thêm vào danh sách assignments.
            */
            for (var i = 0; i < assignmentsDbList.Count; i++)
            {
                var studentClass = classes.First(c => c.Id == assignmentsDbList[i].StudentClassId);
                var subject = subjects.First(s => s.Id == assignmentsDbList[i].SubjectId);
                var teacher = teachers.First(t => t.Id == assignmentsDbList[i].TeacherId);
                assignments.Add(new TeacherAssigmentScheduleModel(assignmentsDbList[i], teacher, subject, studentClass));
            }

            // Kiểm tra xem tất cả các lớp đã được phân công đầy đủ hay chưa
            /*Kiểm tra từng lớp học và môn học xem có phân công phù hợp hay không. Với mỗi lớp, kiểm tra rằng:
             Số lượng tiết học của mỗi môn trong phân công có khớp với số tiết học được yêu cầu cho môn đó không (PeriodCount).
             Giáo viên được phân công có hợp lệ không.
            */
            // Kiểm tra xem tất cả các lớp đã được phân công đầy đủ hay chưa
            //for (var i = 0; i < classesDbList.Count; i++)
            //{
            //    var periodCount = 0; // Tổng số tiết học trong lớp
            //    var classPeriodCount = classesDbList[i].PeriodCount; // Số tiết yêu cầu của lớp học

            //    // Duyệt qua từng môn học trong lớp
            //    for (var j = 0; j < classesDbList[i].SubjectClasses.Count; j++)
            //    {
            //        var subjectClass = classesDbList[i].SubjectClasses.ToList()[j];

            //        // Tìm phân công giáo viên cho môn học
            //        var assignment = assignmentsDbList.FirstOrDefault(a =>
            //            a.SubjectId == subjectClass.SubjectId &&
            //            a.StudentClassId == subjectClass.ClassId);

            //        // Kiểm tra xem có phân công hay không, nếu không thì ném ngoại lệ
            //        if (assignment == null)
            //        {
            //            var subjectName = subjects.First(s => s.Id == subjectClass.SubjectId).SubjectName;
            //            throw new Exception($"Lớp {classesDbList[i].Name} chưa được phân công môn {subjectName}.");
            //        }

            //        // Kiểm tra số tiết học có khớp với yêu cầu không
            //        if (assignment.PeriodCount != subjectClass.PeriodCount)
            //        {
            //            throw new Exception($"Số tiết học cho môn {subjects.First(s => s.Id == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} không khớp.");
            //        }

            //        // Kiểm tra xem giáo viên có được phân công không
            //        if (assignment.TeacherId == null || assignment.TeacherId == 0)
            //        {
            //            throw new Exception($"Môn {subjects.First(s => s.Id == subjectClass.SubjectId).SubjectName} của lớp {classesDbList[i].Name} chưa được phân công giáo viên.");
            //        }

            //        // Cộng số tiết của môn vào tổng số tiết của lớp
            //        periodCount += subjectClass.PeriodCount;
            //    }

            //    // Kiểm tra tổng số tiết của lớp
            //    if (periodCount != classPeriodCount)
            //    {
            //        throw new Exception($"Tổng số tiết học cho lớp {classesDbList[i].Name} không khớp với số yêu cầu.");
            //    }
            //}


            return (classes, teachers, subjects, assignments, timetableFlags);
        }
        #endregion

        #region TimetableRootIndividual 
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
           ETimetableFlag[,] timetableFlags,
           List<SubjectScheduleModel> doublePeriodSubjects,
           GenerateTimetableModel parameters)
        {
            for (var i = 0; i < classes.Count; i++)
            {
                // ca sáng thì a sẽ bắt đầu từ 0 (tương ứng tiết 1 trong ngày) còn ca chiều bắt đầu từ 5 (tương ứng tiết 6 trong ngày)
                var a = classes[i].MainSession == (int)MainSession.Morning ? 0 : 5;
                // j sẽ là index cho mỗi ngày, (max một tuần 60 tiết), mỗi vòng tăng 10 tức sang ngày mới
                for (var j = 1; j < 61; j += 10)
                    // trong ngày j đánh dấu lớp i tiết từ a đến a+5 là chưa fill 
                    for (var k = j; k < j + 5; k++)
                        timetableFlags[i, k + a] = ETimetableFlag.Unfilled;

                //Đánh dấu các tiết trong FreeTimetable vs trạng thái none là các tiết k xếp  
                var list = parameters.FreeTimetablePeriods.Where(u => u.Id == classes[i].Id).ToList();
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
                var classIndex = classes.IndexOf(classes.First(c => c.Id == timetableUnits[i].Id));
                var startAt = timetableUnits[i].StartAt;
                timetableFlags[classIndex, startAt] = ETimetableFlag.Fixed;
            }

            /* Thêm các tiết phân công chưa được xếp vào sau */
            for (var i = 0; i < assignments.Count; i++)
            {
                var count = parameters.FixedPeriods.Count(u => u.TeacherAssignmentId == assignments[i].Id);
                //for (var j = 0; j < assignments[i]. - count; j++)
                //    timetableUnits.Add(new TimetableUnitTCDTO(assignments[i]));
            }

            /* Tạo danh sách các tiết đôi */

            for (var i = 0; i < classes.Count; i++)
            {
                //lấy ra ds tiết học của lớp đó trong timetableUnits
                var classTimetableUnits = timetableUnits.Where(u => u.Id == classes[i].Id).ToList();

                for (var j = 0; j < doublePeriodSubjects.Count; j++)
                {
                    //lấy ra ds tiết học có short name = môn tại vị trí j trong tham số môn đôi, take 2 để lấy 2 tiết đầu tiên 
                    var dPeriods = classTimetableUnits
                        .Where(u => doublePeriodSubjects[j].Id == u.Id).Take(2).ToList();

                    //đặt ưu tiên là tiết đôi 
                    for (var k = 0; k < dPeriods.Count; k++)
                    {
                        dPeriods[k].Priority = (int)EPriority.Double;
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

        #region RandomlyAssign
        /*
         RandomlyAssign() có nhiệm vụ phân bổ ngẫu nhiên các tiết học (timetable units) vào các vị trí trống trong thời khóa biểu cho mỗi lớp học. 
         Điều này giúp tạo ra sự đa dạng trong quần thể ban đầu bằng cách sắp xếp các tiết học một cách ngẫu nhiên.
         Phương thức này sẽ tìm các tiết trống trong thời khóa biểu (Unfilled) và sau đó gán ngẫu nhiên các tiết học vào những vị trí này.
         Shuffle(): Hàm ngẫu nhiên hóa thứ tự của danh sách tiết học và vị trí trống, đảm bảo rằng các tiết học sẽ được gán một cách ngẫu nhiên.
         */
        private static void RandomlyAssign(TimetableIndividual src, GenerateTimetableModel parameters)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        #endregion

        #region SinglePointCrossover
        private List<TimetableIndividual> SinglePointCrossover(
            List<TimetableIndividual> parents,
            List<TimetableIndividual> children,
            EChromosomeType chromosomeType)
        {
            throw new NotImplementedException();
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

        private static void RemarkTimetableFlag(TimetableIndividual src)
        {

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
