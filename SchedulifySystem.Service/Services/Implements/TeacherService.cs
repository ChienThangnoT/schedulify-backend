using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using SchedulifySystem.Service.BusinessModels.RoleAssignmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
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
        private readonly IMailService _mailService;
        private readonly int MAX_APPROVIATE_LEVEL = 5;
        private readonly int MIN_APPROVIATE_LEVEL = 1;

        public TeacherService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IMailService mailService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mailService = mailService;
        }

        #region Generate Teacher Account
        public async Task<BaseResponseModel> GenerateTeacherAccount(TeacherGenerateAccount teacherGenerateAccount)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(teacherGenerateAccount.SchoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(teacherGenerateAccount.TeacherId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);

            var teacherAccount = (await _unitOfWork.RoleAssignmentRepo.GetV2Async(filter: t => t.Account.Email == teacher.Email && t.Account.Status == (int)AccountStatus.Active)).FirstOrDefault();
            if (teacherAccount != null)
            {
                var existRole = await _unitOfWork.RoleAssignmentRepo.GetV2Async(
                    filter: t => t.AccountId == teacherAccount.Id && t.Role.Name.ToLower() == RoleEnum.Teacher.ToString().ToLower() && t.IsDeleted == false,
                    include: query => query.Include(u => u.Role));
                if (existRole != null)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.GENERATE_TEACHER_HAS_EXISTED
                    };
                }
            }
            var accountPassword = AuthenticationUtils.GeneratePassword();
            Account account = new()
            {
                Email = teacher.Email,
                Password = AuthenticationUtils.HashPassword(accountPassword),
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                SchoolId = school.Id,
                IsChangeDefaultPassword = false,
                Status = (int)AccountStatus.Active,
                Phone = teacher.Phone,
                AvatarURL = teacher.AvatarURL,
                CreateDate = DateTime.UtcNow,
                IsConfirmSchoolManager = false
            };

            await _unitOfWork.UserRepo.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();
            var role = await _unitOfWork.RoleRepo.GetRoleByNameAsync(RoleEnum.Teacher.ToString());
            if (role == null)
            {
                Role newRole = new()
                {
                    Name = RoleEnum.Teacher.ToString()
                };
                await _unitOfWork.RoleRepo.AddAsync(newRole);
                await _unitOfWork.SaveChangesAsync();
                role = newRole;
            }

            var accountRoleModel = new RoleAssigntmentAddModel
            {
                AccountId = account.Id,
                RoleId = role.Id
            };
            var accountRoleEntyties = _mapper.Map<RoleAssignment>(accountRoleModel);
            await _unitOfWork.RoleAssignmentRepo.AddAsync(accountRoleEntyties);
            await _unitOfWork.SaveChangesAsync();

            account.School = school;
            var result = _mapper.Map<AccountViewModel>(account);

            var messageRequest = new EmailRequest
            {
                To = account.Email,
                Subject = "Tạo tài khoản thành công",
                Content = MailTemplate.SendPasswordTemplate(school.Name, account.LastName, account.Email, accountPassword)
            };
            await _mailService.SendEmailAsync(messageRequest);

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.GENERATE_TEACHER_SUCCESS,
                Result = result
            };
        }
        #endregion

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

            var activeAccountEmails = (await _unitOfWork.UserRepo.GetAsync(
                filter: a => a.Status == (int)AccountStatus.Active)).Select(a => a.Email.ToLower()).ToHashSet();
            var existingTeacherEmails = (await _unitOfWork.TeacherRepo.GetAsync(
                filter: t => !t.IsDeleted && t.SchoolId == schoolId)).Select(t => t.Email.ToLower()).ToHashSet();


            // Create a set to track abbreviations that have been assigned during this process
            var assignedAbbreviations = new HashSet<string>(existingTeachers.Select(t => t.Abbreviation.ToLower()));

            // Check department code exist
            foreach (var model in models)
            {
                if (existingTeacherEmails.Contains(model.Email.ToLower()) || activeAccountEmails.Contains(model.Email.ToLower()))
                {
                    errorList.Add(model);
                    continue;
                }

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
            //var subjectNotExist = models.Select(g => g.MainSubject.SubjectAbreviation)
            //    .Where(s => !subjectAbreviations.Contains(s.ToLower())).ToList();

            //if (subjectNotExist.Any())
            //{
            //    foreach (var model in models)
            //    {
            //        if (subjectNotExist.Contains(model.MainSubject.SubjectAbreviation.ToLower()))
            //        {
            //            errorList.Add(model);
            //        }
            //    }

            //    return new BaseResponseModel()
            //    {
            //        Status = StatusCodes.Status400BadRequest,
            //        Message = ConstantResponse.SUBJECT_NOT_EXISTED,
            //        Result = new { ValidList = models.Where(m => !errorList.Contains(m)).ToList(), errorList }
            //    };
            //}
            //else
            //{
            //    var subjectLookup = subjects.ToDictionary(s => s.Abbreviation.ToLower(), s => s.Id);

            //    foreach (var model in models)
            //    {
            //        var list = new List<TeachableSubject>();
            //        foreach (var grade in model.MainSubject.ListApproriateLevelByGrades)
            //        {
            //            var teachableSubject = new TeachableSubject
            //            {
            //                CreateDate = DateTime.UtcNow,
            //                SubjectId = subjectLookup[model.MainSubject.SubjectAbreviation.ToLower()],
            //                Grade = (int)grade.Grade,
            //                AppropriateLevel = MAX_APPROVIATE_LEVEL,
            //                IsMain = true
            //            };
            //            list.Add(teachableSubject);

            //        }
            //        model.TeachableSubjects = list;


            //    }
            //}


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
        public async Task<BaseResponseModel> GetTeachers(int schoolId, int? departmentId, bool includeDeleted, int pageIndex, int pageSize)
        {
            // Kiểm tra sự tồn tại của trường học
            _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            // Kiểm tra sự tồn tại của phòng ban nếu được cung cấp
            if (departmentId != null)
            {
                var department = await _unitOfWork.DepartmentRepo.GetByIdAsync((int)departmentId) ??
                    throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);
            }

            // Lấy danh sách giáo viên với các môn học và khối phù hợp
            var teachers = await _unitOfWork.TeacherRepo.ToPaginationIncludeAsync(
                pageSize: pageSize,
                pageIndex: pageIndex,
                filter: t => t.SchoolId == schoolId && (includeDeleted || !t.IsDeleted)
                            && (departmentId == null || departmentId == t.DepartmentId),
                include: query => query.Include(q => q.Department).Include(o => o.StudentClasses).Include(t => t.TeachableSubjects).ThenInclude(ts => ts.Subject)
            );

            var teacherViewModels = teachers.Items.Select(teacher => new TeacherViewModel
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Abbreviation = teacher.Abbreviation,
                Email = teacher.Email,
                DepartmentId = teacher.DepartmentId,
                DepartmentName = teacher.Department?.Name,
                DateOfBirth = teacher.DateOfBirth,
                TeacherRole = (TeacherRole)teacher.TeacherRole,
                IsHomeRoomTeacher = teacher.StudentClasses?.Any(a => a.HomeroomTeacherId == teacher.Id && a.IsDeleted == false),
                StudentClassId = teacher.StudentClasses?.Where(a => a.IsDeleted == false).Select(a => (int?)a.Id).FirstOrDefault(),
                HomeRoomTeacherOfClass = teacher.StudentClasses?.Where(a => a.IsDeleted == false).Select(a => a.Name).FirstOrDefault(),
                Gender = (Gender)teacher.Gender,
                Status = teacher.Status,
                IsDeleted = teacher.IsDeleted,
                Phone = teacher.Phone,
                PeriodCount = teacher.PeriodCount,
                TeachableSubjects = teacher.TeachableSubjects
                    .GroupBy(ts => ts.SubjectId)
                    .Select(group => new TeachableSubjectViewModel
                    {
                        SubjectId = group.Key,
                        SubjectName = group.First().Subject?.SubjectName,
                        Abbreviation = group.First().Subject?.Abbreviation,
                        IsMain = group.First().IsMain,
                        ListApproriateLevelByGrades = group.Select(ts => new ListApproriateLevelByGrade
                        {
                            AppropriateLevel = (EAppropriateLevel)ts.AppropriateLevel,
                            Grade = (EGrade)ts.Grade
                        }).ToList()
                    }).ToList()
            }).ToList();

            var teachersResponse = new Pagination<TeacherViewModel>
            {
                Items = teacherViewModels,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemCount = teachers.TotalItemCount
            };

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Result = teachersResponse
            };
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

                _mapper.Map(updateTeacherRequestModel, existedTeacher);

                if (updateTeacherRequestModel.DepartmentId.HasValue && updateTeacherRequestModel.DepartmentId != 0)
                {
                    var _ = await _unitOfWork.DepartmentRepo.GetByIdAsync((int)updateTeacherRequestModel.DepartmentId)
                        ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);
                }

                if (updateTeacherRequestModel.SchoolId.HasValue && updateTeacherRequestModel.SchoolId != 0)
                {
                    var _ = await _unitOfWork.SchoolRepo.GetByIdAsync((int)updateTeacherRequestModel.SchoolId)
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
                }

                _unitOfWork.TeacherRepo.Update(existedTeacher);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_TEACHER_SUCCESS };
            }
        }


        #endregion

        #region Update Teacherable Subeject
        public async Task<BaseResponseModel> UpdateTeachableSubjects(int id, List<SubjectGradeModel> teachableSubjects)
        {
            foreach (var subject in teachableSubjects)
            {
                var duplicateGrades = subject.ListApproriateLevelByGrades
                    .GroupBy(g => g.Grade)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateGrades.Any())
                {
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Subject '{subject.SubjectAbreviation}' contains duplicate grades: {string.Join(", ", duplicateGrades)}"
                    };
                }
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                var existedTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id, include: query => query.Include(t => t.TeachableSubjects));
                if (existedTeacher == null)
                {
                    return new BaseResponseModel { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
                }

                var subjects = await _unitOfWork.SubjectRepo.GetV2Async(filter: f => !f.IsDeleted);
                var subjectAbbreviationDbs = subjects.Select(s => s.Abbreviation.ToLower()).ToList();
                var teachableSubjectAbbreviationPara = teachableSubjects.Select(s => s.SubjectAbreviation.ToLower()).ToList();

                if (!teachableSubjectAbbreviationPara.All(subjectAbbreviationDbs.Contains))
                {
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.SUBJECT_NOT_EXISTED
                    };
                }

                var subjectObjectPara = subjects.Where(s => teachableSubjectAbbreviationPara.Contains(s.Abbreviation.ToLower()));

                // Xóa các bản ghi không còn phù hợp
                var subjectIds = subjectObjectPara.Select(s => s.Id).ToList();
                var gradesToKeep = teachableSubjects.SelectMany(ts => ts.ListApproriateLevelByGrades.Select(g => (int)g.Grade)).Distinct().ToList();

                var subjectsToRemove = existedTeacher.TeachableSubjects
                    .Where(ts => !subjectIds.Contains(ts.SubjectId) || !gradesToKeep.Contains(ts.Grade))
                    .ToList();

                if (subjectsToRemove.Any())
                {
                    _unitOfWork.TeachableSubjectRepo.RemoveRange(subjectsToRemove);
                }

                int countMainSubjects = teachableSubjects.Count(s => s.IsMain);
                if (countMainSubjects > 1)
                {
                    throw new DefaultException(ConstantResponse.INVALID_NUMBER_MAIN_SUBJECR_OF_TEACHER);
                }

                foreach (var item in subjectObjectPara)
                {
                    var model = teachableSubjects.FirstOrDefault(s => s.SubjectAbreviation.ToLower() == item.Abbreviation.ToLower());

                    foreach (var grade in model.ListApproriateLevelByGrades)
                    {
                        var existingSubject = existedTeacher.TeachableSubjects
                            .FirstOrDefault(ts => ts.SubjectId == item.Id && ts.Grade == (int)grade.Grade);

                        if (existingSubject != null)
                        {
                            // Cập nhật nếu cần
                            if (existingSubject.AppropriateLevel != (int)grade.AppropriateLevel || existingSubject.IsMain != model.IsMain)
                            {
                                existingSubject.AppropriateLevel = (int)grade.AppropriateLevel;
                                existingSubject.IsMain = model.IsMain;
                                existingSubject.UpdateDate = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            // Thêm mới
                            existedTeacher.TeachableSubjects.Add(new TeachableSubject
                            {
                                TeacherId = existedTeacher.Id,
                                SubjectId = item.Id,
                                Grade = (int)grade.Grade,
                                AppropriateLevel = (int)grade.AppropriateLevel,
                                IsMain = model.IsMain,
                                CreateDate = DateTime.UtcNow
                            });
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_TEACHER_SUCCESS };
            }
        }

        #endregion

        #region GetTeacherById
        public async Task<BaseResponseModel> GetTeacherById(int id)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id,
                    include: query => query.Include(t => t.Department).Include(t => t.TeachableSubjects).ThenInclude(ts => ts.Subject));

                var teacherViewModels = new TeacherViewModel
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Abbreviation = teacher.Abbreviation,
                    Email = teacher.Email,
                    DepartmentId = teacher.DepartmentId,
                    DepartmentName = teacher.Department?.Name,
                    Gender = (Gender)teacher.Gender,
                    Status = teacher.Status,
                    IsDeleted = teacher.IsDeleted,
                    TeacherRole = (TeacherRole)teacher.TeacherRole,
                    Phone = teacher.Phone,
                    DateOfBirth = teacher.DateOfBirth,
                    PeriodCount = teacher.PeriodCount,
                    TeachableSubjects = teacher.TeachableSubjects
                    .GroupBy(ts => ts.SubjectId)
                    .Select(group => new TeachableSubjectViewModel
                    {
                        SubjectId = group.Key,
                        SubjectName = group.First().Subject?.SubjectName,
                        Abbreviation = group.First().Subject?.Abbreviation,
                        IsMain = group.First().IsMain,
                        ListApproriateLevelByGrades = group.Select(ts => new ListApproriateLevelByGrade
                        {
                            AppropriateLevel = (EAppropriateLevel)ts.AppropriateLevel,
                            Grade = (EGrade)ts.Grade
                        }).ToList()
                    }).ToList()
                };
                return teacher != null ? new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teacherViewModels } :
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

                        teacher.TeacherRole = (int)TeacherRole.TEACHER_DEPARTMENT_HEAD;
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
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region Get Teacher AssignmentDetail
        public async Task<BaseResponseModel> GetTeacherAssignmentDetail(int teacherId, int schoolYearId)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            var terms = await _unitOfWork.TermRepo.GetV2Async(filter: t => t.IsDeleted == false && t.SchoolYearId == schoolYearId)
                ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
            var termList = terms.Select(t => t.Id).ToList();
            var teacherAssignment = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(filter: t => t.TeacherId == teacherId && termList.Contains(t.TermId) && t.IsDeleted == false,
                orderBy: o => o.OrderBy(q => q.TermId),
                include: query => query.Include(t => t.Subject).Include(q => q.Teacher).Include(u => u.Term).Include(p => p.StudentClass)
                );
            if (teacherAssignment == null || !teacherAssignment.Any())
            {
                throw new NotExistsException(ConstantResponse.TEACHER_ASSIGNMENT_NOT_EXIST);
            }

            var assignmentDictionary = teacherAssignment
                .GroupBy(a => a.SubjectId)
                .Select(group => new ViewAssignmentDetail
                {
                    TeacherId = group.Key,
                    TeacherFirstName = group.First().Teacher?.FirstName,
                    TeacherLastName = group.First().Teacher?.LastName,
                    DepartmentId = (int)group.First().Teacher?.DepartmentId,
                    TotalSlotInYear = group.Sum(x => x.PeriodCount * (x.Term.EndWeek - x.Term.StartWeek + 1)),
                    OveragePeriods = (group.Sum(x => x.PeriodCount * (x.Term.EndWeek - x.Term.StartWeek + 1))) - 17 * group.First().Term.EndWeek,
                    AssignmentDetails = group.Select(a => new AssignmentTeacherDetail
                    {
                        SubjectId = a.SubjectId,
                        SubjectName = a.Subject.SubjectName,
                        ClassName = a.StudentClass.Name,
                        TotalPeriod = a.PeriodCount,
                        StartWeek = a.Term.StartWeek,
                        EndWeek = a.Term.EndWeek
                    }).ToList()
                }).ToList();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_STUDENT_CLASS_ASSIGNMENT_SUCCESS,
                Result = assignmentDictionary
            };
        }
        #endregion
    }
}
