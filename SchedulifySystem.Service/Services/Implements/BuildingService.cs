using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
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
    public class BuildingService : IBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BuildingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region AddBuildings
        public async Task<BaseResponseModel> AddBuildings(int schoolId, List<AddBuildingModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var check = await CheckValidDataAddBuilding(schoolId, models);
            if (check.Status != StatusCodes.Status200OK)
            {
                return check;
            }

            var buildings = _mapper.Map<List<Building>>(models, opt => opt.Items["schoolId"] = schoolId);
            await _unitOfWork.BuildingRepo.AddRangeAsync(buildings);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_BUILDING_SUCCESS };
        }
        #endregion

        #region CheckValidDataAddBuilding
        public async Task<BaseResponseModel> CheckValidDataAddBuilding(int schoolId, List<AddBuildingModel> models)
        {
            var ValidList = new List<AddBuildingModel>();
            var errorList = new List<AddBuildingModel>();

            //check duplicate name in list
            var duplicateNameBuildings = models
             .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateNameBuildings.Any())
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.BUILDING_NAME_DUPLICATED, Result = duplicateNameBuildings };
            }

            //check duplicate Code in list
            var duplicateCodeBuildings = models
             .GroupBy(b => b.BuildingCode, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateCodeBuildings.Any())
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.BUILDING_CODE_DUPLICATED, Result = duplicateNameBuildings };
            }

            // List of names to check in the database
            var modelNames = models.Select(m => m.Name.ToLower()).ToList();
            var modelCodes = models.Select(m => m.BuildingCode.ToUpper()).ToList();

            // Check duplicates in the database
            var foundBuildings = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(
                filter: b => b.SchoolId == schoolId && !b.IsDeleted && (modelNames.Contains(b.Name.ToLower()) || modelCodes.Contains(b.BuildingCode)));

            errorList = _mapper.Map<List<AddBuildingModel>>(foundBuildings.Items);
            ValidList = models.Where(m => !errorList.Any(e => e.Name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            return errorList.Any()
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.BUILDING_CODE_OR_NAME_EXISTED,
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

        #region DeleteBuildings
        public async Task<BaseResponseModel> DeleteBuildings(int buildingId)
        {
            var existed = await _unitOfWork.BuildingRepo.GetByIdAsync(buildingId, filter: t => !t.IsDeleted) ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);

            var roomInDB = await _unitOfWork.RoomRepo.GetV2Async(
               filter: sc => !sc.IsDeleted && existed.Id == sc.BuildingId);

            if (roomInDB != null)
            {
                throw new DefaultException(ConstantResponse.DELETE_BUILDING_FAILED);
            }

            existed.IsDeleted = true;
            _unitOfWork.BuildingRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_BUILDING_SUCCESS};
        }
        #endregion

        #region GetBuildings
        public async Task<BaseResponseModel> GetBuildings(int schoolId, bool? includeRoom = false, int pageIndex = 1, int pageSize = 20)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var buildings = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(pageIndex, pageSize, filter: b => b.SchoolId == schoolId && !b.IsDeleted, include: query => query.Include(b => b.Rooms));
            var response = _mapper.Map<Pagination<BuildingViewModel>>(buildings);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_BUILDING_SUCCESS, Result = response };

        }
        #endregion


        #region UpdateBuildings
        public async Task<BaseResponseModel> UpdateBuildings(int buildingId, UpdateBuildingModel model)
        {
            var existed = await _unitOfWork.BuildingRepo.GetByIdAsync(buildingId) ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);

            // Check duplicates in the database
            var foundBuildings = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(
                filter: b => b.SchoolId == existed.SchoolId && !b.IsDeleted && existed.Id != b.Id && (model.Name.Equals(b.Name.ToLower()) || model.BuildingCode.ToUpper().Equals(b.BuildingCode)));
            if (foundBuildings.Items.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.BUILDING_CODE_OR_NAME_EXISTED
                };
            }
            _mapper.Map(model, existed);
            _unitOfWork.BuildingRepo.Update(existed);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_BUILDING_SUCCESS };
        }
        #endregion
    }
}
