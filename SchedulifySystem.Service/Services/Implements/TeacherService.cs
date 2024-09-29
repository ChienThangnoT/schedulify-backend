using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using SchedulifySystem.Service.ViewModels.ResponseModels.TeacherResponseModels;
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
        public async Task<BaseResponseModel> CreateTeacher(CreateTeacherRequestModel createTeacherRequestModel)
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
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Add Teacher success" };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion

        #region GetTeachers
        public async Task<BaseResponseModel> GetTeachers(bool includeDeleted, int pageIndex, int pageSize)
        {
            try
            {
                var teachers = await _unitOfWork.TeacherRepo.GetPaginationAsync(pageSize: pageSize, pageIndex: pageIndex, filter: t => includeDeleted ? true : t.IsDeleted == false);
                var teachersResponse = _mapper.Map<Pagination<TeacherResponseModel>>(teachers);
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion

        #region UpdateTeacher
        public async Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherRequestModel updateTeacherRequestModel)
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
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Update Teacher success" };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion

        #region GetTeacherById
        public async Task<BaseResponseModel> GetTeacherById(int id)
        {
            try
            {
                var teacher = await _unitOfWork.TeacherRepo.GetByIdAsync(id);
                var teachersResponse = _mapper.Map<TeacherResponseModel>(teacher);
                return teacher != null ? new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse } :
                    new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "The teacher is not found!" };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion
    }
}
