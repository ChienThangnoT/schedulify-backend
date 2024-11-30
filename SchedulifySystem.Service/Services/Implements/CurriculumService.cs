using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.CurriculumBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int REQUIRE_ELECTIVE_SUBJECT = 4;
        private const int REQUIRE_SPECIALIZED_SUBJECT = 3;
        public CurriculumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region create Curriculum
        public async Task<BaseResponseModel> CreateCurriculum(int schoolId, int schoolYearId, CurriculumAddModel curriculumAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var schoolData = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: s => !s.IsDeleted && s.Status == (int)SchoolStatus.Active)
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

                    var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: sy => !sy.IsDeleted, include: query => query.Include(sy => sy.Terms))
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

                    var checkExistCurriculum = await _unitOfWork.CurriculumRepo.GetAsync(t => !t.IsDeleted &&
                        t.SchoolId == schoolId && t.Grade == (int)curriculumAddModel.Grade &&
                        t.CurriculumName.ToLower() == curriculumAddModel.CurriculumName.ToLower() &&
                        schoolYearId == t.SchoolYearId && 
                        t.CurriculumCode.ToLower() == curriculumAddModel.CurriculumCode.ToLower());

                    if (checkExistCurriculum.Any())
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.CURRICULUM_NAME_OR_CODE_EXISTED
                        };

                    // check số lượng môn chuyên đề và tự chọn
                    if (curriculumAddModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT ||
                        curriculumAddModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    // add curriculum
                    var curriculum = _mapper.Map<Curriculum>(curriculumAddModel);
                    curriculum.SchoolId = schoolId;
                    curriculum.SchoolYearId = schoolYearId;
                    await _unitOfWork.CurriculumRepo.AddAsync(curriculum);
                    await _unitOfWork.SaveChangesAsync();

                    var termList = schoolYear.Terms.ToList();
                    var curriculumDetails = new List<CurriculumDetail>();

                    var subjects = await _unitOfWork.SubjectRepo.GetV2Async(filter: s => !s.IsDeleted);

                    var electiveSubjects = subjects.Where(s => curriculumAddModel.ElectiveSubjectIds.Contains(s.Id));

                    var invalidIds = curriculumAddModel.ElectiveSubjectIds
                        .Where(i => !subjects.Select(s => s.Id).Contains(i))
                        .ToList();

                    if (invalidIds.Count != 0)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = $"Không tìm thấy môn có Id: {string.Join(", ", invalidIds)}"
                        };
                    }

                    // add chuyen de
                    foreach (var subject in electiveSubjects)
                    {
                        if (subject.IsRequired)
                            return new BaseResponseModel() 
                            { 
                                Status = StatusCodes.Status400BadRequest,
                                Message = "Không thể chọn môn bắt buộc."
                            };

                        int slotPerTerm1 = (int)subject.TotalSlotInYear / 2;
                        int slotPerTerm2 = (int)subject.TotalSlotInYear - slotPerTerm1;

                        curriculumDetails.AddRange(termList.Select((term, index) => new CurriculumDetail
                        {
                            SubjectId = subject.Id,
                            CurriculumId = curriculum.Id,
                            MainSlotPerWeek = (int)subject.TotalSlotInYear / 35,
                            SlotPerTerm = (term.EndWeek == 18) ? slotPerTerm1 : slotPerTerm2,
                            TermId = term.Id,
                            CreateDate = DateTime.UtcNow
                        }));
                    }

                    // add required
                    var requireSubjects = subjects.Where(s => s.IsRequired).ToList();
                    foreach(var subject in requireSubjects)
                    {
                        int slotPerTerm1 = (int)subject.TotalSlotInYear / 2;
                        int slotPerTerm2 = (int)subject.TotalSlotInYear - slotPerTerm1;

                        curriculumDetails.AddRange(termList.Select((term, index) => new CurriculumDetail
                        {
                            SubjectId = subject.Id,
                            CurriculumId = curriculum.Id,
                            MainSlotPerWeek = (int)subject.TotalSlotInYear / 35,
                            SlotPerTerm = (term.EndWeek == 18) ? slotPerTerm1 : slotPerTerm2,
                            TermId = term.Id,
                            CreateDate = DateTime.UtcNow
                        }));
                    }

                    // thêm chuyên đề
                    var specializedSubjects = curriculumDetails.Where(s => curriculumAddModel.SpecializedSubjectIds.Contains(s.SubjectId));
                    foreach(var subject in specializedSubjects)
                    {
                        subject.SlotPerTerm++;
                        subject.MainSlotPerWeek++;
                        subject.IsSpecialized = true;
                    }

                    await _unitOfWork.CurriculumDetailRepo.AddRangeAsync(curriculumDetails);
                    await _unitOfWork.SaveChangesAsync();

                    transaction.Commit();
                    return new BaseResponseModel
                    {
                        Status = StatusCodes.Status201Created,
                        Message = ConstantResponse.ADD_CURRICULUM_SUCCESS,
                        Result = _mapper.Map<CurriculumViewModel>(curriculum)
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

        #region Get Curriculum Detail
        public async Task<BaseResponseModel> GetCurriculumDetails(int schoolId, int yearId,int curriculumId)
        {
            var curriculum = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId,
                filter: c =>  !c.IsDeleted && schoolId == c.SchoolId && yearId == c.SchoolYearId,
                include: query => query.Include(c => c.StudentClassGroups).Include(c => c.CurriculumDetails)
                           .ThenInclude(cd => cd.Subject)) ??
                           throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);

            var result = _mapper.Map<CurriculumViewDetailModel>(curriculum);
            result.StudentClassGroupViewNames = curriculum.StudentClassGroups.Where(cg => !cg.IsDeleted).Select(c => c.GroupName).ToList();

            var subjectViewData = _mapper.Map<List<SubjectViewDetailModel>>(curriculum.CurriculumDetails);
            result.SubjectSelectiveViews = subjectViewData.Where(s => !s.IsRequired).ToList();
            result.SubjectSpecializedtViews = subjectViewData.Where(s => s.IsSpecialized).ToList();
            result.SubjectRequiredViews = subjectViewData.Where(s => s.IsRequired).ToList();

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CURRICULUM_DETAIL_SUCCESS,
                Result = result
            };
        }


        #endregion

        #region get Curriculums
        public async Task<BaseResponseModel> GetCurriculums(int schoolId, EGrade? grade, int schoolYearId, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => !t.IsDeleted && t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: t => !t.IsDeleted)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            var curriculum = await _unitOfWork.CurriculumRepo.GetPaginationAsync(
                filter: t => t.SchoolId == schoolId
                && (grade == null || t.Grade == (int)grade)
                && (t.SchoolYearId == schoolYearId)
                && !t.IsDeleted ,
                includeProperties: "School",
                orderBy: q => q.OrderBy(s => s.CurriculumName),
                pageIndex: pageIndex,
                pageSize: pageSize
                );
            var result = _mapper.Map<Pagination<CurriculumViewModel>>(curriculum);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CURRICULUM_LIST_SUCCESS,
                Result = result.Items.Count == 0 ? null : result
            };
        }
        #endregion

        #region Update Curriculum
        public async Task<BaseResponseModel> UpdateCurriculum(int schoolId, int schoolYearId, int curriculumId, CurriculumUpdateModel curriculumUpdateModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var CurriculumDb = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId,
                    filter: t => !t.IsDeleted && t.SchoolId == schoolId && t.SchoolYearId == schoolYearId,
                    include: query => query.Include(c => c.CurriculumDetails)
                               .ThenInclude(sg => sg.Subject)
                               .Include(sg => sg.School)
                               .Include(sg => sg.SchoolYear)
                               .ThenInclude(sy => sy.Terms)
                               .Include(sg => sg.StudentClassGroups.Where(scg => !scg.IsDeleted)))?? 
                               throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);

                    var termInYear = await _unitOfWork.TermRepo.GetAsync(filter: t => t.SchoolYearId == schoolYearId && !t.IsDeleted)
                        ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);

                    List<Term> termList = termInYear.ToList();

                    bool isNameChanged = !string.Equals(CurriculumDb.CurriculumName, curriculumUpdateModel.CurriculumName, StringComparison.OrdinalIgnoreCase);

                    if (isNameChanged)
                    {
                        var checkExistCurriculum = await _unitOfWork.CurriculumRepo.GetAsync(
                            filter: t => (t.CurriculumName.ToLower() == curriculumUpdateModel.CurriculumName.ToLower()
                                          &&
                                          t.Id != curriculumId && !t.IsDeleted && t.SchoolId == schoolId
                                          && schoolYearId == t.SchoolYearId
                        ));

                        if (checkExistCurriculum.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.CURRICULUM_NAME_EXISTED
                            };
                        }
                    }

                    bool isCodeChanged = !string.Equals(CurriculumDb.CurriculumCode, curriculumUpdateModel.CurriculumCode, StringComparison.OrdinalIgnoreCase);

                    if (isCodeChanged)
                    {
                        var checkExistCurriculum = await _unitOfWork.CurriculumRepo.GetAsync(
                            filter: t => (t.CurriculumCode.ToLower() == curriculumUpdateModel.CurriculumCode.ToLower()
                                          &&
                                          t.Id != curriculumId && !t.IsDeleted && t.SchoolId == schoolId
                                          && schoolYearId == t.SchoolYearId
                        ));

                        if (checkExistCurriculum.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.CURRICULUM_CODE_EXISTED
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(curriculumUpdateModel.CurriculumCode))
                    {
                        CurriculumDb.CurriculumCode = curriculumUpdateModel.CurriculumCode.Trim();
                    }


                    // check enough number subject
                    if (curriculumUpdateModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT
                        || curriculumUpdateModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    // ko update grade nếu đã được sử dụng bởi lớp nào
                    if (curriculumUpdateModel.Grade != 0 && curriculumUpdateModel.Grade != (EGrade)CurriculumDb.Grade)
                    {
                        var classGroups = await _unitOfWork.StudentClassGroupRepo.GetAsync(
                            filter: t => t.Grade == (int)curriculumUpdateModel.Grade && t.CurriculumId == curriculumId && !t.IsDeleted);

                        if (classGroups.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.CURRICULUM_HAS_SUBJECTS_REGISTERED
                            };
                        }

                        CurriculumDb.Grade = (int)curriculumUpdateModel.Grade;
                    }

                    if (!string.IsNullOrEmpty(curriculumUpdateModel.CurriculumName))
                    {
                        CurriculumDb.CurriculumName = curriculumUpdateModel.CurriculumName.Trim();
                    }

                    var subjects = await _unitOfWork.SubjectRepo.GetV2Async(filter: s => !s.IsDeleted);

                    var electiveSubjects = subjects.Where(s => curriculumUpdateModel.ElectiveSubjectIds.Contains(s.Id));

                    var invalidIds = curriculumUpdateModel.ElectiveSubjectIds
                        .Where(i => !subjects.Select(s => s.Id).Contains(i))
                        .ToList();

                    if (invalidIds.Count != 0)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = $"Không tìm thấy môn có Id: {string.Join(", ", invalidIds)}"
                        };
                    }

                    // get list môn học hiện tại trong tổ hợp
                    var oldElectiveSubjectIds = CurriculumDb.CurriculumDetails
                        .Where(query => !query.Subject.IsRequired).Select(s => s.SubjectId).ToList();
                        
                    var subjectsToRemove = oldElectiveSubjectIds
                        .Except(curriculumUpdateModel.ElectiveSubjectIds)
                        .ToList();

                    var newElectiveSubjects = electiveSubjects.Where(s => !subjectsToRemove.Contains(s.Id) && !oldElectiveSubjectIds.Contains(s.Id));
                    // add chuyen de
                    foreach (var subject in newElectiveSubjects)
                    {
                        if (subject.IsRequired)
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = "Không thể chọn môn bắt buộc."
                            };

                        int slotPerTerm1 = (int)subject.TotalSlotInYear / 2;
                        int slotPerTerm2 = (int)subject.TotalSlotInYear - slotPerTerm1;

                        var newCurriculumDetails = termList.Select((term, index) => new CurriculumDetail
                        {
                            SubjectId = subject.Id,
                            CurriculumId = curriculumId,
                            MainSlotPerWeek = (int)subject.TotalSlotInYear / 35,
                            SlotPerTerm = (term.EndWeek == 18) ? slotPerTerm1 : slotPerTerm2,
                            TermId = term.Id,
                            CreateDate = DateTime.UtcNow
                        }).ToList();

                        await _unitOfWork.CurriculumDetailRepo.AddRangeAsync(newCurriculumDetails);
                    }


                    if (subjectsToRemove.Any()) 
                    {
                        var subjectsToRemoveEntities = CurriculumDb.CurriculumDetails
                            .Where(s => subjectsToRemove.Contains(s.SubjectId))
                            .ToList();
                        if (subjectsToRemoveEntities.Any())
                        {
                            _unitOfWork.CurriculumDetailRepo.RemoveRange(subjectsToRemoveEntities);
                        }

                    }
                    // cập nhật môn chuyên đề
                    var oldSpecializeSubjects = CurriculumDb.CurriculumDetails.Where(cd => cd.IsSpecialized).ToList();
                    var newSpecializeSubjects = CurriculumDb.CurriculumDetails.Where(cd => curriculumUpdateModel.SpecializedSubjectIds.Contains(cd.SubjectId) && !cd.IsSpecialized);
                    var removeSpecialzeSubjects = oldSpecializeSubjects.Where(cd => !curriculumUpdateModel.SpecializedSubjectIds.Contains(cd.SubjectId));
                    
                    foreach (var subject in removeSpecialzeSubjects)
                    {
                        subject.IsSpecialized = false;
                        if (subject.MainSlotPerWeek > 0) { subject.MainSlotPerWeek--; } 
                        else if(subject.SubSlotPerWeek > 0) subject.SubSlotPerWeek--;
                        subject.SlotPerTerm--;
                    }

                    foreach (var subject in newSpecializeSubjects) 
                    { 
                        subject.IsSpecialized = true;
                        if (subject.MainSlotPerWeek > 0) { subject.MainSlotPerWeek++; }
                        else if (subject.SubSlotPerWeek > 0) subject.SubSlotPerWeek++;
                        subject.SlotPerTerm++;
                    }
                    await _unitOfWork.SaveChangesAsync();
                    // update assignment 
                    await UpdateToAssignment(curriculumId);
                    transaction.Commit();
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status200OK,
                        Message = ConstantResponse.UPDATE_CURRICULUM_SUCCESS,
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

        private async Task UpdateToAssignment(int curriculumId)
        {
            var curriculumDb = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId, include: query => 
            query.Include(c => c.StudentClassGroups.Where(c => !c.IsDeleted))
            .ThenInclude(c => c.StudentClasses.Where(c => !c.IsDeleted))
            .Include(c => c.CurriculumDetails));
            var classes = curriculumDb.StudentClassGroups.SelectMany(s => s.StudentClasses);
            var classIds = classes.Select(s => s.Id).ToList();
            if (classIds.Any())
            {
                var curriculumDetails = curriculumDb.CurriculumDetails.Where(c => !c.IsDeleted).ToList();
                var curriculumDetailSubjectIds = curriculumDetails.Select(c => c.SubjectId).Distinct().ToList();
                var oldAssignments = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                filter: ta => classIds.Contains(ta.StudentClassId) && !ta.IsDeleted);

                if (oldAssignments.Any())
                {
                    // remove
                    var assignmentToRemove = oldAssignments.Where(a => !curriculumDetailSubjectIds.Contains(a.SubjectId));
                    if(assignmentToRemove.Any()) 
                        _unitOfWork.TeacherAssignmentRepo.RemoveRange(assignmentToRemove);
                    
                    // update
                    var assignmentToUpdate = oldAssignments.Where(a => curriculumDetailSubjectIds.Contains(a.SubjectId));
                    foreach (var assignment in assignmentToUpdate)
                    {
                        var founded = curriculumDetails.Find(c => c.SubjectId == assignment.SubjectId && c.TermId == assignment.TermId);
                        assignment.PeriodCount = founded.SubSlotPerWeek + founded.MainSlotPerWeek;
                        _unitOfWork.TeacherAssignmentRepo.Update(assignment);
                    }
                    // add
                    var curriculumDetailsToAdd = curriculumDetails.Where(c => !oldAssignments.Select(a => a.SubjectId).Contains(c.SubjectId));
                    var newAssignments = new List<TeacherAssignment>();
                    foreach (var sClass in classes)
                    {
                        foreach (var sig in curriculumDetailsToAdd)
                        {
                            newAssignments.Add(new TeacherAssignment
                            {
                                AssignmentType = (int)AssignmentType.Permanent,
                                PeriodCount = sig.MainSlotPerWeek + sig.SubSlotPerWeek,
                                StudentClassId = sClass.Id,
                                CreateDate = DateTime.UtcNow,
                                SubjectId = sig.SubjectId,
                                TermId = (int)sig.TermId,
                                TeacherId = sig.Subject.IsTeachedByHomeroomTeacher ? sClass.HomeroomTeacherId : null,
                            });
                        }
                    }
                    if(newAssignments.Any())
                    {
                        await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignments);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        #region DeleteCurriculum
        public async Task<BaseResponseModel> DeleteCurriculum(int schoolId, int yearId, int curriculumId)
        {
            var curriculum = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId,filter: c => c.SchoolId == schoolId && yearId == c.SchoolYearId,
                include: query => query.Include(c => c.StudentClassGroups).Include(sg => sg.CurriculumDetails)) ?? throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);
            if(curriculum.StudentClassGroups.Where(scg => !scg.IsDeleted).Count() > 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không thể xóa do có nhóm lớp đang sử dụng trương trình học!."
                };
            }
            curriculum.IsDeleted = true;

            _unitOfWork.CurriculumRepo.Update(curriculum);
            _unitOfWork.CurriculumDetailRepo.RemoveRange(curriculum.CurriculumDetails);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel { Status = StatusCodes.Status200OK, Message = ConstantResponse.DELETE_CURRICULUM_SUCCESS };
        }
        #endregion

        #region QuickAssignPeriod
        public async Task<BaseResponseModel> QuickAssignPeriod(int schoolId, int schoolYearId, QuickAssignPeriodModel model)
        {

            // check valid subject assignment config
            var invalidSubjects = model.SubjectAssignmentConfigs.Where(s => !s.CheckValid());
            if (invalidSubjects.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Subject config is not correct!",
                    Result = invalidSubjects
                };
            }


            var CurriculumDbs = await _unitOfWork.CurriculumRepo.GetV2Async(
                filter: sg => sg.SchoolId == schoolId && !sg.IsDeleted &&
                model.CurriculumApplyIds.Contains(sg.Id) && sg.SchoolYearId == schoolYearId,
                include: query => query.Include(sg => sg.CurriculumDetails));

            var subjectDbs = await _unitOfWork.SubjectRepo.GetV2Async(
                filter: s => !s.IsDeleted);

            // check valid Curriculum id
            var invalidCurriculumIds = model.CurriculumApplyIds.Where(i => !CurriculumDbs.Select(s => s.Id).Contains(i));

            if (invalidCurriculumIds.Any())
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.CURRICULUM_NOT_EXISTED,
                    Result = invalidSubjects
                };
            }

            // insert data
            var subjectInGroups = CurriculumDbs.SelectMany(sg => sg.CurriculumDetails);
            foreach (var subjectAssignment in model.SubjectAssignmentConfigs)
            {
                var filtered = subjectInGroups.Where(sig => sig.SubjectId == subjectAssignment.SubjectId &&
                sig.TermId == subjectAssignment.TermId);

                foreach (var sig in filtered)
                {
                    var extraSlots = sig.IsSpecialized ? subjectDbs.First(s => s.Id == sig.SubjectId).SlotSpecialized / 35 : 0;
                    sig.MainSlotPerWeek = subjectAssignment.MainSlotPerWeek + (int)extraSlots;
                    sig.SubSlotPerWeek = subjectAssignment.SubSlotPerWeek;
                    sig.MainMinimumCouple = subjectAssignment.MainMinimumCouple;
                    sig.SubMinimumCouple = subjectAssignment.SubMinimumCouple;
                    sig.SlotPerTerm = subjectAssignment.SlotPerTerm;
                    sig.IsDoublePeriod = subjectAssignment.IsDoublePeriod;
                    _unitOfWork.CurriculumDetailRepo.Update(sig);
                }

            }
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Quick assign success!"
            };

        }

        public async Task<BaseResponseModel> GetQuickAssignPeriodData(int schoolId, int schoolYearId)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ??
                throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);

            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId, filter: sy => !sy.IsDeleted,
                include: query => query.Include(sy => sy.Terms)) ??
                throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            var data = new List<SubjectAssignmentConfig>();

            var subjects = await _unitOfWork.SubjectRepo.GetV2Async(filter: s => !s.IsDeleted);

            foreach (var subject in subjects)
            {
                var termRange = 17;
                var slotPerWeek = subject.TotalSlotInYear / 35;
                var remainder = subject.TotalSlotInYear % 35;
                foreach (var term in schoolYear.Terms.Where(s => !s.IsDeleted).OrderBy(t => t.Id))
                {
                    var extraSlotInTerm = remainder > 0 ? remainder / termRange : 0;

                    data.Add(new SubjectAssignmentConfig()
                    {
                        SubjectId = subject.Id,
                        SubjectName = subject.SubjectName ?? "",
                        SubjectAbbreviation = subject.Abbreviation ?? "",
                        IsDoublePeriod = false,
                        MainMinimumCouple = 0,
                        SubMinimumCouple = 0,
                        MainSlotPerWeek = (int)(slotPerWeek + extraSlotInTerm),
                        SubSlotPerWeek = 0,
                        SlotPerTerm = (int)(slotPerWeek + extraSlotInTerm) * termRange,
                        TermId = term.Id,
                        TermName = term.Name ?? ""
                    });
                    termRange++;
                }
            }

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy data mẫu thành công!",
                Result = data.OrderBy(d => d.SubjectName)
            };

        }
        #endregion
    }
}
