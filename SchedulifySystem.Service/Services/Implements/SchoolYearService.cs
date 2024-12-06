using AutoMapper;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Utilities;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels;
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
    public class SchoolYearService : ISchoolYearService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly int[] START_WEEKS = [1, 19];
        private readonly int[] END_WEEKS = [18, 35];

        public SchoolYearService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> AddSchoolYear(SchoolYearAddModel model)
        {
            try
            {
                var startYear = int.Parse(model.StartYear);
                var endYear = int.Parse(model.EndYear);
                if (startYear >= endYear)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Năm học kết thúc phải lớn hơn năm học bắt đầu."
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Năm học không hợp lệ."
                };
            }


            var check = await _unitOfWork.SchoolYearRepo.GetV2Async(
                filter: f => !f.IsDeleted && f.SchoolYearCode.ToLower() == model.SchoolYearCode.ToLower());

            if (check.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mã năm học đã tồn tại."
                };
            }

            var year = _mapper.Map<SchoolYear>(model);
            year.CreateDate = DateTime.UtcNow;

            for (var i = 1; i <= 2; i++)
            {
                year.Terms.Add(new Term()
                {
                    Name = $"HK{i}",
                    StartWeek = START_WEEKS[i - 1],
                    EndWeek = END_WEEKS[i - 1],
                    CreateDate = DateTime.UtcNow
                });
            }
            await _unitOfWork.SchoolYearRepo.AddAsync(year);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Thêm năm học thành công."
            };
        }

        public async Task<BaseResponseModel> DeteleSchoolYear(int id)
        {
            var found = await _unitOfWork.SchoolYearRepo.GetByIdAsync(id, filter: f => !f.IsDeleted) ??
                throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            if (found.IsPublic)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không thể xóa năm học đang được công bố."
                };
            }
            found.IsDeleted = true;
            _unitOfWork.SchoolYearRepo.Update(found);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa năm học thành công."
            };
        }

        public async Task<BaseResponseModel> GetSchoolYear(bool includePrivate)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetAsync(filter: t => t.IsDeleted == false && (includePrivate || t.IsPublic == true));

            var result = _mapper.Map<List<SchoolYearViewModel>>(schoolYear);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SCHOOL_YEAR_SUCCESS,
                Result = result
            };
        }

        public async Task<BaseResponseModel> UpdatePublicStatus(int id, bool status)
        {
            var found = await _unitOfWork.SchoolYearRepo.GetByIdAsync(id, filter: f => !f.IsDeleted) ??
                 throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            found.IsPublic = status;
            found.UpdateDate = DateTime.UtcNow;
            _unitOfWork.SchoolYearRepo.Update(found);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật trạng thái thành công."
            };
        }

        public async Task<BaseResponseModel> UpdateSchoolYear(int id, SchoolYearUpdateModel model)
        {
            var found = await _unitOfWork.SchoolYearRepo.GetByIdAsync(id, filter: f => !f.IsDeleted) ??
                throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            if (found.IsPublic)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không thể cập nhât năm học đang được công bố."
                };
            }

            try
            {
                var startYear = int.Parse(model.StartYear);
                var endYear = int.Parse(model.EndYear);
                if (startYear >= endYear)
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Năm học kết thúc phải lớn hơn năm học bắt đầu."
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Năm học không hợp lệ."
                };
            }

            found.StartYear = model.StartYear;
            found.EndYear = model.EndYear;

            if (found.SchoolYearCode.ToLower() != model.SchoolYearCode.ToLower())
            {
                var check = await _unitOfWork.SchoolYearRepo.GetV2Async(
                filter: f => !f.IsDeleted && f.SchoolYearCode.ToLower() == model.SchoolYearCode.ToLower() && f.Id != id);

                if (check.Any())
                {
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mã năm học đã tồn tại."
                    };
                }
                found.SchoolYearCode = model.SchoolYearCode;
            }
            found.UpdateDate = DateTime.UtcNow;
            _unitOfWork.SchoolYearRepo.Update(found);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật năm học thành công."
            };
        }
    }
}
