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
        public async Task<BaseResponseModel> CreateTeachers(int schoolId, List<CreateTeacherModel> createTeacherRequestModels)
        {
            var _ = _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School is {schoolId} is not found!");
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var addedTeachers = new List<string>();

                    var skippedTeachers = new List<string>();

                    foreach (var createTeacherRequestModel in createTeacherRequestModels)
                    {
                        var existedTeacher = await _unitOfWork.TeacherRepo.GetAsync(filter: t =>!t.IsDeleted && t.SchoolId == schoolId && t.Email == createTeacherRequestModel.Email);

                        if (existedTeacher.FirstOrDefault() != null)
                        {
                            skippedTeachers.Add($"{createTeacherRequestModel.FirstName} {createTeacherRequestModel.LastName} is cannot add due to email {createTeacherRequestModel.Email} is existed");
                            continue;
                        }

                        var newTeacher = _mapper.Map<Teacher>(createTeacherRequestModel);
                        await _unitOfWork.TeacherRepo.AddAsync(newTeacher);
                        addedTeachers.Add($"{newTeacher.FirstName} {newTeacher.LastName} is added");
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Operation completed",
                        Result = new
                        {
                            AddedTeachers = addedTeachers,
                            SkippedTeachers = skippedTeachers
                        }
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = $"Error: {ex.Message}"
                    };
                }
            }
        }

        #endregion

        #region GetTeachers
        public async Task<BaseResponseModel> GetTeachers(int schoolId, bool includeDeleted, int pageIndex, int pageSize)
        {
            _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException("School is not found!");

            var teachers = await _unitOfWork.TeacherRepo.ToPaginationIncludeAsync(pageSize: pageSize, pageIndex: pageIndex, filter: t => t.SchoolId == schoolId && (includeDeleted ? true : t.IsDeleted == false),
                include: query => query.Include(t => t.Department).Include(t => t.Group).Include(t => t.TeachableSubjects));
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
                        return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "The teacher is not found!" };
                    }
                    _mapper.Map(updateTeacherRequestModel, existedTeacher);
                    _unitOfWork.TeacherRepo.Update(existedTeacher);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Update Teacher success" };
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
                    new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "The teacher is not found!" };
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
                    return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "The teacher is not found!" };
                }
                existedTeacher.IsDeleted = true;
                _unitOfWork.TeacherRepo.Update(existedTeacher);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Deleted Teacher success" };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion
    }
}
