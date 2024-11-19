using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t=> t.IsDeleted == false) 
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var ValidList = new List<AddRoomModel>();
            var errorList = new List<AddRoomModel>();

            //check duplicate name in list
            errorList = models
             .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (errorList.Any())
            {
                return new BaseResponseModel 
                { 
                    Status = StatusCodes.Status400BadRequest, 
                    Message = ConstantResponse.ROOM_NAME_DUPLICATED, 
                    Result = new { ValidList = models.Where(m => !errorList.Contains(m)), errorList } 
                };
            }
            //check duplicate code in list
            errorList = models
             .GroupBy(b => b.RoomCode, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (errorList.Any())
            {
                return new BaseResponseModel 
                { 
                    Status = StatusCodes.Status400BadRequest, 
                    Message = ConstantResponse.ROOM_CODE_DUPLICATED, 
                    Result = new { ValidList = models.Where(m => !errorList.Contains(m)), errorList } 
                };
            }


            // check subject abreviation

            var subjects = (await _unitOfWork.SubjectRepo.GetV2Async(filter: f => !f.IsDeleted ));
            var subjectAbreviations = subjects.Select(s => s.Abbreviation.ToLower());
            foreach (var model in models)
            {
                if(model.RoomType == ERoomType.PRACTICE_ROOM)
                {
                    if (model.SubjectsAbreviation.IsNullOrEmpty())
                    {
                        errorList.Add(model);
                    }
                    else
                    {
                        if(!model.SubjectsAbreviation.All(s => subjectAbreviations.Contains(s, StringComparer.OrdinalIgnoreCase)))
                        {
                            errorList.Add(model);
                        }
                        else
                        {
                           foreach(var s in model.SubjectsAbreviation)
                            {
                                model.RoomSubjects.Add(new RoomSubject()
                                {
                                    SubjectId = subjects.First(sj => sj.Abbreviation.ToLower().Equals(s.ToLower())).Id,
                                    CreateDate = DateTime.UtcNow,
                                });
                            }
                        }
                    }
                }
            }

            if (errorList.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.ROOM_TYPE_BAD_REQUEST,
                    Result = new { ValidList = models.Where(m => !errorList.Contains(m)), errorList }
                };
            }

            //check have building in db
            foreach (AddRoomModel model in models)
            {
                var found = await _unitOfWork.BuildingRepo.ToPaginationIncludeAsync(
                    filter: b => b.SchoolId == schoolId && !b.IsDeleted && b.BuildingCode.Equals(model.BuildingCode.ToUpper()));
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
                return new BaseResponseModel() 
                { 
                    Status = StatusCodes.Status404NotFound, 
                    Message = ConstantResponse.BUILDING_CODE_NOT_EXIST, 
                    Result = new { ValidList = models.Where(m => !errorList.Contains(m)), errorList } 
                };
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
            var room = await _unitOfWork.RoomRepo.GetByIdAsync(RoomId, filter: t=> t.IsDeleted == false) 
                ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            room.IsDeleted = true;
            _unitOfWork.RoomRepo.Update(room);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_ROOM_SUCCESS };
        }

        public async Task<BaseResponseModel> GetRoomById(int roomId)
        {
            var room = await _unitOfWork.RoomRepo.GetByIdAsync(roomId, filter: t=> t.IsDeleted == false, 
                include: query => query.Include(r => r.RoomSubjects).ThenInclude(rs => rs.Subject)) 
                ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_ROOM_SUCCESS,
                Result = _mapper.Map<RoomViewModel>(room)
            };
        }

        public async Task<BaseResponseModel> GetRooms(int schoolId, int? buildingId, ERoomType? roomType, int pageIndex = 1, int pageSize = 20)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active) 
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (buildingId != null)
            {
                var __ = await _unitOfWork.BuildingRepo.GetByIdAsync((int)buildingId, filter: t=> t.IsDeleted == false) 
                    ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);
            }
            var found = await _unitOfWork.RoomRepo
                .ToPaginationIncludeAsync(
                    pageIndex, pageSize,
                    filter: r => r.Building.SchoolId == schoolId && (buildingId == null ? true : r.Building.Id == buildingId) && (roomType == null || roomType == (ERoomType) r.RoomType) && !r.IsDeleted
                    , include: query => query.Include(r => r.RoomSubjects).ThenInclude(rs => rs.Subject)
                );
            if (found.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            }
            var response = _mapper.Map<Pagination<RoomViewModel>>(found);
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.GET_ROOM_SUCCESS, Result = response };
        }

        public async Task<BaseResponseModel> UpdateRoom(int RoomId, UpdateRoomModel model)
        {
            var room = await _unitOfWork.RoomRepo.GetByIdAsync(RoomId, filter: t => t.IsDeleted == false ,include: query => query.Include(r => r.RoomSubjects))
                ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            var building = await _unitOfWork.BuildingRepo.GetByIdAsync(model.BuildingId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.BUILDING_NOT_EXIST);

            // Check existed name or code
            var isDuplicate = await _unitOfWork.RoomRepo.ToPaginationIncludeAsync(
                filter: b => !b.IsDeleted && b.Id != room.Id && !model.Name.Equals(room.Name, StringComparison.OrdinalIgnoreCase) 
                && !model.RoomCode.Equals(room.RoomCode, StringComparison.OrdinalIgnoreCase) &&
                (model.Name.Equals(b.Name, StringComparison.OrdinalIgnoreCase) ||
                 model.RoomCode.Equals(b.RoomCode, StringComparison.OrdinalIgnoreCase)));

            if (isDuplicate.Items.Count != 0)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.ROOM_CODE_OR_NAME_EXISTED,
                };
            }

            // Check subject
            var subjects = (await _unitOfWork.SubjectRepo.GetV2Async(filter: f => !f.IsDeleted));
            var subjectIds = subjects.Select(s => s.Id).ToList();
            var newRoomSubjects = new List<RoomSubject>();

            if (model.RoomType == ERoomType.PRACTICE_ROOM)
            {
                if (model.SubjectIds == null || !model.SubjectIds.All(s => subjectIds.Contains(s)))
                {
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = ConstantResponse.ROOM_TYPE_BAD_REQUEST
                    };
                }

                // remove roomSubject không còn nằm trong danh sách SubjectIds và xóa khỏi db
                var subjectsToRemove = room.RoomSubjects
                    .Where(rs => !model.SubjectIds.Contains((int)rs.SubjectId))
                    .ToList();

                foreach (var subjectToRemove in subjectsToRemove)
                {
                    _unitOfWork.RoomSubjectRepo.Remove(subjectToRemove);
                }

                // add các môn học mới vào RoomSubjects nếu chưa có
                foreach (var item in model.SubjectIds)
                {
                    if (!room.RoomSubjects.Any(rs => rs.SubjectId == item))
                    {
                        newRoomSubjects.Add(new RoomSubject() { RoomId = room.Id, SubjectId = item, CreateDate = DateTime.UtcNow });
                    }
                }

                if (newRoomSubjects.Count != 0)
                {
                    await _unitOfWork.RoomSubjectRepo.AddRangeAsync(newRoomSubjects);
                }
            }

            _mapper.Map(model, room);

            _unitOfWork.RoomRepo.Update(room);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_ROOM_SUCCESS
            };
        }


    }
}
