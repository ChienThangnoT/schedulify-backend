using AutoMapper;
using Microsoft.AspNetCore.Http;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
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
    public class StudentClassGroupService : IStudentClassGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClassGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<BaseResponseModel> CheckData(HashSet<string> codes, HashSet<string> names, int schoolId, int schoolYearId, int skip = 0)
        {
            // Kiểm tra trùng lặp trong danh sách đầu vào
            if (codes.Count != codes.Distinct().Count() || names.Count != names.Distinct().Count())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NAME_OR_CODE_EXISTED
                };
            }

            // Kiểm tra trùng lặp trong cơ sở dữ liệu
            var classGroupFounds = await _unitOfWork.StudentClassGroupRepo.GetV2Async(
                filter: stg => !stg.IsDeleted && stg.SchoolId == schoolId && stg.SchoolYearId == schoolYearId && stg.Id != skip &&
                (codes.Contains(stg.StudentClassGroupCode.ToLower()) || names.Contains(stg.GroupName.ToLower())));

            if (classGroupFounds.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NAME_OR_CODE_EXISTED
                };
            }

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK
            };
        }

        public async Task<BaseResponseModel> AddStudentClassgroup(int schoolId, int schoolYearId, List<AddStudentClassGroupModel> models)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            var codes = models.Select(m => m.StudentClassGroupCode.ToLower()).ToHashSet();
            var names = models.Select(m => m.GroupName.ToLower()).ToHashSet();

            var checkResult = await CheckData(codes, names, schoolId, schoolYearId);
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            var data = _mapper.Map<List<StudentClassGroup>>(models);
            data.ForEach(stg =>
            {
                stg.SchoolId = schoolId;
                stg.SchoolYearId = schoolYearId;
            });

            await _unitOfWork.StudentClassGroupRepo.AddRangeAsync(data);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.ADD_STUDENT_CLASS_GROUP_SUCCESS
            };
        }

        public async Task<BaseResponseModel> DeleteStudentClassGroup(int classGroupId)
        {
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            classGroup.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.DELETE_STUDENT_CLASS_GROUP_SUCCESS
            };
        }

        public async Task<BaseResponseModel> GetStudentClassGroups(int schoolId, int schoolYearId, int pageIndex = 1, int pageSize = 20)
        {
            var classGroups = await _unitOfWork.StudentClassGroupRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: stg => !stg.IsDeleted && stg.SchoolId == schoolId && stg.SchoolYearId == schoolYearId,
                orderBy: stg => stg.OrderBy(c => c.GroupName)
            );


            var result = _mapper.Map<Pagination<StudentClassGroupViewModel>>(classGroups);

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_STUDENT_CLASS_GROUP_SUCCESS,
                Result = result
            };
        }

        public async Task<BaseResponseModel> UpdateStudentClassGroup(int classGroupId, UpdateStudentClassGroupModel model)
        {
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            var codes = new HashSet<string> { model.StudentClassGroupCode.ToLower() };
            var names = new HashSet<string> { model.GroupName.ToLower() };

            var checkResult = await CheckData(codes, names, (int)classGroup.SchoolId, (int)classGroup.SchoolYearId, classGroupId); ;
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            classGroup.StudentClassGroupCode = model.StudentClassGroupCode;
            classGroup.GroupName = model.GroupName;
            classGroup.GroupDescription = model.GroupDescription;

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_STUDENT_CLASS_GROUP_SUCCESS
            };
        }

    }

}
