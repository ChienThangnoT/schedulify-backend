using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectInGroupBusinessModels;
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
                return new BaseResponseModel() 
                { 
                    Status = StatusCodes.Status404NotFound, 
                    Message = ConstantResponse.TEACHER_ABBREVIATION_NOT_EXIST, 
                    Result = errorList 
                };
            }


            //check teacher is assigned other class
            foreach (CreateListStudentClassModel model in models)
            {
                if (await _unitOfWork.StudentClassesRepo.ExistsAsync(
                    filter: c => !c.IsDeleted && c.SchoolId == schoolId && 
                    c.HomeroomTeacherId == model.HomeroomTeacherId)
                    )
                {
                    errorList.Add(model);
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() 
                { 
                    Status = StatusCodes.Status400BadRequest, 
                    Message = ConstantResponse.HOMEROOM_TEACHER_ASSIGNED, 
                    Result = errorList 
                };
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
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id, include: query => query.Include(sc => sc.Teacher).Include(sc => sc.SubjectGroup) ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST));
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_CLASS_SUCCESS, Result = _mapper.Map<StudentClassViewModel>(existedClass) };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, EGrade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var studentClasses = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: sc => sc.SchoolId == schoolId && (includeDeleted ? true : sc.IsDeleted == false) && (grade == null ? true : sc.Grade == (int)grade ) && (schoolYearId == null ? true : sc.SchoolYearId == schoolYearId),
                include: query => query.Include(sc => sc.Teacher).Include(sc => sc.SubjectGroup));
            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_CLASS_SUCCESS, Result = studentClassesViewModel };

        }
        #endregion

        #region DeleteStudentClass
        public async Task<BaseResponseModel> DeleteStudentClass(int id)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id) ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            existedClass.IsDeleted = true;
            _unitOfWork.StudentClassesRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_CLASS_SUCCESS };
        }
        #endregion

        #region UpdateStudentClass
        public async Task<BaseResponseModel> UpdateStudentClass(int id, UpdateStudentClassModel updateStudentClassModel)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id) ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
           
            _mapper.Map(updateStudentClassModel, existedClass);
            _unitOfWork.StudentClassesRepo.Update(existedClass);
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

        #region
        public async Task<BaseResponseModel> GetSubjectInGroupOfClass(int schoolId, int schoolYearId, int studentClassId)
        {
            var classesDb = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: t => t.SchoolId == schoolId &&
                             t.Id == studentClassId &&
                             t.SchoolYearId == schoolYearId &&
                             t.IsDeleted == false,
                orderBy: q => q.OrderBy(s => s.Name),
                include: query => query.Include(c => c.SubjectGroup)
                           .ThenInclude(sg => sg.SubjectInGroups));

            if (classesDb == null || !classesDb.Any())
            {
                throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            }

            var classesDbList = classesDb.ToList();
            var listSBInGroup = new List<SubjectInGroup>();
            for (var i = 0; i < classesDbList.Count; i++)
            {
                for (var j = 0; j < classesDbList[i].SubjectGroup.SubjectInGroups.Count; j++)
                {
                    listSBInGroup.Add(classesDbList[i].SubjectGroup.SubjectInGroups.ToList()[j]);
                }
            }
            var result = _mapper.Map<List<SubjectInGroupViewModel>>(listSBInGroup);


            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_IN_CLASS_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region AssignSubjectGroupToClasses
        public async Task<BaseResponseModel> AssignSubjectGroupToClasses(AssignSubjectGroup model)
        {
            var subjectGroup = await _unitOfWork.SubjectGroupRepo.GetByIdAsync(model.SubjectGroupId)
                                ?? throw new NotExistsException(ConstantResponse.SUBJECT_GROUP_NOT_EXISTED);

            foreach (var classId in model.ClassIds)
            {
                var founded = await _unitOfWork.StudentClassesRepo.GetByIdAsync(classId)
                                ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);

                founded.SubjectGroupId = model.SubjectGroupId;
                founded.UpdateDate = DateTime.UtcNow;
                _unitOfWork.StudentClassesRepo.Update(founded);
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.SUBJECT_GROUP_ASSIGN_SUCCESS
            };
        }

        #endregion

    }
}
