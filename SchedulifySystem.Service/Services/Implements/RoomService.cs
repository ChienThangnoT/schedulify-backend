using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.Enums;
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
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> AddRooms(int schoolId, List<AddRoomModel> models)
        {
            var check = await CheckValidDataAddRooms(schoolId, models);
            if (check.Status != StatusCodes.Status200OK)
            {
                return check;
            }

            var rooms = _mapper.Map<List<Room>>(models);
            await _unitOfWork.RoomRepo.AddRangeAsync(rooms);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_ROOM_SUCCESS };
        }

        public async Task<BaseResponseModel> CheckValidDataAddRooms(int schoolId, List<AddRoomModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var ValidList = new List<AddRoomModel>();
            var errorList = new List<AddRoomModel>();

            //check duplicate name in list
            var duplicateNameRooms = models
             .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateNameRooms.Any())
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.ROOM_NAME_DUPLICATED, Result = duplicateNameRooms };
            }

            //check duplicate code in list
            var duplicateCodeRooms = models
             .GroupBy(b => b.RoomCode, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateCodeRooms.Any())
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.ROOM_CODE_DUPLICATED, Result = duplicateNameRooms };
            }

            //check have building in db
            foreach (AddRoomModel model in models)
            {
                var found = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(filter: b => b.SchoolId == schoolId && !b.IsDeleted && b.BuildingCode.Equals(model.BuildingCode.ToUpper()));
                if (!found.Items.Any())
                {
                    errorList.Add(model);
                }
                else
                {
                    model.buildingId = found.Items.FirstOrDefault()?.Id;
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = ConstantResponse.BUILDING_CODE_NOT_EXIST, Result = errorList };
            }


            // List of room names && code to check in the database
            var modelNames = models.Select(m => m.Name.ToLower()).ToList();
            var modelCodes = models.Select(m => m.RoomCode.ToUpper()).ToList();

            // Check room duplicates in the database
            var foundRooms = await _unitOfWork.RoomRepo.ToPaginationIncludeAsync(
                filter: b => b.Building.SchoolId == schoolId && !b.IsDeleted &&
                (modelNames.Contains(b.Name.ToLower()) || modelCodes.Contains(b.RoomCode)));

            errorList = _mapper.Map<List<AddRoomModel>>(foundRooms.Items);
            ValidList = models.Where(m => !errorList.Any(e => e.Name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            return errorList.Any()
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.ROOM_CODE_OR_NAME_EXISTED,
                    Result = new { ValidList, errorList }
                }
                : new BaseResponseModel
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Data is valid!",
                    Result = new { ValidList, errorList }
                };
        }

        public async Task<BaseResponseModel> DeleteRoom(int RoomId)
        {
            var room = await _unitOfWork.RoomRepo.GetByIdAsync(RoomId) ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            room.IsDeleted = true;
            _unitOfWork.RoomRepo.Update(room);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_ROOM_SUCCESS };
        }

        public async Task<BaseResponseModel> GetRooms(int schoolId, int? buildingId, ERoomType? roomType, int pageIndex = 1, int pageSize = 20)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (buildingId != null)
            {
                var __ = await _unitOfWork.BuildingRepo.GetByIdAsync((int)buildingId) ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);
            }
            var found = await _unitOfWork.RoomRepo
                .ToPaginationIncludeAsync(
                    pageIndex, pageSize,
                    filter: r => r.Building.SchoolId == schoolId && (buildingId == null ? true : r.Building.Id == buildingId) && (roomType == null || roomType == (ERoomType) r.RoomType) && !r.IsDeleted
                );
            var response = _mapper.Map<Pagination<RoomViewModel>>(found);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_ROOM_SUCCESS, Result = response };
        }

        public async Task<BaseResponseModel> UpdateRoom(int RoomId, UpdateRoomModel model)
        {
            var room = await _unitOfWork.RoomRepo.GetByIdAsync(RoomId) ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            var _ = await _unitOfWork.BuildingRepo.GetByIdAsync(model.BuildingId) ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);
             
            //check existed name or code
            var foundRooms = await _unitOfWork.RoomRepo.ToPaginationIncludeAsync(
                filter: b => !b.IsDeleted && b.Id != room.Id &&
                (model.Name.ToLower().Equals(b.Name.ToLower())) || model.RoomCode.ToUpper().Equals(b.RoomCode));
            if(foundRooms.Items.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.ROOM_CODE_OR_NAME_EXISTED,
                };
            }
            var newRoom = _mapper.Map(model, room);
            _unitOfWork.RoomRepo.Update(newRoom);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_ROOM_SUCCESS };
        }
    }
}
