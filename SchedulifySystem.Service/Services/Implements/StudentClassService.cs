using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
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
        private const int ROOT = 0;

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

            //check grade
            var existedGrade = await _unitOfWork.ClassGroupRepo
                .GetByIdAsync(id: createStudentClassModel.GradeId ?? 0, filter: g => !g.IsDeleted && g.ParentId == ROOT)
                ?? throw new NotExistsException($"Grade id {createStudentClassModel.GradeId} is not found!");

            //check class
            if (existedClass.FirstOrDefault() == null)
            {
                var newClass = _mapper.Map<StudentClass>(createStudentClassModel);
                await _unitOfWork.StudentClassesRepo.AddAsync(newClass);
                //save to get class id
                await _unitOfWork.SaveChangesAsync();

                //create new class in group
                var classInGroup = new StudentClassInGroup()
                {
                    StudentClassId = newClass.Id,
                    ClassGroupId = existedGrade.Id,
                    CreateDate = DateTime.UtcNow,
                };
                await _unitOfWork.StudentClassInGroupRepo.AddAsync(classInGroup);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = $"Class {className} is created!" };
            }
            throw new AlreadyExistsException($"Class {className} is already existed!");

        }
        #endregion

        #region CreateStudentClasses
        public async Task<BaseResponseModel> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> createStudentClassModels)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"school id {schoolId} is not found!");
            
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var addedClasses = new List<string>();
                    var classInGroups = new List<StudentClassInGroup>();
                    var skippedClasses = new List<string>();

                    foreach (var studentClass in createStudentClassModels)
                    {

                        var className = studentClass.Name.ToUpper();
                        var existedClasses = await _unitOfWork.StudentClassesRepo.GetAsync(filter: sc => !sc.IsDeleted && sc.Name.Equals(className) && sc.SchoolYearId == schoolYearId);
                        var existedClass = existedClasses.FirstOrDefault();
                        if (existedClass != null)
                        {
                            skippedClasses.Add($"Class {className} can not be add due to existed!");
                            continue;
                        }
                        // check grade is exist
                        var existedGrade = await _unitOfWork.ClassGroupRepo
                            .GetByIdAsync(id: studentClass.GradeId ?? 0, filter: g => !g.IsDeleted && g.ParentId == ROOT);
                        if (existedGrade == null)
                        {
                            skippedClasses.Add($"Class {className} can not be add due to grade id {studentClass.GradeId} do not exist!");
                        }
                        else
                        {
                            var newClass = _mapper.Map<StudentClass>(studentClass);
                            newClass.SchoolId = schoolId;
                            newClass.SchoolYearId = schoolYearId;
                            await _unitOfWork.StudentClassesRepo.AddAsync(newClass);
                            await _unitOfWork.SaveChangesAsync();
                            var classInGroup = new StudentClassInGroup()
                            {
                                StudentClassId = newClass.Id,
                                ClassGroupId = (int)studentClass.GradeId,
                                CreateDate = DateTime.UtcNow
                            };
                            classInGroups.Add(classInGroup);
                            addedClasses.Add($"Class {className} is added!");
                        }
                    }

                    await _unitOfWork.StudentClassInGroupRepo.AddRangeAsync(classInGroups);
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
            var existedClass = await _unitOfWork.StudentClassInGroupRepo.GetByIdAsync(id, include: query => query.Include(stig => stig.ClassGroup).Include(stig => stig.StudentClass).Include(stig => stig.StudentClass.Teacher)) ?? throw new NotExistsException($"class-group id {id} is not found!");
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get class success!", Result = _mapper.Map<StudentClassViewModel>(existedClass) };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException("School is not found!");
            var studentClasses = await _unitOfWork.StudentClassInGroupRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: stig => stig.StudentClass.SchoolId == schoolId && (includeDeleted ? true : stig.IsDeleted == false) && (stig.ClassGroup.ParentId == ROOT) && (schoolYearId == null ? true : stig.StudentClass.SchoolYearId == schoolYearId),
                include: query => query.Include(stig => stig.ClassGroup).Include(stig => stig.StudentClass).Include(stig => stig.StudentClass.Teacher));
            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get student classes success!", Result = studentClassesViewModel };

        }
        #endregion

        #region DeleteStudentClass
        public async Task<BaseResponseModel> DeleteStudentClass(int id)
        {
            var existedClass = await _unitOfWork.StudentClassInGroupRepo.GetByIdAsync(id, include: query => query.Include(c => c.StudentClass)) ?? throw new NotExistsException($"class-group id {id} is not found!");
            existedClass.IsDeleted = true;
            existedClass.StudentClass.IsDeleted = true;
            _unitOfWork.StudentClassInGroupRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Class delete success!" };
        }


        #endregion

        #region UpdateStudentClass
        public async Task<BaseResponseModel> UpdateStudentClass(int id, UpdateStudentClassModel updateStudentClassModel)
        {
            var existedClass = await _unitOfWork.StudentClassInGroupRepo.GetByIdAsync(id, include: query => query.Include(c => c.StudentClass).Include(c => c.ClassGroup)) ?? throw new NotExistsException($"class-group id {id} is not found!");
             var existedGroup = await _unitOfWork.ClassGroupRepo
                            .GetByIdAsync(id: updateStudentClassModel.GradeId ?? 0, filter: g => !g.IsDeleted && g.ParentId == ROOT)
                            ?? throw new NotExistsException($"Grade id {updateStudentClassModel.GradeId} is not found!");
            _mapper.Map(updateStudentClassModel, existedClass);
            _unitOfWork.StudentClassInGroupRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = "Update class success!" };
        }
        #endregion
    }
}
