using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.RoomBusinessModels;
using SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels;
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
    public class RoomSubjectService : IRoomSubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomSubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region Add Room Subject
        public async Task<BaseResponseModel> AddRoomSubject(RoomSubjectAddModel roomSubjectAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var subjectExists = await _unitOfWork.SubjectRepo.ExistsAsync(s => s.Id == roomSubjectAddModel.SubjectId && !s.IsDeleted);
                    if (!subjectExists)
                    {
                        throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
                    }

                    var schoolExist = await _unitOfWork.SchoolRepo.ExistsAsync(s => s.Id == roomSubjectAddModel.SchoolId && !s.IsDeleted);
                    if (!schoolExist)
                    {
                        throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
                    }

                    var termExist = await _unitOfWork.TermRepo.ExistsAsync(s => s.Id == roomSubjectAddModel.TermId && !s.IsDeleted);
                    if (!termExist)
                    {
                        throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
                    }

                    var roomExists = (await _unitOfWork.RoomRepo.GetAsync(r => r.Id == roomSubjectAddModel.RoomId && !r.IsDeleted)).FirstOrDefault();
                    if (roomExists == null)
                    {
                        throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
                    }

                    if (roomExists.MaxClassPerTime < roomSubjectAddModel.StudentClassId.Distinct().Count())
                    {
                        throw new NotExistsException(ConstantResponse.ROOM_CAPILITY_NOT_ENOUGH);
                    }

                    var validStudentClassIds = await _unitOfWork.StudentClassesRepo
                        .GetAsync(s => roomSubjectAddModel.StudentClassId.Contains(s.Id) && !s.IsDeleted);

                    var validStudentClassIdList = validStudentClassIds.Select(s => s.Id).ToList();

                    var invalidStudentClassIds = roomSubjectAddModel.StudentClassId
                        .Except(validStudentClassIdList)
                        .ToList();

                    if (invalidStudentClassIds.Count != 0)
                    {
                        throw new NotExistsException($"Các StudentClassId không hợp lệ: {string.Join(", ", invalidStudentClassIds)}");
                    }

                    var roomSubjectCodeExists = await _unitOfWork.RoomSubjectRepo.ExistsAsync(rs =>
                        (!string.IsNullOrEmpty(rs.RoomSubjectCode) && rs.RoomSubjectCode.ToLower() == roomSubjectAddModel.RoomSubjectCode.ToLower())
                        || (!string.IsNullOrEmpty(rs.RoomSubjectName) && rs.RoomSubjectName.ToLower() == roomSubjectAddModel.RoomSubjectName.ToLower())
                        && !rs.IsDeleted);



                    if (roomSubjectCodeExists)
                    {
                        throw new DefaultException(ConstantResponse.ROOM_SUBJECT_NAME_OR_CODE_EXIST);
                    }

                    var newRoomSubject = _mapper.Map<RoomSubject>(roomSubjectAddModel);

                    await _unitOfWork.RoomSubjectRepo.AddAsync(newRoomSubject);
                    await _unitOfWork.SaveChangesAsync();

                    // add id StudentClassRoomSubject
                    var studentClassRoomSubjects = roomSubjectAddModel.StudentClassId
                        .Select(studentClassId => new StudentClassRoomSubject
                        {
                            RoomSubjectId = newRoomSubject.Id,
                            StudentClassId = studentClassId
                        }).ToList();

                    await _unitOfWork.StudentClassRoomSubjectRepo.AddRangeAsync(studentClassRoomSubjects);

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.ADD_ROOM_SUBJECT_SUCCESS
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new BaseResponseModel() { Status = StatusCodes.Status500InternalServerError, Message = $"Error: {ex.Message}" };
                }
            }
        }

        #endregion

        #region Update Room Subject
        public async Task<BaseResponseModel> UpdateRoomSubject(int schoolId, int id, RoomSubjectUpdateModel model)
        {
            var subjectExists = await _unitOfWork.SubjectRepo.ExistsAsync(s => s.Id == model.SubjectId && !s.IsDeleted);
            if (!subjectExists)
            {
                throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
            }

            var schoolExist = await _unitOfWork.SchoolRepo.ExistsAsync(s => s.Id == schoolId && !s.IsDeleted);
            if (!schoolExist)
            {
                throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            }

            var termExist = await _unitOfWork.TermRepo.ExistsAsync(s => s.Id == model.TermId && !s.IsDeleted);
            if (!termExist)
            {
                throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
            }
            var roomExists = (await _unitOfWork.RoomRepo.GetAsync(r => r.Id == model.RoomId && !r.IsDeleted)).FirstOrDefault();
            if (roomExists == null)
            {
                throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            }
            if (roomExists.MaxClassPerTime < model.StudentClassIds.Distinct().Count())
            {
                throw new NotExistsException(ConstantResponse.ROOM_CAPILITY_NOT_ENOUGH);
            }
            var validStudentClassIds = await _unitOfWork.StudentClassesRepo
                        .GetAsync(s => model.StudentClassIds.Contains(s.Id) && !s.IsDeleted);

            var validStudentClassIdList = validStudentClassIds.Select(s => s.Id).ToList();

            var invalidStudentClassIds = model.StudentClassIds
                .Except(validStudentClassIdList)
                .ToList();

            if (invalidStudentClassIds.Count != 0)
            {
                throw new NotExistsException($"Các StudentClassId không hợp lệ: {string.Join(", ", invalidStudentClassIds)}");
            }
            var roomSubjectExisted = await _unitOfWork.RoomSubjectRepo.GetByIdAsync(id, filter: f => !f.IsDeleted && f.SchoolId == schoolId, 
                include: query => query.Include(rs => rs.StudentClassRoomSubjects)) ??
                throw new NotExistsException(ConstantResponse.ROOM_SUBJECT_NOT_EXIST);
            if (!model.RoomSubjectName.Equals(roomSubjectExisted.RoomSubjectName, StringComparison.OrdinalIgnoreCase) || !model.RoomSubjectCode.Equals(roomSubjectExisted.RoomSubjectCode, StringComparison.OrdinalIgnoreCase))
            {
                var roomSubjectCodeExists = await _unitOfWork.RoomSubjectRepo.ExistsAsync(rs => rs.Id != id &&
                                       (!string.IsNullOrEmpty(rs.RoomSubjectCode) && rs.RoomSubjectCode.ToLower() == model.RoomSubjectCode.ToLower())
                                       || (!string.IsNullOrEmpty(rs.RoomSubjectName) && rs.RoomSubjectName.ToLower() == model.RoomSubjectName.ToLower())
                                       && !rs.IsDeleted);

                if (roomSubjectCodeExists)
                {
                    throw new DefaultException(ConstantResponse.ROOM_SUBJECT_NAME_OR_CODE_EXIST);
                }
            }
            var scrs = roomSubjectExisted.StudentClassRoomSubjects.Where(s => !s.IsDeleted);
            var newClasses = model.StudentClassIds.Except(scrs.Select(s => s.Id));
            var newClassesRoomSubjects = newClasses
                       .Select(studentClassId => new StudentClassRoomSubject
                       {
                           RoomSubjectId = id,
                           StudentClassId = studentClassId
                       }).ToList();
            var oldIds = scrs.Select(c => c.Id).Except(model.StudentClassIds);
            var oldClassRomSubjects = scrs.Where(s => oldIds.Contains(s.Id));
            if(oldClassRomSubjects.Any())
            {
                foreach(var item in oldClassRomSubjects)
                {
                    _unitOfWork.StudentClassRoomSubjectRepo.Remove(item);
                }
            }
            await _unitOfWork.StudentClassRoomSubjectRepo.AddRangeAsync(newClassesRoomSubjects);
            _mapper.Map(model, roomSubjectExisted);
            _unitOfWork.RoomSubjectRepo.Update(roomSubjectExisted);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_ROOM_SUBJECT_SUCCESS
            };

        }
        #endregion

        #region Delete Room Subject
        public async Task<BaseResponseModel> DeleteRoomSubject(int schoolId, int id)
        {
            var schoolExist = await _unitOfWork.SchoolRepo.ExistsAsync(s => s.Id == schoolId && !s.IsDeleted);
            if (!schoolExist)
            {
                throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            }
            var roomSubjectExisted = await _unitOfWork.RoomSubjectRepo.GetByIdAsync(id, filter: f => !f.IsDeleted && f.SchoolId == schoolId,
                include: query => query.Include(rs => rs.StudentClassRoomSubjects)) ??
                throw new NotExistsException(ConstantResponse.ROOM_SUBJECT_NOT_EXIST);
            roomSubjectExisted.IsDeleted = true;
            var scrs = roomSubjectExisted.StudentClassRoomSubjects.Where(s => !s.IsDeleted);
            foreach (var item in scrs)
            {
                _unitOfWork.StudentClassRoomSubjectRepo.Remove(item);
            }
            _unitOfWork.RoomSubjectRepo.Update(roomSubjectExisted);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.DELETE_ROOM_SUBJECT_SUCCESS
            };
        }
        #endregion

        #region View Room Subject List
        public async Task<BaseResponseModel> ViewRoomSubjectList(int schoolId, int? roomSubjectId, int? termId, int pageIndex, int pageSize)
        {
            var schoolExists = await _unitOfWork.SchoolRepo.ExistsAsync(s => s.Id == schoolId && s.Status == (int)SchoolStatus.Active);
            if (!schoolExists)
            {
                throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            }
            var get = await _unitOfWork.RoomSubjectRepo.GetV2Async(filter: t => t.SchoolId == schoolId && !t.IsDeleted,
                include: query => query.Include(t => t.StudentClassRoomSubjects).ThenInclude(scrs => scrs.StudentClass),
                orderBy: query => query.OrderBy(rs => rs.UpdateDate),
                pageIndex: pageIndex, pageSize: pageSize);

            var roomSubjectsQuery = await _unitOfWork.RoomSubjectRepo.ToPaginationIncludeAsync(
                filter: rs => rs.SchoolId == schoolId && (roomSubjectId == null || rs.Id == roomSubjectId)
                    && (termId == null || rs.TermId == termId)
                    && !rs.IsDeleted,
                include: query => query.Include(t => t.StudentClassRoomSubjects).ThenInclude(scrs => scrs.StudentClass),
                orderBy: query => query.OrderBy(rs => rs.UpdateDate),
                pageIndex: pageIndex, pageSize: pageSize);

            var roomSubjectViewModels = roomSubjectsQuery.Items.Select(rs => new RoomSubjectsViewModel
            {
                Id = rs.Id,
                SubjectId = rs.SubjectId ?? 0,
                RoomId = rs.RoomId ?? 0,
                SchoolId = (int)rs.SchoolId,
                TermId = rs.TermId ?? 0,
                RoomSubjectCode = rs.RoomSubjectCode,
                RoomSubjectName = rs.RoomSubjectName,
                Model = (ERoomSubjectModel)rs.Model,
                StudentClass = rs.StudentClassRoomSubjects
                .Select(scrs => new StudentClassList
                {
                    Id = scrs.StudentClass.Id,
                    StudentClassName = scrs.StudentClass.Name
                }).ToList(),
                CreateDate = rs.CreateDate,
                UpdateDate = rs.UpdateDate,
                IsDeleted = rs.IsDeleted
            }).ToList();

            var paginationResult = new Pagination<RoomSubjectsViewModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemCount = roomSubjectsQuery.TotalItemCount,
                Items = roomSubjectViewModels
            };

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách lớp ghép thành công.",
                Result = paginationResult
            };
        }
        #endregion
    }
}
