﻿using SchedulifySystem.Service.BusinessModels.TeacherBusinessModels;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<BaseResponseModel> GetTeachers(int schoolId, bool includeDeleted, int pageIndex, int pageSize);
        Task<BaseResponseModel> CreateTeacher(CreateTeacherModel createTeacherRequestModel);
        Task<BaseResponseModel> CreateTeachers(int schoolId, List<CreateListTeacherModel> createTeacherRequestModels);
        Task<BaseResponseModel> UpdateTeacher(int id, UpdateTeacherModel updateTeacherRequestModel);
        Task<BaseResponseModel> GetTeacherById(int id);
        Task<BaseResponseModel> DeleteTeacher(int id);
    }
}
