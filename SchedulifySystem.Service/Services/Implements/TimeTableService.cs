using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherAssignmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
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

        #region GetData
        private (
            List<ClassScheduleModel>,
            List<TeacherScheduleModel>,
            List<SubjectScheduleModel>,
            List<TeacherScheduleModel>,
            ETimetableFlag[,]
            ) GetData(GenerateTimetableModel parameters)
        {
            throw new NotImplementedException();
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
           GenerateTimetableModel parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Clone
        /*
        *Tạo một bản sao của cá thể gốc (root) để làm cá thể mới. đơn giản là data giống cá thể cũ nhưng là một instance mới 
        */
        private TimetableIndividual Clone(TimetableIndividual src)
        {
            throw new NotImplementedException();
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
