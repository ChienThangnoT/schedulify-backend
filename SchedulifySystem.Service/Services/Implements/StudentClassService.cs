using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
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
                ?? throw new NotExistsException(ConstantResponse.GRADE_NOT_EXIST);

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
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_CLASS_SUCCESS };
            }
            throw new AlreadyExistsException($"Class {className} is already existed!");

        }
        #endregion

        #region CreateStudentClasses
        public async Task<BaseResponseModel> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            var check = await CheckValidDataAddClasses(schoolId, schoolYearId, models);
            if (check.Status != StatusCodes.Status200OK)
            {
                return check;
            }

            var classes = _mapper.Map<List<StudentClass>>(models);
            await _unitOfWork.StudentClassesRepo.AddRangeAsync(classes);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_CLASS_SUCCESS };
        }
        #endregion

        #region Check
        public async Task<BaseResponseModel> CheckValidDataAddClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var __ = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            var ValidList = new List<CreateListStudentClassModel>();
            var errorList = new List<CreateListStudentClassModel>();

            //check duplicate name in list
            var duplicateNameRooms = models
             .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateNameRooms.Any())
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.CLASS_NAME_DUPLICATED, Result = duplicateNameRooms };
            }


            //check have teacher in db
            foreach (CreateListStudentClassModel model in models)
            {
                var found = await _unitOfWork.TeacherRepo.ToPaginationIncludeAsync(filter: t => t.SchoolId == schoolId && !t.IsDeleted && t.Abbreviation.ToLower().Equals(model.HomeroomTeacherAbbreviation.ToLower()));
                if (!found.Items.Any())
                {
                    errorList.Add(model);
                }
                else
                {
                    model.HomeroomTeacherId = found.Items.FirstOrDefault()?.Id;
                    model.SchoolId = schoolId;
                    model.SchoolYearId = schoolYearId;
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_ABBREVIATION_NOT_EXIST, Result = errorList };
            }

            //check have grade type in db
            foreach (CreateListStudentClassModel model in models)
            {
                var found = await _unitOfWork.ClassGroupRepo.ToPaginationIncludeAsync(filter: cg => cg.SchoolId == schoolId && !cg.IsDeleted && cg.ClassGroupCode.ToLower().Equals(model.GradeCode.ToLower()));
                if (!found.Items.Any())
                {
                    errorList.Add(model);
                }
                else
                {
                    model.GradeId = found.Items.FirstOrDefault()?.Id;
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.GRADE_CODE_NOT_EXIST, Result = errorList };
            }

            //check teacher is assigned other class
            foreach (CreateListStudentClassModel model in models)
            {
                if(await _unitOfWork.StudentClassesRepo.ExistsAsync(filter: c => !c.IsDeleted && c.SchoolId == schoolId && c.HomeroomTeacherId == model.HomeroomTeacherId))
                {
                    errorList.Add(model);
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.HOMEROOM_TEACHER_ASSIGNED, Result = errorList };
            }


            // List of class names to check in the database
            var modelNames = models.Select(m => m.Name.ToLower()).ToList();

            // Check class duplicates in the database
            var foundClass = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(
                filter: sc => sc.SchoolId == schoolId && !sc.IsDeleted &&
                modelNames.Contains(sc.Name.ToLower()));

            errorList = _mapper.Map<List<CreateListStudentClassModel>>(foundClass.Items);
            ValidList = models.Where(m => !errorList.Any(e => e.Name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            return errorList.Any()
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.CLASS_NAME_EXISTED,
                    Result = new { ValidList, errorList }
                }
                : new BaseResponseModel
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Data is valid!",
                    Result = new { ValidList, errorList }
                };
        }
        #endregion

        #region GetStudentClassById
        public async Task<BaseResponseModel> GetStudentClassById(int id)
        {
            var existedClass = await _unitOfWork.StudentClassInGroupRepo.GetByIdAsync(id, include: query => query.Include(stig => stig.ClassGroup).Include(stig => stig.StudentClass).Include(stig => stig.StudentClass.Teacher)) ?? throw new NotExistsException($"class-group id {id} is not found!");
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_CLASS_SUCCESS, Result = _mapper.Map<StudentClassViewModel>(existedClass) };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, int? gradeId, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var studentClasses = await _unitOfWork.StudentClassInGroupRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: stig => stig.StudentClass.SchoolId == schoolId && (includeDeleted ? true : stig.IsDeleted == false) && (gradeId == null ? true : stig.ClassGroup.Id == gradeId) && (stig.ClassGroup.ParentId == ROOT) && (schoolYearId == null ? true : stig.StudentClass.SchoolYearId == schoolYearId),
                include: query => query.Include(stig => stig.ClassGroup).Include(stig => stig.StudentClass).Include(stig => stig.StudentClass.Teacher));
            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_CLASS_SUCCESS, Result = studentClassesViewModel };

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
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_CLASS_SUCCESS };
        }
        #endregion

        #region UpdateStudentClass
        public async Task<BaseResponseModel> UpdateStudentClass(int id, UpdateStudentClassModel updateStudentClassModel)
        {
            var existedClass = await _unitOfWork.StudentClassInGroupRepo.GetByIdAsync(id, include: query => query.Include(c => c.StudentClass).Include(c => c.ClassGroup)) ?? throw new NotExistsException($"class-group id {id} is not found!");
            var existedGroup = await _unitOfWork.ClassGroupRepo
                           .GetByIdAsync(id: updateStudentClassModel.GradeId ?? 0, filter: g => !g.IsDeleted && g.ParentId == ROOT)
                           ?? throw new NotExistsException(ConstantResponse.GRADE_NOT_EXIST);
            _mapper.Map(updateStudentClassModel, existedClass);
            _unitOfWork.StudentClassInGroupRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_CLASS_SUCCESS };
        }

        #endregion

        #region AssignHomeroomTeacherToClasses
        public async Task<BaseResponseModel> AssignHomeroomTeacherToClasses(AssignListStudentClassModel assignListStudentClassModel)
        {
            if (assignListStudentClassModel.HasDuplicateClassId())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.CLASS_ID_DUPLICATED };
            }

            if (assignListStudentClassModel.HasDuplicateTeacherId())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.HOMEROOM_TEACHER_LIMIT, Result = assignListStudentClassModel.GetDuplicateAssigns() };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (AssignStudentClassModel assign in assignListStudentClassModel)
                    {
                        var existClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(assign.ClassId) ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);
                        var _ = await _unitOfWork.TeacherRepo.GetByIdAsync(assign.TeacherId) ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);
                        existClass.HomeroomTeacherId = assign.TeacherId;
                        existClass.UpdateDate = DateTime.UtcNow;
                    }
                    await _unitOfWork.SaveChangesAsync();
                    transaction.Commit();
                    return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Assign success!" };

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion
    }
}
