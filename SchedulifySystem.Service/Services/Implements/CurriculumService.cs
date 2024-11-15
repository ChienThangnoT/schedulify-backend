using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        public async Task<BaseResponseModel> CreateCurriculum(int schoolId, CurriculumAddModel CurriculumAddModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // check exist 
                    var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                        ?? throw new NotExistsException($"School not found with id {schoolId}");
                    var schoolYear = (await _unitOfWork.SchoolYearRepo.ToPaginationIncludeAsync(
                        filter: sy => sy.Id == CurriculumAddModel.SchoolYearId && sy.IsDeleted == false,
                        include: query => query.Include(sy => sy.Terms))).Items.FirstOrDefault()
                        ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

                    var termInYear = await _unitOfWork.TermRepo.GetAsync(filter: t => t.SchoolYearId == schoolYear.Id && t.IsDeleted == false)
                        ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
                    List<Term> termList = termInYear.ToList();
                    var checkExistCurriculum = await _unitOfWork.CurriculumRepo.GetAsync(
                        filter: t => t.SchoolId == schoolId && t.Grade == (int)CurriculumAddModel.Grade
                        && (t.CurriculumName.ToLower() == CurriculumAddModel.CurriculumName.ToLower())
                        );

                    if (checkExistCurriculum.Any())
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.CURRICULUM_NAME_OR_CODE_EXISTED
                        };
                    }
                    // check enough number subject
                    if (CurriculumAddModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT
                        || CurriculumAddModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    var CurriculumAdd = _mapper.Map<Curriculum>(CurriculumAddModel);
                    CurriculumAdd.SchoolId = schoolId;
                    await _unitOfWork.CurriculumRepo.AddAsync(CurriculumAdd);
                    //save to have group id
                    await _unitOfWork.SaveChangesAsync();
                    //generate data subject

                    //ElectiveSubjects
                    var newSubjectInGroupAdded = new List<CurriculumDetail>();
                    foreach (int subjectId in CurriculumAddModel.ElectiveSubjectIds)
                    {
                        // check subject existed and not belong to require subject
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);
                        if (checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.REQUIRE_ELECTIVE_SUBJECT,
                                Result = subjectId
                            };

                        // cal slot per term

                        int slotPerTerm1 = (int)Math.Floor((double)checkSubject.TotalSlotInYear / 2);
                        int slotPerTerm2 = (int)Math.Ceiling((double)checkSubject.TotalSlotInYear / 2);

                        var newSubjectInGroup = new CurriculumDetail
                        {
                            SubjectId = subjectId,
                            CurriculumId = CurriculumAdd.Id,
                            MainSlotPerWeek = (checkSubject?.TotalSlotInYear / 35) ?? 0,
                            IsSpecialized = CurriculumAddModel.SpecializedSubjectIds.Contains(subjectId)
                        };

                        for (int i = 0; i < termList.Count; i++)
                        {
                            newSubjectInGroup.CreateDate = DateTime.UtcNow;
                            newSubjectInGroup.TermId = termList[i].Id;
                            newSubjectInGroup.SlotPerTerm = (i == 0) ? slotPerTerm1 : slotPerTerm2;
                            _unitOfWork.CurriculumDetailRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                            newSubjectInGroupAdded.Add(newSubjectInGroup);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    //add required subject
                    var requiredSubjects = await _unitOfWork.SubjectRepo.GetAsync(
                        filter: t => t.IsDeleted == false && t.IsRequired == true);
                    //var requiredSubjectList = new List<SubjectInGroup>();
                    foreach (var subject in requiredSubjects)
                    {
                        int slotPerTerm1 = (int)Math.Floor((double)subject.TotalSlotInYear / 2);
                        int slotPerTerm2 = (int)Math.Ceiling((double)subject.TotalSlotInYear / 2);

                        var newSubjectRequiredInGroup = new CurriculumDetail
                        {
                            SubjectId = subject.Id,
                            CurriculumId = CurriculumAdd.Id,
                            MainSlotPerWeek = (subject?.TotalSlotInYear / 35) ?? 0,
                            IsSpecialized = CurriculumAddModel.SpecializedSubjectIds.Contains(subject.Id)
                        };

                        for (int i = 0; i < termList.Count; i++)
                        {
                            newSubjectRequiredInGroup.CreateDate = DateTime.UtcNow;
                            newSubjectRequiredInGroup.TermId = termList[i].Id;
                            newSubjectRequiredInGroup.SlotPerTerm = (i == 0) ? slotPerTerm1 : slotPerTerm2;
                            _unitOfWork.CurriculumDetailRepo.AddAsync(newSubjectRequiredInGroup.ShallowCopy());
                            newSubjectInGroupAdded.Add(newSubjectRequiredInGroup);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    //Specialized Subject
                    var specializedSubjectNotInElectiveSubjects = CurriculumAddModel.SpecializedSubjectIds
                        .Where(s => !CurriculumAddModel.ElectiveSubjectIds.Contains(s));

                    foreach (int subjectId in specializedSubjectNotInElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (!checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.INVALID_SPECIALIZED_SUBJECT,
                                Result = subjectId
                            };
                    }

                    //var subjectInGroupAdded = newSubjectInGroupAdded.FirstOrDefault(s => CurriculumAddModel.SpecializedSubjectIds.Contains(s.SubjectId));
                    transaction.Commit();
                    //var subjectInGroupAdded = newSubjectInGroupAdded
                    //    .Where(c => CurriculumAddModel.SpecializedSubjectIds.Contains(c.SubjectId)).ToList();

                    var updateSpecialSubject = await _unitOfWork.CurriculumDetailRepo.GetV2Async(
                        filter: t => t.CurriculumId == CurriculumAdd.Id && CurriculumAddModel.SpecializedSubjectIds.Contains(t.SubjectId)
                        , include: query => query.Include(i => i.Subject));

                    foreach (var subjectInGroup in updateSpecialSubject)
                    {
                        subjectInGroup.IsSpecialized = true;
                        subjectInGroup.MainSlotPerWeek += (subjectInGroup.Subject?.SlotSpecialized / 35) ?? 0;
                        _unitOfWork.CurriculumDetailRepo.Update(subjectInGroup);
                    }
                    await _unitOfWork.SaveChangesAsync();
                    var result = _mapper.Map<BusinessModels.CurriculumBusinessModels.CurriculumDetailViewModel>(CurriculumAdd);
                    return new BaseResponseModel()
                    {
                        Status = StatusCodes.Status201Created,
                        Message = ConstantResponse.ADD_CURRICULUM_SUCCESS,
                        Result = result
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
        public async Task<BaseResponseModel> GetCurriculumDetails(int curriculumId)
        {
            var curriculum = await _unitOfWork.CurriculumRepo.GetV2Async(
                filter: t => t.Id == curriculumId && t.IsDeleted == false,
                include: query => query.Include(c => c.CurriculumDetails)
                           .ThenInclude(sg => sg.Subject));

            if (curriculum == null || !curriculum.Any())
            {
                throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);
            }

            var curriculumDb = curriculum.FirstOrDefault();

            var result = _mapper.Map<CurriculumViewDetailModel>(curriculumDb);


            result.SubjectSelectiveViews = new List<SubjectViewDetailModel>();
            result.SubjectSpecializedtViews = new List<SubjectViewDetailModel>();
            result.SubjectRequiredViews = new List<SubjectViewDetailModel>();
            result.StudentClassGroupViewNames = new List<StudentClassGroupViewName>();

            var listSBInGroup = curriculumDb.CurriculumDetails.ToList();

            var studentClassGroups = await _unitOfWork.StudentClassGroupRepo.GetAsync(
                filter: t => t.CurriculumId == curriculumDb.Id && t.IsDeleted == false);
            if (studentClassGroups.Any() || studentClassGroups != null)
            {
                foreach (var group in studentClassGroups)
                {
                    var studenClassName = new StudentClassGroupViewName
                    {
                        GroupName = group.GroupName
                    };
                    result.StudentClassGroupViewNames.Add(studenClassName);
                }
            }

            var specializedSubjectIds = listSBInGroup
                .Where(sig => sig.IsSpecialized)
                .Select(sig => sig.SubjectId)
                .ToList();

            foreach (var item in listSBInGroup)
            {
                var subjectDetail = new SubjectViewDetailModel
                {
                    Id = item.Id,
                    SubjectName = item.Subject?.SubjectName,
                    Abbreviation = item.Subject?.Abbreviation,
                    SubjectInGroupType = (ESubjectInGroupType)item.SubjectInGroupType,
                    IsRequired = item.Subject?.IsRequired ?? false,
                    Description = item.Subject?.Description,
                    MainSlotPerWeek = item.MainSlotPerWeek,
                    SubSlotPerWeek = item.SubSlotPerWeek,
                    TotalSlotPerWeek = item.MainSlotPerWeek + item.SubSlotPerWeek,
                    IsSpecialized = item.IsSpecialized,
                    IsDoublePeriod = item.IsDoublePeriod,
                    SlotPerTerm = item.SlotPerTerm,
                    TermId = item.TermId ?? 0,
                    TotalSlotInYear = item.Subject?.TotalSlotInYear,
                    SlotSpecialized = item.Subject?.SlotSpecialized ?? 35,
                    SubjectGroupType = (ESubjectGroupType)(item.Subject?.SubjectGroupType ?? 0),
                    MainMinimumCouple = item.MainMinimumCouple,
                    SubMinimumCouple = item.SubMinimumCouple,
                };

                if (item.Subject?.IsRequired == true)
                {
                    result.SubjectRequiredViews.Add(subjectDetail);

                    if (specializedSubjectIds.Contains(item.SubjectId))
                    {
                        result.SubjectSpecializedtViews.Add(subjectDetail);
                    }
                }

                else if (item.Subject?.IsRequired == false)
                {
                    result.SubjectSelectiveViews.Add(subjectDetail);

                    if (specializedSubjectIds.Contains(item.SubjectId))
                    {
                        result.SubjectSpecializedtViews.Add(subjectDetail);
                    }
                }
                else if (item.IsSpecialized)
                {
                    result.SubjectSpecializedtViews.Add(subjectDetail);
                }
            }

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CURRICULUM_DETAIL_SUCCESS,
                Result = result
            };
        }


        #endregion

        #region get Curriculums
        public async Task<BaseResponseModel> GetCurriculums(int schoolId, int? CurriculumId, EGrade? grade, int? schoolYearId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId, filter: t => t.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            if (CurriculumId != null)
            {
                var Curriculum = await _unitOfWork.CurriculumRepo.GetByIdAsync((int)CurriculumId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);
            }
            if (schoolYearId != null)
            {
                var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync((int)schoolYearId, filter: t => t.IsDeleted == false)
                    ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);
            }

            var curriculum = await _unitOfWork.CurriculumRepo.GetPaginationAsync(
                filter: t => t.SchoolId == schoolId
                && (CurriculumId == null || t.Id == CurriculumId)
                && (grade == null || t.Grade == (int)grade)
                && (schoolYearId == null || t.SchoolYearId == schoolYearId)
                && t.IsDeleted == includeDeleted,
                includeProperties: "School",
                orderBy: q => q.OrderBy(s => s.CurriculumName),
                pageIndex: pageIndex,
                pageSize: pageSize
                );
            if (curriculum.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.GET_CURRICULUM_LIST_FAILED
                };
            }
            var result = _mapper.Map<Pagination<BusinessModels.CurriculumBusinessModels.CurriculumDetailViewModel>>(curriculum);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_CURRICULUM_LIST_SUCCESS
                //Result = result
            };
        }
        #endregion

        #region Update Curriculum
        public async Task<BaseResponseModel> UpdateCurriculum(int CurriculumId, CurriculumUpdateModel CurriculumUpdateModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var curriculums = await _unitOfWork.CurriculumRepo.GetV2Async(
                    filter: t => t.Id == CurriculumId && t.IsDeleted == false,
                    include: query => query.Include(c => c.CurriculumDetails)
                               .ThenInclude(sg => sg.Subject)
                               .Include(sg => sg.School)
                               .Include(sg => sg.SchoolYear)
                               .ThenInclude(sy => sy.Terms));
                    var termInYear = await _unitOfWork.TermRepo.GetAsync(filter: t => t.SchoolYearId == CurriculumUpdateModel.SchoolYearId && t.IsDeleted == false)
                        ?? throw new NotExistsException(ConstantResponse.TERM_NOT_EXIST);
                    List<Term> termList = termInYear.ToList();
                    if (curriculums == null || !curriculums.Any())
                    {
                        throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);
                    }

                    var CurriculumDb = curriculums.FirstOrDefault();

                    // Kiểm tra có sự thay đổi trong Name không
                    bool isNameChanged = !string.Equals(CurriculumDb.CurriculumName, CurriculumUpdateModel.CurriculumName, StringComparison.OrdinalIgnoreCase);

                    if (isNameChanged )
                    {
                        // Thực hiện truy vấn kiểm tra trùng lặp chỉ khi có sự thay đổi
                        var checkExistCurriculum = await _unitOfWork.CurriculumRepo.GetAsync(
                            filter: t => (t.CurriculumName.ToLower() == CurriculumUpdateModel.CurriculumName.ToLower()
                                          &&
                                          t.Id != CurriculumId
                        ));

                        if (checkExistCurriculum.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.CURRICULUM_NAME_OR_CODE_EXISTED
                            };
                        }
                    }


                    // check enough number subject
                    if (CurriculumUpdateModel.SpecializedSubjectIds.Count != REQUIRE_SPECIALIZED_SUBJECT
                        || CurriculumUpdateModel.ElectiveSubjectIds.Count != REQUIRE_ELECTIVE_SUBJECT)
                    {
                        return new BaseResponseModel()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = ConstantResponse.INVALID_NUMBER_SUBJECT
                        };
                    }

                    // ko update grade nếu đã được sử dụng bởi lớp nào
                    if (CurriculumUpdateModel.Grade != 0 && CurriculumUpdateModel.Grade != (EGrade)CurriculumDb.Grade)
                    {
                        var classGroups = await _unitOfWork.StudentClassGroupRepo.GetAsync(
                            filter: t => t.Grade == (int)CurriculumUpdateModel.Grade && t.CurriculumId == CurriculumId && !t.IsDeleted);

                        if (classGroups.Any())
                        {
                            return new BaseResponseModel()
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.CURRICULUM_HAS_SUBJECTS_REGISTERED
                            };
                        }

                        CurriculumDb.Grade = (int)CurriculumUpdateModel.Grade;
                    }

                    if (!string.IsNullOrEmpty(CurriculumUpdateModel.CurriculumName))
                    {
                        CurriculumDb.CurriculumName = CurriculumUpdateModel.CurriculumName.Trim();
                    }

                    // get list môn học hiện tại trong tổ hợp
                    var existingSubjectIds = CurriculumDb.CurriculumDetails
                        .Where(query => query.Subject.IsRequired != true)
                        .Select(s => s.SubjectId).ToList();

                    // xác định các môn tự chọn mới và các môn cần xóa bằng except
                    var newElectiveSubjects = CurriculumUpdateModel.ElectiveSubjectIds.Except(existingSubjectIds).ToList();

                    var subjectsToRemove = existingSubjectIds
                        .Except(CurriculumUpdateModel.ElectiveSubjectIds)
                        .Except(CurriculumUpdateModel.SpecializedSubjectIds)
                        .ToList();

                    // xóa các môn không có trong danh sách cập nhật từ SubjectInGroup
                    if (subjectsToRemove.Any() || subjectsToRemove.Count != 0)
                    {
                        var subjectsToRemoveEntities = CurriculumDb.CurriculumDetails
                            .Where(s => subjectsToRemove.Contains(s.SubjectId))
                            .ToList();
                        _unitOfWork.CurriculumDetailRepo.RemoveRange(subjectsToRemoveEntities);
                    }

                    // thêm các môn tự chọn mới
                    foreach (var subjectId in newElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.REQUIRE_ELECTIVE_SUBJECT,
                                Result = subjectId
                            };

                        foreach (var term in termInYear)
                        {
                            var newSubjectInGroup = new CurriculumDetail
                            {
                                SubjectId = subjectId,
                                CurriculumId = CurriculumId,
                                IsSpecialized = CurriculumUpdateModel.SpecializedSubjectIds.Contains(subjectId),
                                TermId = term.Id
                            };
                            await _unitOfWork.CurriculumDetailRepo.AddAsync(newSubjectInGroup.ShallowCopy());
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();

                    //add required subject
                    var requiredSubjects = await _unitOfWork.SubjectRepo.GetAsync(
                        filter: t => t.IsDeleted == false && t.IsRequired == true);
                    var requiredSubjectList = requiredSubjects.ToList();

                    var requiredSBInDB = await _unitOfWork.CurriculumDetailRepo.GetAsync(filter: t => t.CurriculumId == CurriculumId && t.IsDeleted == false);
                    var requiredSubjectInDBList = requiredSBInDB.ToList();

                    if (!requiredSubjectList.All(subject => requiredSubjectInDBList.Any(dbSubject => dbSubject.Id == subject.Id)))
                    {
                        //var requiredSubjectList = new List<SubjectInGroup>();
                        foreach (var subject in requiredSubjects)
                        {
                            int slotPerTerm1 = (int)Math.Floor((double)subject.TotalSlotInYear / 2);
                            int slotPerTerm2 = (int)Math.Ceiling((double)subject.TotalSlotInYear / 2);

                            var newSubjectRequiredInGroup = new CurriculumDetail
                            {
                                SubjectId = subject.Id,
                                CurriculumId = CurriculumId,
                                MainSlotPerWeek = (subject?.TotalSlotInYear / 35) ?? 0,
                                IsSpecialized = CurriculumUpdateModel.SpecializedSubjectIds.Contains(subject.Id)
                            };

                            for (int i = 0; i < termList.Count; i++)
                            {
                                newSubjectRequiredInGroup.CreateDate = DateTime.UtcNow;
                                newSubjectRequiredInGroup.TermId = termList[i].Id;
                                newSubjectRequiredInGroup.SlotPerTerm = (i == 0) ? slotPerTerm1 : slotPerTerm2;
                                await _unitOfWork.CurriculumDetailRepo.AddAsync(newSubjectRequiredInGroup.ShallowCopy());
                            }
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }


                    //Specialized Subject
                    var specializedSubjectNotInElectiveSubjects = CurriculumUpdateModel.SpecializedSubjectIds
                        .Where(s => !CurriculumUpdateModel.ElectiveSubjectIds.Contains(s));


                    foreach (int subjectId in specializedSubjectNotInElectiveSubjects)
                    {
                        var checkSubject = await _unitOfWork.SubjectRepo.GetByIdAsync(subjectId)
                            ?? throw new NotExistsException(ConstantResponse.SUBJECT_NOT_EXISTED);

                        if (!checkSubject.IsRequired)
                            return new BaseResponseModel
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = ConstantResponse.INVALID_SPECIALIZED_SUBJECT,
                                Result = subjectId
                            };
                    }

                    transaction.Commit();

                    var subjectInDB = await _unitOfWork.CurriculumRepo.GetV2Async(
                        filter: t => t.Id == CurriculumId && t.IsDeleted == false,
                        include: query => query.Include(q => q.CurriculumDetails));

                    var CurriculumInDb = subjectInDB.FirstOrDefault();

                    var existingSubjectIdsInDB = CurriculumInDb.CurriculumDetails
                        .Where(query => query.IsSpecialized == true)
                        .Select(s => s.SubjectId).ToList();

                    var newSpecialSubjects = CurriculumUpdateModel.SpecializedSubjectIds.Except(existingSubjectIdsInDB).ToList();

                    var subjectsSpecialToRemove = existingSubjectIdsInDB
                        .Except(CurriculumUpdateModel.SpecializedSubjectIds)
                        .ToList();

                    var updateSpecialSubject = await _unitOfWork.CurriculumDetailRepo.GetAsync(
                        filter: t => t.CurriculumId == CurriculumId && newSpecialSubjects.Contains(t.SubjectId));

                    // update false các môn chuyên đề không có trong danh sách cập nhật
                    if (subjectsSpecialToRemove.Count != 0)
                    {
                        var subjectsToRemoveEntities = CurriculumDb.CurriculumDetails
                            .Where(s => subjectsSpecialToRemove.Contains(s.SubjectId))
                            .ToList();
                        foreach (var subjectInGroup in subjectsToRemoveEntities)
                        {
                            subjectInGroup.IsSpecialized = false;
                            _unitOfWork.CurriculumDetailRepo.Update(subjectInGroup);
                        }
                    }

                    //update new specialized
                    foreach (var subjectInGroup in updateSpecialSubject)
                    {
                        subjectInGroup.IsSpecialized = true;
                        _unitOfWork.CurriculumDetailRepo.Update(subjectInGroup);
                    }

                    _unitOfWork.CurriculumRepo.Update(CurriculumDb);
                    await _unitOfWork.SaveChangesAsync();

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

        #region DeleteCurriculum
        public async Task<BaseResponseModel> DeleteCurriculum(int curriculumId)
        {
            var curriculum = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId, include: query => query.Include(sg => sg.CurriculumDetails)) ?? throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);
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
