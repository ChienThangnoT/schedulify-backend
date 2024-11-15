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
            // Kiểm tra sự tồn tại của trường học và năm học
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            // Chuẩn bị dữ liệu để kiểm tra trùng lặp
            var codes = models.Select(m => m.StudentClassGroupCode.ToLower()).ToHashSet();
            var names = models.Select(m => m.GroupName.ToLower()).ToHashSet();

            // Kiểm tra dữ liệu trùng lặp
            var checkResult = await CheckData(codes, names, schoolId, schoolYearId);
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            // Ánh xạ và chuẩn bị dữ liệu để lưu vào cơ sở dữ liệu
            var data = _mapper.Map<List<StudentClassGroup>>(models);
            data.ForEach(stg =>
            {
                stg.SchoolId = schoolId;
                stg.SchoolYearId = schoolYearId;
            });

            // Lưu dữ liệu
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
            // Kiểm tra xem nhóm lớp có tồn tại không
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            // Đánh dấu xóa
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
            // Lấy danh sách nhóm lớp theo trường và năm học
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
            // Kiểm tra xem nhóm lớp có tồn tại không
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            // Chuẩn bị dữ liệu để kiểm tra trùng lặp
            var codes = new HashSet<string> { model.StudentClassGroupCode.ToLower() };
            var names = new HashSet<string> { model.GroupName.ToLower() };

            // Sử dụng lại hàm CheckData để kiểm tra trùng lặp
            var checkResult = await CheckData(codes, names, (int)classGroup.SchoolId, (int)classGroup.SchoolYearId, classGroupId); ;
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            // Cập nhật thông tin
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
