using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.AccountBusinessModels;
using SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels;
using SchedulifySystem.Service.BusinessModels.EmailModels;
using SchedulifySystem.Service.BusinessModels.RoleAssignmentBusinessModels;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
        }
        #region Add Department
        public async Task<BaseResponseModel> AddDepartment(int schoolId, List<DepartmentAddModel> models)
        {
            try
            {
                var checkSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

                //check duplicate in list
                var duplicateName = models.GroupBy(d => d.Name, StringComparer.OrdinalIgnoreCase).Where(d => d.Count() > 1).SelectMany(g => g).ToList();
                var duplicateCode = models.GroupBy(d => d.DepartmentCode, StringComparer.OrdinalIgnoreCase).Where(d => d.Count() > 1).SelectMany(g => g).ToList();

                if (duplicateName.Count != 0)
                {
                    return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.DEPARTMENT_NAME_DUPLICATE, Result = duplicateName };
                }

                if (duplicateCode.Count != 0)
                {
                    return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.DEPARTMENT_CODE_DUPLICATE, Result = duplicateCode };
                }
                //check duplicate in database 
                var names = models.Select(d => d.Name.ToLower()).ToList();
                var code = models.Select(d => d.DepartmentCode.ToLower()).ToList();
                var duplicateInDb = (await _unitOfWork.DepartmentRepo.GetV2Async(
                    filter: d => d.SchoolId == schoolId && !d.IsDeleted && (names.Contains(d.Name.ToLower()) || code.Contains(d.DepartmentCode.ToLower())))).ToList();

                if (duplicateInDb.Count != 0)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.DEPARTMENT_NAME_OR_CODE_EXISTED,
                        Result = _mapper.Map<List<DepartmentAddModel>>(duplicateInDb)
                    };
                }

                //add to db
                models.ForEach(d => d.SchoolId = schoolId);
                var data = _mapper.Map<List<Department>>(models);
                await _unitOfWork.DepartmentRepo.AddRangeAsync(data);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_DEPARTMENT_SUCCESS };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete Department
        public async Task<BaseResponseModel> DeleteDepartment(int departmentId)
        {
            var existed = await _unitOfWork.DepartmentRepo.GetByIdAsync(departmentId)
                ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);

            existed.IsDeleted = true;
            _unitOfWork.DepartmentRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_DEPARTMENT_SUCCESS };
        }
        #endregion

        #region Get Departments
        public async Task<BaseResponseModel> GetDepartments(int schoolId, int pageIndex = 1, int pageSize = 20)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                 ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var departments = await _unitOfWork.DepartmentRepo
                .ToPaginationIncludeAsync(pageIndex, pageSize, filter: (f => f.SchoolId == schoolId && !f.IsDeleted));
            var result = _mapper.Map<Pagination<DepartmentViewModel>>(departments);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_DEPARTMENT_SUCCESS, Result = result };
        }
        #endregion

        #region Update Department
        public async Task<BaseResponseModel> UpdateDepartment(int departmentId, int schoolId, DepartmentUpdateModel model)
        {
            var existed = await _unitOfWork.DepartmentRepo.GetByIdAsync(departmentId, filter: t => t.IsDeleted == false && schoolId == t.SchoolId)
                ?? throw new NotExistsException(ConstantResponse.DEPARTMENT_NOT_EXIST);
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (model.Name != null || model.DepartmentCode != null)
            {
                var check = (await _unitOfWork.DepartmentRepo.GetV2Async(
                                filter: d => d.SchoolId == schoolId && !d.IsDeleted && (model.Name == null || d.Name.ToLower().Equals(model.Name.ToLower()))
                                && (model.DepartmentCode == null || d.DepartmentCode.ToLower().Equals(model.DepartmentCode.ToLower())))).ToList();
                if (check.Count != 0)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.DEPARTMENT_NAME_OR_CODE_EXISTED,
                    };
                }
            }

            if (model.Name != null)
            {
                existed.Name = model.Name;
            }
            if (model.DepartmentCode != null)
            {
                existed.DepartmentCode = model.DepartmentCode;
            }

            if (model.Description != null)
            {
                existed.Description = model.Description;
            }

            _unitOfWork.DepartmentRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_DEPARTMENT_SUCCESS };
        }
        #endregion

        public async Task<BaseResponseModel> GenerateDepartmentAccount(GenerateTeacherInDepartmentAccountModel generateMode)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(generateMode.SchoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var departments = await _unitOfWork.TeacherRepo.GetByIdAsync(generateMode.DepartmentId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);
            var teachers = (await _unitOfWork.TeacherRepo.GetAsync(filter: t => t.DepartmentId == generateMode.DepartmentId && t.IsDeleted == false && t.Status != (int)TeacherStatus.NgungHoatDong));
            var teacherEmails = teachers.Select(t => t.Email);

            var teacherAccount = (await _unitOfWork.RoleAssignmentRepo.GetV2Async(filter: t => teacherEmails.Contains(t.Account.Email) && t.Account.Status == (int)AccountStatus.Active)).Select(t => t.AccountId);
            if (teacherAccount != null)
            {
                var existRole = (await _unitOfWork.RoleAssignmentRepo.GetV2Async(
                    filter: t => teacherAccount.Contains(t.AccountId) && t.Role.Name.ToLower() == RoleEnum.Teacher.ToString().ToLower() && t.IsDeleted == false,
                    include: query => query.Include(u => u.Role))).Select(q => q.Account.Id);
                if (existRole != null)
                {
                    teacherAccount = teacherAccount.Except(existRole).ToList();
                }
            }
            if (teacherAccount == null)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.GENERATE_TEACHER_IN_DEPARTMENT_FAILED };
            }

            foreach (var teacher in teacherAccount)
            {
                var newTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync(teacher);
                var accountPassword = AuthenticationUtils.GeneratePassword();
                Account account = new()
                {
                    Email = newTeacher.Email,
                    Password = AuthenticationUtils.HashPassword(accountPassword),
                    FirstName = newTeacher.FirstName,
                    LastName = newTeacher.LastName,
                    SchoolId = school.Id,
                    IsChangeDefaultPassword = false,
                    Status = (int)AccountStatus.Active,
                    Phone = newTeacher.Phone,
                    AvatarURL = newTeacher.AvatarURL,
                    CreateDate = DateTime.UtcNow,
                    IsConfirmSchoolManager = false
                };

                await _unitOfWork.UserRepo.AddAsync(account);
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
            }

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = ConstantResponse.GENERATE_TEACHER_IN_DEPARTMENT_SUCCESS
            };
        }

    }
}
