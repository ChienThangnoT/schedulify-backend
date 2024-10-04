using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Exceptions;
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
    public class StudentClassService : IStudentClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region CreateStudentClass
        public async Task<BaseResponseModel> CreateStudentClass(CreateStudentClassModel createStudentClassModel)
        {
            string className = createStudentClassModel.Name.ToUpper();
            var existedClass = await _unitOfWork.StudentClassesRepo.GetAsync(filter: sc => !sc.IsDeleted && sc.Name.Equals(className) && sc.SchoolYearId == createStudentClassModel.SchoolYearId);
            if (existedClass.FirstOrDefault() == null)
            {
                var newClass = _mapper.Map<StudentClass>(createStudentClassModel);
                await _unitOfWork.StudentClassesRepo.AddAsync(newClass);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = $"Class {className} is created!" };
            }
            throw new AlreadyExistsException($"Class {className} is already existed!");

        }
        #endregion

        #region CreateStudentClasses
        public async Task<BaseResponseModel> CreateStudentClasses(List<CreateStudentClassModel> createStudentClassModels)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var addAbleClasses = new List<StudentClass>();
                    var addedClasses = new List<string>();

                    var skippedClasses = new List<string>();

                    foreach (var studentClass in createStudentClassModels)
                    {
                        var className = studentClass.Name.ToUpper();
                        var existedClasses = await _unitOfWork.StudentClassesRepo.GetAsync(filter: sc => !sc.IsDeleted && sc.Name.Equals(className) && sc.SchoolYearId == studentClass.SchoolYearId);
                        var existedClass = existedClasses.FirstOrDefault();
                        if (existedClass != null)
                        {
                            skippedClasses.Add($"Class {className} can not be add due to existed!");
                            continue;
                        }
                        addedClasses.Add($"Class {className} is added!");
                        addAbleClasses.Add(_mapper.Map<StudentClass>(studentClass));
                    }
                    await _unitOfWork.StudentClassesRepo.AddRangeAsync(addAbleClasses);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Operation completed!",
                        Result = new
                        {
                            AddedClasses = addedClasses,
                            SkippedClasses = skippedClasses
                        }
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region GetStudentClassById
        public async Task<BaseResponseModel> GetStudentClassById(int id)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id) ?? throw new NotExistsException($"Student class id {id} is not found!");
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get class success!", Result = existedClass };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException("School is not found!");
            var studentClasses = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: tc => tc.School.Id == schoolId && (includeDeleted ? true : tc.IsDeleted == false) && (schoolYearId == null ? true : tc.SchoolYearId == schoolYearId),
                include: query => query.Include(tc => tc.Teacher));
            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get student classes success!", Result = studentClassesViewModel };

        }
        #endregion
    }
}
