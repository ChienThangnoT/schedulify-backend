using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
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
                include: query => query.Include(t => t.Department).Include(t => t.TeachableSubjects));
            var teachersResponse = _mapper.Map<Pagination<TeacherViewModel>>(teachers);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse };
        }
        #endregion

        #region UpdateTeacher
        public async Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherModel updateTeacherRequestModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    var existedTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id);
                    if (existedTeacher == null)
                    {
                        return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.TEACHER_NOT_EXIST };
                    }
                    _mapper.Map(updateTeacherRequestModel, existedTeacher);
                    _unitOfWork.TeacherRepo.Update(existedTeacher);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_TEACHER_SUCCESS };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
                }
            }

        }
        #endregion

        #region GetTeacherById
        public async Task<BaseResponseModel> GetTeacherById(int id)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id);
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
    }
}
