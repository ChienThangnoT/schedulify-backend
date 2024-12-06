using AutoMapper;
using Google.OrTools.ConstraintSolver;
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

                    if (roomSubjectAddModel.StudentClassId.Distinct().Count() <=1)
                    {
                        throw new DefaultException("Số lượng lớp phải lớn hơn 1 lớp khi tạo lớp gộp");
                    }

                    //var getRoomSubjectBySubject = await _unitOfWork.RoomSubjectRepo.GetAsync(
                    //    filter: t => !t.IsDeleted && t.SubjectId == roomSubjectAddModel.SubjectId 
                    //    && t.TermId == roomSubjectAddModel.TermId 
                    //    && t.SchoolId == roomSubjectAddModel.SchoolId);

                    var existingGroupAssignments = await _unitOfWork.StudentClassRoomSubjectRepo.GetV2Async(
                        s => roomSubjectAddModel.StudentClassId.Contains(s.StudentClassId)
                           && s.RoomSubject.SubjectId == roomSubjectAddModel.SubjectId
                           && s.RoomSubject.TermId == roomSubjectAddModel.TermId
                           && s.RoomSubject.SchoolId == roomSubjectAddModel.SchoolId
                           && !s.IsDeleted,
                        include: t => t.Include(a => a.RoomSubject)
                                       .ThenInclude(r => r.Term)
                                       .Include(a => a.RoomSubject)
                                       .ThenInclude(r => r.School));

                    var existingStudentClassIds = existingGroupAssignments
                        .Select(s => s.StudentClassId)
                        .Distinct()
                        .ToList();

                    var conflictingStudentClassIds = roomSubjectAddModel.StudentClassId
                        .Intersect(existingStudentClassIds)
                        .ToList();

                    if (conflictingStudentClassIds.Count > 0)
                    {
                        var studentClass = await _unitOfWork.StudentClassesRepo.GetAsync(filter: t => conflictingStudentClassIds.Contains(t.Id));
                        throw new DefaultException($"Các lớp sau đã tham gia lớp ghép khác cho môn học này: {string.Join(", ", studentClass)}");
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
                        && !rs.IsDeleted && rs.SchoolId == roomSubjectAddModel.SchoolId);


                    if (roomSubjectCodeExists)
                    {
                        throw new DefaultException(ConstantResponse.ROOM_SUBJECT_NAME_OR_CODE_EXIST);
                    }

                    var curriDetail = await _unitOfWork.StudentClassesRepo.GetV2Async(filter: t=> t.Id == roomSubjectAddModel.StudentClassId.First(),
                        include: query => query.Include(t => t.StudentClassGroup).ThenInclude(u => u.Curriculum).ThenInclude(p => p.CurriculumDetails));

                    var slot = curriDetail.FirstOrDefault().StudentClassGroup.Curriculum.CurriculumDetails;
                   
                    var newRoomSubject = _mapper.Map<RoomSubject>(roomSubjectAddModel);
                    if (curriDetail.First().MainSession == (int)roomSubjectAddModel.Session)
                    {
                        if (slot.Any(c => c.SubjectId == roomSubjectAddModel.SubjectId && c.MainSlotPerWeek > 0))
                            newRoomSubject.SlotPerWeek = slot.First().MainSlotPerWeek;
                    }
                    else
                    {
                        if (slot.Any(c => c.SubjectId == roomSubjectAddModel.SubjectId && c.SubSlotPerWeek > 0))
                            newRoomSubject.SlotPerWeek = slot.First().SubSlotPerWeek;
                    }
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

                    var teacherAssignment = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                        filter: t => roomSubjectAddModel.StudentClassId.Contains(t.StudentClassId) 
                        && t.SubjectId == roomSubjectAddModel.SubjectId 
                        && t.TermId == roomSubjectAddModel.TermId 
                        && !t.IsDeleted);

                    foreach ( var assignment in teacherAssignment)
                    {
                        assignment.TeacherId = roomSubjectAddModel.TeacherId;
                        _unitOfWork.TeacherAssignmentRepo.Update(assignment);
                    }

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
            if (model.StudentClassIds.Distinct().Count() <= 1)
            {
                throw new DefaultException("Số lượng lớp phải lớn hơn 1 lớp khi tạo lớp gộp");
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

            var existingGroupAssignments = await _unitOfWork.StudentClassRoomSubjectRepo.GetAsync(
                s => model.StudentClassIds.Contains(s.StudentClassId)
                       && s.RoomSubject.SubjectId == model.SubjectId
                       && s.RoomSubject.TermId == model.TermId
                       && s.RoomSubject.SchoolId == schoolId
                       && s.RoomSubject.Id != id
                       && !s.IsDeleted);

            var existingStudentClassIds = existingGroupAssignments
                .Select(s => s.StudentClassId)
                .Distinct()
                .ToList();

            var conflictingStudentClassIds = model.StudentClassIds
                .Intersect(existingStudentClassIds)
                .ToList();

            if (conflictingStudentClassIds.Count > 0)
            {
                var studentClass = await _unitOfWork.StudentClassesRepo.GetAsync(filter: t => conflictingStudentClassIds.Contains(t.Id));
                throw new DefaultException($"Các lớp sau đã tham gia lớp ghép khác cho môn học này: {string.Join(", ", studentClass)}");
            }

            var roomSubjectExisted = await _unitOfWork.RoomSubjectRepo.GetByIdAsync(id, filter: f => !f.IsDeleted && f.SchoolId == schoolId, 
                include: query => query.Include(rs => rs.StudentClassRoomSubjects)) ??
                throw new NotExistsException(ConstantResponse.ROOM_SUBJECT_NOT_EXIST);
            if (!model.RoomSubjectName.Equals(roomSubjectExisted.RoomSubjectName, StringComparison.OrdinalIgnoreCase) 
                || !model.RoomSubjectCode.Equals(roomSubjectExisted.RoomSubjectCode, StringComparison.OrdinalIgnoreCase))
            {
                var roomSubjectCodeExists = await _unitOfWork.RoomSubjectRepo.ExistsAsync(rs => rs.Id != id &&
                ((!string.IsNullOrEmpty(rs.RoomSubjectCode) && rs.RoomSubjectCode.ToLower() == model.RoomSubjectCode.ToLower())
                || (!string.IsNullOrEmpty(rs.RoomSubjectName) && rs.RoomSubjectName.ToLower() == model.RoomSubjectName.ToLower()))
                && !rs.IsDeleted && rs.SchoolId == schoolId);


                if (roomSubjectCodeExists)
                {
                    throw new DefaultException(ConstantResponse.ROOM_SUBJECT_NAME_OR_CODE_EXIST);
                }
            }
            var scrs = roomSubjectExisted.StudentClassRoomSubjects.Where(s => !s.IsDeleted);
            var studentClassId = scrs.Select(s => s.StudentClassId);
            var teacherAssignment = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                        filter: t => studentClassId.Distinct().Contains(t.StudentClassId) 
                        && t.SubjectId == roomSubjectExisted.SubjectId  
                        && t.TermId == roomSubjectExisted.TermId
                        && !t.IsDeleted);

            if (model.TeacherId != null && model.TeacherId != teacherAssignment.First().TeacherId)
            {
                foreach( var oldAssignment in teacherAssignment)
                {
                    oldAssignment.TeacherId = null;
                    _unitOfWork.TeacherAssignmentRepo.Update(oldAssignment);
                }

                var newTeacherAssignment = await _unitOfWork.TeacherAssignmentRepo.GetAsync(
                        filter: t => model.StudentClassIds.Contains(t.StudentClassId) 
                        && t.SubjectId == model.SubjectId
                        && t.TermId == model.TermId
                        && !t.IsDeleted);

                foreach (var newAssignment in newTeacherAssignment)
                {
                    newAssignment.TeacherId = model.TeacherId;
                    _unitOfWork.TeacherAssignmentRepo.Update(newAssignment);
                }
                await _unitOfWork.SaveChangesAsync();
            }

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
                include: query => query.Include(a => a.Teacher).Include(t => t.StudentClassRoomSubjects).ThenInclude(scrs => scrs.StudentClass),
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
                EGrade = (EGrade)rs.Grade,
                StudentClass = rs.StudentClassRoomSubjects
                .Select(scrs => new StudentClassList
                {
                    Id = scrs.StudentClass.Id,
                    StudentClassName = scrs.StudentClass.Name
                }).ToList(),
                CreateDate = rs.CreateDate,
                UpdateDate = rs.UpdateDate,
                IsDeleted = rs.IsDeleted,
                TeacherId = rs.TeacherId,
                TeacherFirstName = rs.Teacher.FirstName,
                TeacherLastName = rs.Teacher.LastName,
                TeacherAbbreviation = rs.Teacher.Abbreviation,
                Session = (MainSession)rs.Session

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
