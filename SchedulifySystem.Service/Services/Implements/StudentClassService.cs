using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;

namespace SchedulifySystem.Service.Services.Implements
{
    public class StudentClassService : IStudentClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region CreateStudentClasses
        public async Task<BaseResponseModel> CreateStudentClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            var check = await CheckValidDataAddClasses(schoolId, schoolYearId, models);
            if (check.Status != StatusCodes.Status200OK)
            {
                return check;
            }

            var classes = _mapper.Map<List<StudentClass>>(models);
            await _unitOfWork.StudentClassesRepo.AddRangeAsync(classes);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.ADD_CLASS_SUCCESS };
        }
        #endregion

        #region Check
        public async Task<BaseResponseModel> CheckValidDataAddClasses(int schoolId, int schoolYearId, List<CreateListStudentClassModel> models)
        {
            var _ = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var __ = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            var ValidList = new List<CreateListStudentClassModel>();
            var errorList = new List<CreateListStudentClassModel>();

            //check duplicate name in list
            var duplicateNameRooms = models
             .GroupBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
             .Where(g => g.Count() > 1)
             .SelectMany(g => g)
             .ToList();

            if (duplicateNameRooms.Count != 0)
            {
                return new BaseResponseModel { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.CLASS_NAME_DUPLICATED, Result = duplicateNameRooms };
            }


            //check have teacher in db
            foreach (CreateListStudentClassModel model in models)
            {
                var found = await _unitOfWork.TeacherRepo.ToPaginationIncludeAsync(filter: t => t.SchoolId == schoolId && !t.IsDeleted && t.Abbreviation.ToLower().Equals(model.HomeroomTeacherAbbreviation.ToLower()));
                if (found.Items.Count == 0)
                {
                    errorList.Add(model);
                }
                else
                {
                    model.HomeroomTeacherId = found.Items.FirstOrDefault()?.Id;
                    model.SchoolId = schoolId;
                    model.SchoolYearId = schoolYearId;
                }
            }

            if (errorList.Count != 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.TEACHER_ABBREVIATION_NOT_EXIST,
                    Result = errorList
                };
            }


            //check teacher is assigned other class
            foreach (CreateListStudentClassModel model in models)
            {
                if (await _unitOfWork.StudentClassesRepo.ExistsAsync(
                    filter: c => !c.IsDeleted && c.SchoolId == schoolId &&
                    c.HomeroomTeacherId == model.HomeroomTeacherId)
                    )
                {
                    errorList.Add(model);
                }
            }

            if (errorList.Count != 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.HOMEROOM_TEACHER_ASSIGNED,
                    Result = errorList
                };
            }

            var roomCodes = models.Where(r =>!r.RoomCode.IsNullOrEmpty()).Select(c => c.RoomCode.ToLower());
            if(roomCodes.Any() )
            {
                var groupByRoomCode = models.GroupBy(c => c.RoomCode);
                var roomDb = await _unitOfWork.RoomRepo.GetV2Async(
                    filter: r => r.Building.SchoolId == schoolId && !r.IsDeleted && roomCodes.Contains(r.RoomCode.ToLower()),
                    include: query => query.Include(s => s.StudentClasses));

                foreach (var model in models)
                {
                    var room = roomDb.FirstOrDefault(r => r.RoomCode.ToLower().Equals(model.RoomCode.ToLower())) ??
                        throw new NotExistsException($"Không tìm thấy phòng mã {model.RoomCode}");
                    
                    model.RoomId = room.Id;
                }
            }
            
            // List of class names to check in the database
            var modelNames = models.Select(m => m.Name.ToLower()).ToList();
            
            // Check class duplicates in the database
            var foundClass = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(
                filter: sc => sc.SchoolId == schoolId && !sc.IsDeleted &&
                modelNames.Contains(sc.Name.ToLower()));

            errorList = _mapper.Map<List<CreateListStudentClassModel>>(foundClass.Items);
            ValidList = models.Where(m => !errorList.Any(e => e.Name.Equals(m.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            return errorList.Count != 0
                ? new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.CLASS_NAME_EXISTED,
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

        #region GetStudentClassById
        public async Task<BaseResponseModel> GetStudentClassById(int id)
        {
            var existedClass = (await _unitOfWork.StudentClassesRepo.GetV2Async(
                                    filter: t => t.Id == id && t.IsDeleted == false,
                                    include: query => query.Include(r => r.Room).Include(t => t.StudentClassGroup).ThenInclude(q => q.Curriculum)
                )).FirstOrDefault()
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CLASS_SUCCESS,
                Result = _mapper.Map<StudentClassViewModel>(existedClass)
            };
        }
        #endregion

        #region GetStudentClasses
        public async Task<BaseResponseModel> GetStudentClasses(int schoolId, EGrade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var studentClasses = await _unitOfWork.StudentClassesRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: sc => sc.SchoolId == schoolId && (includeDeleted ? true : sc.IsDeleted == false)
                                && (grade == null ? true : sc.Grade == (int)grade)
                                && (schoolYearId == null ? true : sc.SchoolYearId == schoolYearId),
                include: query => query.Include(r => r.Room).Include(sc => sc.Teacher).Include(sc => sc.StudentClassGroup).ThenInclude(cr => cr.Curriculum));

            if (studentClasses.Items.Count == 0)
            {
                throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            }

            var studentClassesViewModel = _mapper.Map<Pagination<StudentClassViewModel>>(studentClasses);

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CLASS_SUCCESS,
                Result = studentClassesViewModel
            };

        }
        #endregion

        #region DeleteStudentClass
        public async Task<BaseResponseModel> DeleteStudentClass(int id)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id) ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            existedClass.IsDeleted = true;
            _unitOfWork.StudentClassesRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_CLASS_SUCCESS };
        }
        #endregion

        #region UpdateStudentClass
        public async Task<BaseResponseModel> UpdateStudentClass(int id, UpdateStudentClassModel updateStudentClassModel)
        {
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);



            if (updateStudentClassModel.RoomId != null && updateStudentClassModel.RoomId != existedClass.RoomId)
            {
                var checkkExistRoom = await _unitOfWork.RoomRepo.GetByIdAsync((int)updateStudentClassModel.RoomId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);

                var cheeckRoomIsUse = await _unitOfWork.StudentClassesRepo.GetAsync(filter: t => t.IsDeleted == false && t.RoomId == updateStudentClassModel.RoomId);
                if (cheeckRoomIsUse != null)
                {
                    throw new DefaultException(ConstantResponse.ROOM_ALREADY_IN_USE);
                }
            }
            updateStudentClassModel.RoomId ??= existedClass.RoomId;

            if (updateStudentClassModel.HomeroomTeacherId != null && updateStudentClassModel.HomeroomTeacherId != existedClass.HomeroomTeacherId)
            {
                var checkkExistTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync((int)updateStudentClassModel.HomeroomTeacherId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);

            }
            updateStudentClassModel.HomeroomTeacherId ??= existedClass.HomeroomTeacherId;
            _mapper.Map(updateStudentClassModel, existedClass);
            _unitOfWork.StudentClassesRepo.Update(existedClass);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.UPDATE_CLASS_SUCCESS };
        }

        #endregion

        #region AssignHomeroomTeacherToClasses
        public async Task<BaseResponseModel> AssignHomeroomTeacherToClasses(AssignListStudentClassModel assignListStudentClassModel)
        {
            if (assignListStudentClassModel.HasDuplicateClassId())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.CLASS_ID_DUPLICATED };
            }

            if (assignListStudentClassModel.HasDuplicateTeacherId())
            {
                return new BaseResponseModel() { Status = StatusCodes.Status400BadRequest, Message = ConstantResponse.HOMEROOM_TEACHER_LIMIT, Result = assignListStudentClassModel.GetDuplicateAssigns() };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (AssignStudentClassModel assign in assignListStudentClassModel)
                    {
                        var existClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(assign.ClassId, filter: t => t.IsDeleted == false) ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);
                        var _ = await _unitOfWork.TeacherRepo.GetByIdAsync(assign.TeacherId, filter: t => t.IsDeleted == false) ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);
                        existClass.HomeroomTeacherId = assign.TeacherId;
                        existClass.UpdateDate = DateTime.UtcNow;
                    }
                    await _unitOfWork.SaveChangesAsync();
                    transaction.Commit();
                    return new BaseResponseModel() { Status = StatusCodes.Status200OK, Message = "Assign success!" };

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region Get Subject In Group Of Class
        public async Task<BaseResponseModel> GetSubjectInGroupOfClass(int schoolId, int schoolYearId, int studentClassId)
        {
            //var classesDb = await _unitOfWork.StudentClassesRepo.GetV2Async(
            //    filter: t => t.SchoolId == schoolId &&
            //                 t.Id == studentClassId &&
            //                 t.SchoolYearId == schoolYearId &&
            //                 t.IsDeleted == false,
            //    orderBy: q => q.OrderBy(s => s.Name),
            //    include: query => query.Include(c => c.StudentClassGroup)
            //               .ThenInclude(sg => sg.CurriculumDetails));

            //if (classesDb == null || !classesDb.Any())
            //{
            //    throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);
            //}

            //var classesDbList = classesDb.ToList();
            //var listSBInGroup = new List<CurriculumDetail>();
            //for (var i = 0; i < classesDbList.Count; i++)
            //{
            //    for (var j = 0; j < classesDbList[i].StudentClassGroup.CurriculumDetails.Count; j++)
            //    {
            //        listSBInGroup.Add(classesDbList[i].StudentClassGroup.CurriculumDetails.ToList()[j]);
            //    }
            //}
            //var result = _mapper.Map<List<SubjectInGroupViewModel>>(listSBInGroup);


            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_SUBJECT_IN_CLASS_SUCCESS
                //Result = result
            };
        }
        #endregion

        #region Get Teacher Assignment Of Class
        public async Task<BaseResponseModel> GetTeacherAssignmentOfClass(int studentClassId, int schoolYearId)
        {
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => t.IsDeleted == false)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            var studentClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(studentClassId, filter: t => t.IsDeleted == false && t.SchoolYearId == schoolYearId)
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);

            var teacherAssignment = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(filter: t => t.StudentClassId == studentClassId && t.IsDeleted == false,
                orderBy: o => o.OrderBy(q => q.TermId),
                include: query => query.Include(t => t.Subject).Include(q => q.Teacher).Include(u => u.Term)
                );
            if (teacherAssignment == null || !teacherAssignment.Any())
            {
                throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_HAVE_ASSIGNMENT);
            }

            var assignmentDictionary = teacherAssignment
                .GroupBy(a => a.SubjectId)
                .Select(group => new StudentClassAssignmentViewModel
                {
                    SubjectId = group.Key,
                    SubjectName = group.First().Subject?.SubjectName,
                    TotalSlotInYear = group.Sum(x => x.PeriodCount * (x.Term.EndWeek - x.Term.StartWeek + 1)),
                    AssignmentDetails = group.Select(a => new AssignmentDetail
                    {
                        TermId = a.Term.Id,
                        TermName = a.Term.Name,
                        TeacherId = a.TeacherId ?? 0,
                        TeacherFirstName = a.Teacher?.FirstName,
                        TeacherLastName = a.Teacher?.LastName,
                        TotalPeriod = a.PeriodCount,
                        StartWeek = a.Term.StartWeek,
                        EndWeek = a.Term.EndWeek
                    }).ToList()
                }).ToList();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_STUDENT_CLASS_ASSIGNMENT_SUCCESS,
                Result = assignmentDictionary
            };
        }
        #endregion
    }
}
