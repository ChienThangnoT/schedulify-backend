using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
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
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class TeacherService : ITeacherService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public TeacherService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }



        #region CreateTeacher
        public async Task<BaseResponseModel> CreateTeacher(CreateTeacherModel createTeacherRequestModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedTeacher = await _unitOfWork.TeacherRepo.GetAsync(filter: t => t.Email == createTeacherRequestModel.Email);
                    if (existedTeacher.FirstOrDefault() != null)
                    {
                        return new BaseResponseModel() { Status = StatusCodes.Status409Conflict, Message = $"Email {createTeacherRequestModel.Email} is existed!" };
                    }

                    // Handle abbreviation
                    var baseAbbreviation = createTeacherRequestModel.Abbreviation.ToLower();

                    // Get existing teachers with similar abbreviations in the same school
                    var existingAbbreviations = await _unitOfWork.TeacherRepo.GetAsync(
                        filter: t => !t.IsDeleted && t.SchoolId == createTeacherRequestModel.SchoolId &&
                                     t.Abbreviation.ToLower().StartsWith(baseAbbreviation)
                    );

                    // Generate a unique abbreviation using AbbreviationUtils
                    createTeacherRequestModel.Abbreviation = AbbreviationUtils.GenerateUniqueAbbreviation(baseAbbreviation, existingAbbreviations.Select(t => t.Abbreviation.ToLower()));

                    var newTeacher = _mapper.Map<Teacher>(createTeacherRequestModel);
                    await _unitOfWork.TeacherRepo.AddAsync(newTeacher);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Add Teacher success" };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = $"Error: {ex.Message}" };
                }
            }

        }
        #endregion


        #region CreateTeachers
        public async Task<BaseResponseModel> CreateTeachers(int schoolId, List<CreateListTeacherModel> models)
        {
            try
            {
                var check = await CheckValidDataAddTeacher(schoolId, models);
                if (check.Status != StatusCodes.Status200OK)
                {
                    return check;
                }

                var teachers = _mapper.Map<List<Teacher>>(models);
                await _unitOfWork.TeacherRepo.AddRangeAsync(teachers);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_TEACHER_SUCCESS };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region check
        public async Task<BaseResponseModel> CheckValidDataAddTeacher(int schoolId, List<CreateListTeacherModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var ValidList = new List<CreateListTeacherModel>();
            var errorList = new List<CreateListTeacherModel>();


            // Retrieve all potential conflicting emails and abbreviations from the database
            var existingTeachers = await _unitOfWork.TeacherRepo.GetAsync(
                filter: t => !t.IsDeleted && t.SchoolId == schoolId &&
                             (models.Select(m => m.Email).Contains(t.Email) ||
                              models.Select(m => m.Abbreviation.ToLower()).Any(a => t.Abbreviation.ToLower().StartsWith(a))));

            // Create a set to track abbreviations that have been assigned during this process
            var assignedAbbreviations = new HashSet<string>(existingTeachers.Select(t => t.Abbreviation.ToLower()));

            // Check department code exist
            foreach (var model in models)
            {

                var department = (await _unitOfWork.DepartmentRepo.GetAsync(filter: d => d.DepartmentCode.ToLower().Equals(model.DepartmentCode.ToLower()))).FirstOrDefault();
                if (department == null)
                {
                    errorList.Add(model);
                }
                else
                {
                    model.DepartmentId = department.Id;
                }

            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.DEPARTMENT_NOT_EXIST, Result = errorList };
            }

            // check subject exist 
            var subjects = (await _unitOfWork.SubjectRepo.GetV2Async(filter: f => !f.IsDeleted)) ?? new List<Subject>();
            var subjectAbreviations = subjects.Select(s => s.Abbreviation.ToLower()).ToHashSet();
            var subjectNotExist = models.Select(g => g.MainSubject.SubjectAbreviation)
                .Where(s => !subjectAbreviations.Contains(s.ToLower())).ToList();

            if (subjectNotExist.Any())
            {
                foreach (var model in models)
                {
                    if (subjectNotExist.Contains(model.MainSubject.SubjectAbreviation.ToLower()))
                    {
                        errorList.Add(model);
                    }
                }

                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.SUBJECT_NOT_EXISTED,
                    Result = new { ValidList = models.Where(m => !errorList.Contains(m)).ToList(), errorList }
                };
            }
            else
            {
                var subjectLookup = subjects.ToDictionary(s => s.Abbreviation.ToLower(), s => s.Id);

                foreach (var model in models)
                {
                    var teachableSubject = new TeachableSubject
                    {
                        CreateDate = DateTime.UtcNow,
                        SubjectId = subjectLookup[model.MainSubject.SubjectAbreviation.ToLower()],
                        Grade = (int)model.MainSubject.Grade,
                        AppropriateLevel = 9
                    };

                    model.TeachableSubjects = new List<TeachableSubject>() { teachableSubject };
                }
            }


            foreach (var model in models)
            {
                // Check for duplicate emails
                var existedTeacher = existingTeachers.FirstOrDefault(t => t.Email == model.Email);

                if (existedTeacher != null)
                {
                    errorList.Add(model);
                }
                model.SchoolId = schoolId;
                // Handle Abbreviation
                var baseAbbreviation = model?.Abbreviation.ToLower();

                // Generate a unique abbreviation by checking both existing abbreviations and ones already assigned in this session
                model.Abbreviation = AbbreviationUtils.GenerateUniqueAbbreviation(baseAbbreviation, assignedAbbreviations);

                // Add the new abbreviation to the set to ensure no duplicates in the current batch
                assignedAbbreviations.Add(model.Abbreviation);

            }

            return errorList.Any()
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.TEACHER_EMAIL_EXISTED,
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

        #region GetTeachers
        public async Task<BaseResponseModel> GetTeachers(int schoolId, bool includeDeleted, int pageIndex, int pageSize)
        {
            _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var teachers = await _unitOfWork.TeacherRepo.ToPaginationIncludeAsync(pageSize: pageSize, pageIndex: pageIndex, filter: t => t.SchoolId == schoolId && (includeDeleted ? true : t.IsDeleted == false),
                include: query => query.Include(t => t.Department).Include(t => t.TeachableSubjects).ThenInclude(ts => ts.Subject));
            var teachersResponse = _mapper.Map<Pagination<TeacherViewModel>>(teachers);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse };
        }
        #endregion

        #region UpdateTeacher
        public async Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherModel updateTeacherRequestModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                var existedTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id, include: query => query.Include(t => t.TeachableSubjects));
                if (existedTeacher == null)
                {
                    return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
                }

                // Chỉ cập nhật các trường không null
                if (!string.IsNullOrEmpty(updateTeacherRequestModel.FirstName))
                {
                    existedTeacher.FirstName = updateTeacherRequestModel.FirstName;
                }
                if (!string.IsNullOrEmpty(updateTeacherRequestModel.LastName))
                {
                    existedTeacher.LastName = updateTeacherRequestModel.LastName;
                }
                if (!string.IsNullOrEmpty(updateTeacherRequestModel.Abbreviation))
                {
                    existedTeacher.Abbreviation = updateTeacherRequestModel.Abbreviation;
                }
                if (!string.IsNullOrEmpty(updateTeacherRequestModel.Email))
                {
                    existedTeacher.Email = updateTeacherRequestModel.Email;
                }
                if (updateTeacherRequestModel.Gender.HasValue)
                {
                    existedTeacher.Gender = (int)updateTeacherRequestModel.Gender;
                }
                if (updateTeacherRequestModel.DepartmentId.HasValue && updateTeacherRequestModel.DepartmentId != 0)
                {
                    var _ = await _unitOfWork.DepartmentRepo.GetByIdAsync((int)updateTeacherRequestModel.DepartmentId)
                        ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);
                    existedTeacher.DepartmentId = updateTeacherRequestModel.DepartmentId.Value;
                }
                if (updateTeacherRequestModel.DateOfBirth.HasValue)
                {
                    existedTeacher.DateOfBirth = updateTeacherRequestModel.DateOfBirth.Value;
                }
                if (updateTeacherRequestModel.SchoolId.HasValue && updateTeacherRequestModel.SchoolId != 0)
                {
                    var _ = await _unitOfWork.SchoolRepo.GetByIdAsync((int)updateTeacherRequestModel.SchoolId)
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
                    existedTeacher.SchoolId = updateTeacherRequestModel.SchoolId.Value;
                }
                if (updateTeacherRequestModel.TeacherRole.HasValue)
                {
                    existedTeacher.TeacherRole = (int)updateTeacherRequestModel.TeacherRole;
                }
                if (updateTeacherRequestModel.Status.HasValue)
                {
                    existedTeacher.Status = (int)updateTeacherRequestModel.Status;
                }
                if (!string.IsNullOrEmpty(updateTeacherRequestModel.Phone))
                {
                    existedTeacher.Phone = updateTeacherRequestModel.Phone;
                }
                if (updateTeacherRequestModel.IsDeleted.HasValue)
                {
                    existedTeacher.IsDeleted = updateTeacherRequestModel.IsDeleted.Value;
                }

                // Check subject
                var subjects = (await _unitOfWork.SubjectRepo.GetV2Async(filter: f => !f.IsDeleted));
                var subjectIds = subjects.Select(s => s.Id).ToList();
                var newTeachableSubjects = new List<TeachableSubject>();


                if (updateTeacherRequestModel.TeachableSubjectIds == null || !updateTeacherRequestModel.TeachableSubjectIds.All(s => subjectIds.Contains(s)))
                {
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.SUBJECT_NOT_EXISTED
                    };
                }

                // Xóa những TeachableSubject không còn nằm trong danh sách SubjectIds và xóa chúng khỏi db
                var subjectsToRemove = existedTeacher.TeachableSubjects
                    .Where(rs => !updateTeacherRequestModel.TeachableSubjectIds.Contains((int)rs.SubjectId))
                    .ToList();

                foreach (var subjectToRemove in subjectsToRemove)
                {
                    _unitOfWork.TeachableSubjectRepo.Remove(subjectToRemove); // Xóa trực tiếp từ repo
                }

                // Thêm các môn học mới vào TeachableSubjects nếu chưa có
                foreach (var item in updateTeacherRequestModel.TeachableSubjectIds)
                {
                    if (!existedTeacher.TeachableSubjects.Any(rs => rs.SubjectId == item))
                    {
                        newTeachableSubjects.Add(new TeachableSubject() { TeacherId = existedTeacher.Id, SubjectId = item, CreateDate = DateTime.UtcNow });
                    }
                }

                // Thêm các TeachableSubject mới vào db
                if (newTeachableSubjects.Any())
                {
                    await _unitOfWork.TeachableSubjectRepo.AddRangeAsync(newTeachableSubjects); // Sử dụng repo để thêm các RoomSubject mới
                }


                _unitOfWork.TeacherRepo.Update(existedTeacher);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_TEACHER_SUCCESS };
            }
        }

        #endregion

        #region GetTeacherById
        public async Task<BaseResponseModel> GetTeacherById(int id)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id, include: query => query.Include(t => t.Department).Include(t => t.TeachableSubjects).ThenInclude(ts => ts.Subject));
                var teachersResponse = _mapper.Map<TeacherViewModel>(teacher);
                return teacher != null ? new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse } :
                    new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion

        #region DeleteTeacher
        public async Task<BaseResponseModel> DeleteTeacher(int id)
        {
            try
            {
                var existedTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id);
                if (existedTeacher == null)
                {
                    return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
                }
                existedTeacher.IsDeleted = true;
                _unitOfWork.TeacherRepo.Update(existedTeacher);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_TEACHER_SUCCESS };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }

        #endregion

        #region AssignTeacherDepartmentHead
        public async Task<BaseResponseModel> AssignTeacherDepartmentHead(int schoolId, List<AssignTeacherDepartmentHeadModel> models)
        {
           using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var teacherIds = models.Select(m => m.TeacherId).ToList();
                    var teacherDbs = await _unitOfWork.TeacherRepo.GetV2Async(
                        filter: t => !t.IsDeleted && t.SchoolId == schoolId && teacherIds.Contains(t.Id),
                        include: query => query.Include(t => t.Department));
                    var teacherDbIds = teacherDbs.Select(t => t.Id);

                    if (!teacherIds.All(t => teacherDbIds.Contains(t)))
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.TEACHER_NOT_EXIST
                        };
                    }

                    var duplicates = models
                                    .GroupBy(m => m.TeacherId)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToList();

                    if (duplicates.Any())
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Một giáo viên chỉ có thể đảm nhiệm một tổ bộ môn!"
                        };
                    }

                    var departmentIds = models.Select(m => m.DepartmentId);
                    var departmentDbs = await _unitOfWork.DepartmentRepo.GetV2Async(
                        filter: d => departmentIds.Contains(d.Id) && !d.IsDeleted && d.SchoolId == schoolId);
                    var departmentDbIds = departmentDbs.Select(d => d.Id);

                    if (!departmentIds.All(d => departmentDbIds.Contains(d)))
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.DEPARTMENT_NOT_EXIST
                        };
                    }

                    var oldTeacherDepartmentHeads = await _unitOfWork.TeacherRepo.GetV2Async(
                        filter: t => !t.IsDeleted && t.SchoolId == schoolId && t.TeacherRole == (int)TeacherRole.TEACHER_DEPARTMENT_HEAD);

                    foreach (var model in models)
                    {
                        var teacher = teacherDbs.First(t => t.Id == model.TeacherId);
                        var department = departmentDbs.First(d => d.Id == model.DepartmentId);

                        if (teacher.DepartmentId != department.Id)
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = $"Giáo viên {teacher.FirstName} {teacher.LastName} thuộc tổ {teacher.Department.Name} không thể đảm nhiệm tổ trưởng tổ {department.Name}"
                            };
                        }
                        var oldTeacherDepartmentHeadFound = oldTeacherDepartmentHeads.Where(t => t.DepartmentId == department.Id);

                        if (oldTeacherDepartmentHeadFound.Any())
                        {
                            foreach (var item in oldTeacherDepartmentHeadFound)
                            {
                                item.TeacherRole = (int)TeacherRole.TEACHER;
                                _unitOfWork.TeacherRepo.Update(item);
                            }
                        }

                        teacher.TeacherRole = (int) TeacherRole.TEACHER_DEPARTMENT_HEAD;
                        _unitOfWork.TeacherRepo.Update(teacher);

                    }

                    await _unitOfWork.SaveChangesAsync();
                    transaction.Commit();
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Phân công tổ trưởng thành công!"
                    };
                }
                catch (Exception )
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion
    }
}
