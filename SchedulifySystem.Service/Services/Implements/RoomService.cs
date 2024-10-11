using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.BuildingBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
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
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Add success" };
        }

        public async Task<BaseResponseModel> CheckValidDataAddRooms(int schoolId, List<AddRoomModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School id {schoolId} is not found!");

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
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = $"Duplicate room name {duplicateNameRooms.First().Name}!", Result = duplicateNameRooms };
            }

            //check have building in db
            foreach(AddRoomModel model in  models)
            {
                var found = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(filter: b => b.SchoolId == schoolId && !b.IsDeleted && b.Name.ToLower().Equals(model.BuildingName.ToLower()));
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
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "Building name is not found!", Result = errorList };
            }

            //check have room type in db
            foreach (AddRoomModel model in models)
            {
                var found = await _unitOfWork.RoomTypeRepo.ToPaginationIncludeAsync(filter: rt => rt.SchoolId == schoolId && !rt.IsDeleted && rt.Name.ToLower().Equals(model.RoomTypeName.ToLower()));
                if (!found.Items.Any())
                {
                    errorList.Add(model);
                }
                else
                {
                    model.RoomTypeId = found.Items.FirstOrDefault()?.Id;
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status404NotFound, Message = "Room type name is not found!", Result = errorList };
            }


            // List of room names to check in the database
            var modelNames = models.Select(m => m.Name.ToLower()).ToList();

            // Check room duplicates in the database
            var foundRooms = await _unitOfWork.RoomRepo.ToPaginationIncludeAsync(
                filter: b =>  b.Building.SchoolId == schoolId && !b.IsDeleted && modelNames.Contains(b.Name.ToLower()));

            errorList = _mapper.Map<List<AddRoomModel>>(foundRooms.Items);
            ValidList = models.Where(m => !errorList.Any(e => e.Name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            return errorList.Any()
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Duplicate room name found in the database!",
                    Result = new { ValidList, errorList }
                }
                : new BaseResponseModel
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Data is valid!",
                    Result = new { ValidList, errorList }
                };
        }

        public Task<BaseResponseModel> DeleteRoom(int RoomId)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseModel> GetRooms(int schoolId, int buildingId, int pageIndex = 1, int pageSize = 20)
        {
            //var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School id {schoolId} is not found!");
            //var __ = await _unitOfWork.BuildingRepo.GetByIdAsync(buildingId) ?? throw new NotExistsException($"Building id {buildingId} is not found!");
            //var found = await _unitOfWork.RoomRepo.ToPaginationIncludeAsync(pageIndex, pageSize,filter: r => r.Building.SchoolId == schoolId && r.Building.Id == buildingId && !r.IsDeleted);
            //var response = _mapper.Map<Pagination<RoomViewModel>>(found);
            //return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Get building success!", Result = response };
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel> UpdateRoom(int RoomId, UpdateRoomModel model)
        {
            throw new NotImplementedException();
        }
    }
}
