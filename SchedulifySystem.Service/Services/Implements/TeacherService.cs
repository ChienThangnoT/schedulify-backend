using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.RequestModels.TeacherRequestModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using SchedulifySystem.Service.ViewModels.ResponseModels.TeacherResponseModels;
using System;
using System.Collections.Generic;
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
            //try
            //{
                var existedTeacher = await _unitOfWork.TeacherRepo.GetAsync(filter: t => t.Email == createTeacherRequestModel.Email);
                if(existedTeacher.FirstOrDefault() != null)
                {
                    return new BaseResponseModel() { Status = StatusCodes.Status409Conflict, Message = $"Email {createTeacherRequestModel.Email} is existed!" };
                }
                var newTeacher = _mapper.Map<Teacher>(createTeacherRequestModel);
                await _unitOfWork.TeacherRepo.AddAsync(newTeacher);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Add Teacher success" };
            //}
            //catch (Exception ex)
            //{
            //    return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            //}
        }
        #endregion

        #region GetTeachers
        public async Task<BaseResponseModel> GetTeachers(int pageIndex, int pageSize)
        {
            try
            {
                var teachers = await _unitOfWork.TeacherRepo.GetAsync(pageSize: pageSize, pageIndex: pageIndex);
                var teachersResponse = _mapper.Map<List<TeacherResponseModel>>(teachers);
                return new BaseResponseModel() { Status = StatusCodes.Status200OK, Result = teachersResponse };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }
        #endregion
    }
}
