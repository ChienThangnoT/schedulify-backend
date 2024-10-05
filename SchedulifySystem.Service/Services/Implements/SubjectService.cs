using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using SchedulifySystem.Service.Exceptions;
using SchedulifySystem.Service.Services.Interfaces;
using SchedulifySystem.Service.UnitOfWork;
using SchedulifySystem.Service.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Services.Implements
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Create Subject
        public async Task<BaseResponseModel> CreateSubject(SubjectAddModel subjectAddModel)
        {
            var existSubject = await _unitOfWork.SubjectRepo.GetAsync(filter: t => t.SubjectName.ToLower() == subjectAddModel.SubjectName.ToLower());
            var existSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(subjectAddModel.SchoolId) ?? throw new NotExistsException($"School is not existed with id {subjectAddModel.SchoolId}");

            if (existSubject.FirstOrDefault() != null)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Subject name {subjectAddModel.SubjectName} already exists!"
                };
            }

            var baseAbbreviation = subjectAddModel.Abbreviation.ToLower();
            var duplicateAbbre = await _unitOfWork.SubjectRepo.GetAsync(filter: t => t.Abbreviation.ToLower().StartsWith(baseAbbreviation));

            string newAbbreviation = baseAbbreviation;
            int counter = 1;

            while (duplicateAbbre.Any(t => t.Abbreviation == newAbbreviation))
            {
                var match = System.Text.RegularExpressions.Regex.Match(baseAbbreviation, @"^(.*?)(\d+)$");

                if (match.Success)
                {
                    var textPart = match.Groups[1].Value;
                    var numberPart = int.Parse(match.Groups[2].Value);
                    newAbbreviation = $"{textPart}{numberPart + counter}";
                }
                else
                {
                    newAbbreviation = $"{baseAbbreviation}{counter}";
                }
                counter++;
            }

            subjectAddModel.Abbreviation = newAbbreviation;

            var subjectEntity = _mapper.Map<Subject>(subjectAddModel);
            await _unitOfWork.SubjectRepo.AddAsync(subjectEntity);
            await _unitOfWork.SaveChangesAsync();

            subjectEntity.School = existSchool;
            var result = _mapper.Map<SubjectViewModel>(subjectEntity);

            return new BaseResponseModel()
            {
                Status = StatusCodes.Status201Created,
                Message = "Create subject success",
                Result = result
            };
        }

        #endregion

        #region get subject list by school id
        public async Task<BaseResponseModel> GetSubjectBySchoolId(int schoolId, bool includeDeleted, int pageIndex, int pageSize)
        {
            var school = await _unitOfWork.SchoolRepo.GetByIdAsync(schoolId) ?? throw new NotExistsException($"School not found with id {schoolId}");
            var subject = await _unitOfWork.SubjectRepo.ToPaginationIncludeAsync(
                pageSize, pageIndex,
                filter: t => t.SchoolId == schoolId && (includeDeleted ? true : t.IsDeleted == false),
                include: query => query.Include(t => t.School));
            if (subject.Items.Count == 0)
            {
                return new BaseResponseModel()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Not found subject with school id {schoolId}"
                };
            }
            var result = _mapper.Map<Pagination<SubjectViewModel>>(subject);
            return new BaseResponseModel() { 
                Status = StatusCodes.Status200OK, 
                Message = "Get subject list successful", 
                Result = result 
            };
        }
        #endregion
    }
}
