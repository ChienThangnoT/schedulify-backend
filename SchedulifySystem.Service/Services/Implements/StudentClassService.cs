using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private const int ROOT = 0;

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
            for (int i = 0; i < classes.Count; i++)
            {
                classes[i].StudentClassGroupId = models[i].SGroupId;
            }
            var studentClassGroup = classes.GroupBy(s => s.StudentClassGroupId);
            foreach (var group in studentClassGroup)
            {
                if (group.Key == null) continue;

                var sg = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync((int)group.Key,
                                filter: t => t.IsDeleted == false,
                                include: query => query.Include(sg => sg.Curriculum).ThenInclude(c => c.CurriculumDetails));

                foreach (var item in group.Select(g => g))
                {
                    var newAssignment = new List<TeacherAssignment>();
                    sg.Curriculum.CurriculumDetails.ToList().ForEach(sig =>
                    {
                        newAssignment.Add(new TeacherAssignment()
                        {
                            AssignmentType = (int)AssignmentType.Permanent,
                            PeriodCount = sig.MainSlotPerWeek + sig.SubSlotPerWeek,
                            StudentClassId = item.Id,
                            CreateDate = DateTime.UtcNow,
                            SubjectId = sig.SubjectId,
                            TermId = (int)sig.TermId
                        });
                    });

                    item.PeriodCount = sg.Curriculum.CurriculumDetails.ToList().Sum(q => q.MainSlotPerWeek + q.SubSlotPerWeek);
                    _unitOfWork.StudentClassesRepo.Update(item);
                    await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignment);
                }
            }

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

            // check add student class group
            var classesWithGroup = models.Where(s => s.StudentClassGroupCode != null);
            var groupCodes = classesWithGroup.Select(s => s.StudentClassGroupCode.ToLower()).Distinct().ToList();
            var studentClassGroups = await _unitOfWork.StudentClassGroupRepo.GetV2Async(
                filter: sg => sg.SchoolId == schoolId && !sg.IsDeleted && groupCodes.Contains(sg.StudentClassGroupCode.ToLower()));

            foreach (var model in classesWithGroup)
            {
                var sg = studentClassGroups.FirstOrDefault(sg => sg.StudentClassGroupCode.ToLower().Equals(model.StudentClassGroupCode.ToLower()));
                if (sg != null)
                {
                    model.SGroupId = sg.Id;
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
            var existedClass = await _unitOfWork.StudentClassesRepo.GetByIdAsync(id, filter: t => t.IsDeleted == false,
                include: query => query.Include(c => c.TeacherAssignments))
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_NOT_EXIST);



            if (updateStudentClassModel.RoomId != null && updateStudentClassModel.RoomId != existedClass.RoomId)
            {
                var checkkExistRoom = await _unitOfWork.RoomRepo.GetByIdAsync(id, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.ROOM_NOT_EXIST);
            }
            updateStudentClassModel.RoomId ??= existedClass.RoomId;

            if (updateStudentClassModel.HomeroomTeacherId != null && updateStudentClassModel.HomeroomTeacherId != existedClass.HomeroomTeacherId)
            {
                var checkkExistTeacher = await _unitOfWork.TeacherRepo.GetByIdAsync((int)updateStudentClassModel.HomeroomTeacherId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.TEACHER_NOT_EXIST);

            }
            updateStudentClassModel.HomeroomTeacherId ??= existedClass.HomeroomTeacherId;

            if (updateStudentClassModel.StudentClassGroupId == null)
            {
                _unitOfWork.TeacherAssignmentRepo.RemoveRange(existedClass.TeacherAssignments);
            }
            else if (existedClass.StudentClassGroupId != updateStudentClassModel.StudentClassGroupId)
            {
                var studentClassGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync((int)updateStudentClassModel.StudentClassGroupId,
                               filter: t => t.IsDeleted == false,
                               include: query => query.Include(sg => sg.Curriculum).ThenInclude(cd => cd.CurriculumDetails))
                               ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND);

                // delete old assignment
                var oldAssignment = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(filter: ta => ta.StudentClassId == id);
                _unitOfWork.TeacherAssignmentRepo.RemoveRange(oldAssignment);

                // add new assignment 
                var newAssignment = new List<TeacherAssignment>();
                studentClassGroup.Curriculum.CurriculumDetails.ToList().ForEach(sig =>
                {
                    newAssignment.Add(new TeacherAssignment()
                    {
                        AssignmentType = (int)AssignmentType.Permanent,
                        PeriodCount = sig.MainSlotPerWeek + sig.SubSlotPerWeek,
                        StudentClassId = id,
                        CreateDate = DateTime.UtcNow,
                        SubjectId = sig.SubjectId,
                        TermId = (int)sig.TermId
                    });
                });
                await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignment);
            }

            if (updateStudentClassModel.Grade != null && (int)updateStudentClassModel.Grade != existedClass.Grade)
            {
                if(updateStudentClassModel.StudentClassGroupId != null && updateStudentClassModel.StudentClassGroupId == existedClass.StudentClassGroupId)
                {
                    var studentClassGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync((int)updateStudentClassModel.StudentClassGroupId,
                               filter: t => t.IsDeleted == false);
                    if(studentClassGroup.Grade != (int)updateStudentClassModel.Grade)
                    {
                        throw new DefaultException(ConstantResponse.INVALID_UPDATE_GRADE_DIFFERENT_STUDENT_CLASS_GROUP);
                    }
                }else if(updateStudentClassModel.StudentClassGroupId != null && updateStudentClassModel.StudentClassGroupId != existedClass.StudentClassGroupId)
                {
                    var studentClassGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync((int)updateStudentClassModel.StudentClassGroupId,
                               filter: t => t.IsDeleted == false);
                    if (studentClassGroup.Grade != (int)updateStudentClassModel.Grade)
                    {
                        throw new DefaultException(ConstantResponse.INVALID_UPDATE_GRADE_DIFFERENT_STUDENT_CLASS_GROUP);
                    }
                }
            }

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

        #region AssignSubjectGroupToClasses
        public async Task<BaseResponseModel> AssignSubjectGroupToClasses(AssignSubjectGroup model)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var studentClassGroup= await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(model.StudentClassGroupId,
                                filter: t => t.IsDeleted == false,
                                include: query => query.Include(sg => sg.Curriculum).ThenInclude(cd => cd.CurriculumDetails))
                                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_GROUP_NOT_EXIST);

                    foreach (var classId in model.ClassIds)
                    {
                        var founded = await _unitOfWork.StudentClassesRepo.GetByIdAsync(classId, filter: t => t.IsDeleted == false)
                                        ?? throw new NotExistsException(ConstantResponse.CLASS_NOT_EXIST);

                        var classPeriodCount = 0;

                        if (founded.StudentClassGroupId == null || founded.StudentClassGroupId != model.StudentClassGroupId)
                        {
                            founded.StudentClassGroupId = model.StudentClassGroupId;
                            founded.UpdateDate = DateTime.UtcNow;

                            // delete old assignment
                            var oldAssignment = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(filter: ta => ta.StudentClassId == classId && ta.IsDeleted == false);
                            _unitOfWork.TeacherAssignmentRepo.RemoveRange(oldAssignment);

                            // add new assignment 
                            var newAssignment = new List<TeacherAssignment>();
                            studentClassGroup.Curriculum.CurriculumDetails.ToList().ForEach(sig =>
                            {
                                newAssignment.Add(new TeacherAssignment()
                                {
                                    AssignmentType = (int)AssignmentType.Permanent,
                                    PeriodCount = sig.MainSlotPerWeek + sig.SubSlotPerWeek,
                                    StudentClassId = classId,
                                    CreateDate = DateTime.UtcNow,
                                    SubjectId = sig.SubjectId,
                                    TermId = (int)sig.TermId
                                });
                                classPeriodCount += sig.MainSlotPerWeek + sig.SubSlotPerWeek;
                            });
                            await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignment);
                            _unitOfWork.StudentClassesRepo.Update(founded);
                        }

                        founded.PeriodCount = classPeriodCount;
                        _unitOfWork.StudentClassesRepo.Update(founded);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    transaction.Commit();
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK
                        //Message = ConstantResponse.SUBJECT_GROUP_ASSIGN_SUCCESS
                    };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region
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
