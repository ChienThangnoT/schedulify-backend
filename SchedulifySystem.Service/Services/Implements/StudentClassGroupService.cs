using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels;
using SchedulifySystem.Service.Enums;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.Utils.Constants;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class StudentClassGroupService : IStudentClassGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClassGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Check data
        private async Task<BaseResponseModel> CheckData(HashSet<string> codes, HashSet<string> names, int schoolId, int schoolYearId, int skip = 0)
        {
            // Kiểm tra trùng lặp trong danh sách đầu vào
            if (codes.Count != codes.Distinct().Count() || names.Count != names.Distinct().Count())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NAME_OR_CODE_EXISTED
                };
            }

            // Kiểm tra trùng lặp trong cơ sở dữ liệu
            var classGroupFounds = await _unitOfWork.StudentClassGroupRepo.GetV2Async(
                filter: stg => !stg.IsDeleted && stg.SchoolId == schoolId && stg.SchoolYearId == schoolYearId && stg.Id != skip &&
                (codes.Contains(stg.StudentClassGroupCode.ToLower()) || names.Contains(stg.GroupName.ToLower())));

            if (classGroupFounds.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NAME_OR_CODE_EXISTED
                };
            }

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK
            };
        }
        #endregion

        #region Add Student Classgroup
        public async Task<BaseResponseModel> AddStudentClassgroup(int schoolId, int schoolYearId, List<AddStudentClassGroupModel> models)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_NOT_FOUND);
            var schoolYear = await _unitOfWork.SchoolYearRepo.GetByIdAsync(schoolYearId)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_YEAR_NOT_EXIST);

            var codes = models.Select(m => m.StudentClassGroupCode.ToLower()).ToHashSet();
            var names = models.Select(m => m.GroupName.ToLower()).ToHashSet();

            var checkResult = await CheckData(codes, names, schoolId, schoolYearId);
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            var data = _mapper.Map<List<StudentClassGroup>>(models);
            data.ForEach(stg =>
            {
                stg.SchoolId = schoolId;
                stg.SchoolYearId = schoolYearId;
            });

            await _unitOfWork.StudentClassGroupRepo.AddRangeAsync(data);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.ADD_STUDENT_CLASS_GROUP_SUCCESS
            };
        }
        #endregion                  

        #region Delete Student ClassGroup
        public async Task<BaseResponseModel> DeleteStudentClassGroup(int classGroupId)
        {
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            classGroup.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.DELETE_STUDENT_CLASS_GROUP_SUCCESS
            };
        }
        #endregion

        #region Get Student Class Groups
        public async Task<BaseResponseModel> GetStudentClassGroups(int schoolId, int schoolYearId, int pageIndex = 1, int pageSize = 20)
        {
            var classGroups = await _unitOfWork.StudentClassGroupRepo.ToPaginationIncludeAsync(pageIndex, pageSize,
                filter: stg => !stg.IsDeleted && stg.SchoolId == schoolId && stg.SchoolYearId == schoolYearId,
                include: query => query.Include(scg => scg.StudentClasses),
                orderBy: stg => stg.OrderBy(c => c.GroupName)
            );

            // Lọc lại danh sách StudentClasses chưa bị xóa
            foreach (var group in classGroups.Items)
            {
                group.StudentClasses = group.StudentClasses
                    .Where(sc => !sc.IsDeleted)
                    .ToList();
            }

            var result = _mapper.Map<Pagination<StudentClassGroupViewModel>>(classGroups);

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_STUDENT_CLASS_GROUP_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region Get Student Class Group By id
        public async Task<BaseResponseModel> GetStudentClassGroupById(int schoolId, int schoolYearId, int id)
        {
            // Lấy danh sách nhóm lớp theo trường và năm học
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(id,
                filter: stg => !stg.IsDeleted && stg.SchoolId == schoolId && stg.SchoolYearId == schoolYearId,
                include: query => query.Include(scg => scg.StudentClasses)
            ) ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND);

            classGroup.StudentClasses = classGroup.StudentClasses
                    .Where(sc => !sc.IsDeleted)
                    .ToList();

            var result = _mapper.Map<Pagination<StudentClassGroupViewModel>>(classGroup);

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.GET_STUDENT_CLASS_GROUP_SUCCESS,
                Result = result
            };
        }
        #endregion

        #region Update Student Class Group
        public async Task<BaseResponseModel> UpdateStudentClassGroup(int classGroupId, UpdateStudentClassGroupModel model)
        {
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(classGroupId);
            if (classGroup == null || classGroup.IsDeleted)
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = ConstantResponse.STUDENT_CLASS_GROUP_NOT_FOUND
                };
            }

            var codes = new HashSet<string> { model.StudentClassGroupCode.ToLower() };
            var names = new HashSet<string> { model.GroupName.ToLower() };

            var checkResult = await CheckData(codes, names, (int)classGroup.SchoolId, (int)classGroup.SchoolYearId, classGroupId); ;
            if (checkResult.Status != StatusCodes.Status200OK)
                return checkResult;

            classGroup.StudentClassGroupCode = model.StudentClassGroupCode;
            classGroup.GroupName = model.GroupName;
            classGroup.GroupDescription = model.GroupDescription;

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_STUDENT_CLASS_GROUP_SUCCESS
            };
        }
        #endregion

        #region AssignCurriculumToClassGroup
        public async Task<BaseResponseModel> AssignCurriculumToClassGroup(int schoolId, int schoolYearId, int id, int curriculumId)
        {
            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(id,
                filter: t => !t.IsDeleted && t.SchoolId == schoolId && t.SchoolYearId == schoolYearId,
                include: query => query.Include(c => c.StudentClasses))
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_GROUP_NOT_EXIST);

            var curriculum = await _unitOfWork.CurriculumRepo.GetByIdAsync(curriculumId,
                filter: c => c.SchoolId == schoolId && c.SchoolYearId == schoolYearId,
                include: query => query.Include(c => c.CurriculumDetails))
                ?? throw new NotExistsException(ConstantResponse.CURRICULUM_NOT_EXISTED);

            var classes = classGroup.StudentClasses.Where(c => !c.IsDeleted).ToList();
            if (classes.Any() && classGroup.CurriculumId != curriculumId)
            {
                await UpsertAssignment(curriculum, classGroup, classes);
            }

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.CURRICULUM_ASSIGN_SUCCESS
            };
        }
        #endregion

        #region Upsert Assignment
        private async Task UpsertAssignment(Curriculum curriculum, StudentClassGroup classGroup, List<StudentClass> classes)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Fetch existing assignments once
                var classIds = classes.Select(c => c.Id).ToList();
                var oldAssignments = await _unitOfWork.TeacherAssignmentRepo.GetV2Async(
                    filter: ta => classIds.Contains(ta.StudentClassId) && !ta.IsDeleted);

                if (oldAssignments.Any())
                {
                    _unitOfWork.TeacherAssignmentRepo.RemoveRange(oldAssignments);
                }

                // Prepare new assignments and update classes
                var newAssignments = new List<TeacherAssignment>();
                foreach (var sClass in classes)
                {
                    sClass.UpdateDate = DateTime.UtcNow;
                    sClass.PeriodCount = 0;

                    foreach (var sig in curriculum.CurriculumDetails)
                    {
                        newAssignments.Add(new TeacherAssignment
                        {
                            AssignmentType = (int)AssignmentType.Permanent,
                            PeriodCount = sig.MainSlotPerWeek + sig.SubSlotPerWeek,
                            StudentClassId = sClass.Id,
                            CreateDate = DateTime.UtcNow,
                            SubjectId = sig.SubjectId,
                            TermId = (int)sig.TermId
                        });

                        sClass.PeriodCount += sig.MainSlotPerWeek + sig.SubSlotPerWeek;
                    }

                    _unitOfWork.StudentClassesRepo.Update(sClass);
                }

                // Add new assignments in bulk
                if (newAssignments.Any())
                {
                    await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignments);
                }

                classGroup.UpdateDate = DateTime.UtcNow;
                _unitOfWork.StudentClassGroupRepo.Update(classGroup);
                await _unitOfWork.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region AssignClassToClassGroup
        public async Task<BaseResponseModel> AssignClassToClassGroup(int schoolId, int schoolYearId, int id, AssignClassToClassGroup model)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId,
                filter: s => s.Status == (int)SchoolStatus.Active)
                ?? throw new NotExistsException(ConstantResponse.SCHOOL_ACCOUNT_NOT_EXIST);

            var classGroup = await _unitOfWork.StudentClassGroupRepo.GetByIdAsync(id,
                filter: f => !f.IsDeleted && f.SchoolId == schoolId && f.SchoolYearId == schoolYearId,
                include: query => query.Include(cg => cg.Curriculum).ThenInclude(c => c.CurriculumDetails))
                ?? throw new NotExistsException(ConstantResponse.STUDENT_CLASS_GROUP_NOT_EXIST);

            var classes = await _unitOfWork.StudentClassesRepo.GetV2Async(
                filter: f => !f.IsDeleted && f.SchoolId == schoolId && f.SchoolYearId == schoolYearId && model.ClassIds.Contains(f.Id));

            if (classes.Count() != model.ClassIds.Count())
            {
                var invalidIds = model.ClassIds.Except(classes.Select(c => c.Id));
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Lớp Id {string.Join(", ", invalidIds)} không tồn tại!"
                };
            }

            var invalidClassesGrade = classes.Where(c => c.Grade != classGroup.Grade);
            if (invalidClassesGrade.Any())
            {
                return new BaseResponseModel
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Lớp {string.Join(", ", invalidClassesGrade.Select(c => c.Name))} không cùng nhóm khối!"
                };
            }

            var classesToUpsertAssignment = classes
                .Where(c => !classGroup.StudentClasses.Select(sc => sc.Id).Contains(c.Id))
                .ToList();

            if (classesToUpsertAssignment.Any())
            {
                if (classGroup.Curriculum != null)
                {
                    await UpsertAssignment(classGroup.Curriculum, classGroup, classesToUpsertAssignment);
                }
                else
                {
                    classesToUpsertAssignment.ForEach(c => c.StudentClassGroupId = classGroup.Id);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return new BaseResponseModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Thêm lớp vào nhóm lớp thành công!"
            };
        }
        #endregion

    }


}
