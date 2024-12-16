using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.CurriculumDetailBusinessModels;
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
    public class CurriculumDetailService : ICurriculumDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CurriculumDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> UpdateCurriculumDetail(int schoolId, int yearId, int curriculumId, int termId, List<CurriculumDetailUpdateModel> curriculumDetailUpdateModel)
        {


            var curriculumDetailIds = curriculumDetailUpdateModel.Select(x => x.CurriculumDetailId).ToList();

            var curriculumDetails = await _unitOfWork.CurriculumDetailRepo.GetAsync(
                filter: t => curriculumDetailIds.Contains(t.Id) && t.IsDeleted == false
                && t.TermId == termId && schoolId == t.Curriculum.SchoolId && t.CurriculumId == curriculumId);

            if (curriculumDetails == null || !curriculumDetails.Any())
                throw new NotExistsException(ConstantResponse.SUBJECT_IN_GROUP_NOT_FOUND);

            foreach (var subject in curriculumDetailUpdateModel)
            {
                var curriculumDetail = curriculumDetails.FirstOrDefault(s => s.Id == subject.CurriculumDetailId);
                if (curriculumDetail != null)
                {
                    _mapper.Map(subject, curriculumDetail);
                    _unitOfWork.CurriculumDetailRepo.Update(curriculumDetail);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await UpdateToAssignment(curriculumId);
            return new BaseResponseModel()
            {
                Status = StatusCodes.Status200OK,
                Message = ConstantResponse.UPDATE_SUBJECT_IN_GROUP_SUCCESS
            };
        }

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
                    if (assignmentToRemove.Any())
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
                    if (newAssignments.Any())
                    {
                        await _unitOfWork.TeacherAssignmentRepo.AddRangeAsync(newAssignments);
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
