using Microsoft.EntityFrameworkCore.Storage;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepo { get; }
        public IRoleRepository RoleRepo { get; }
        public IRoleAssignmentRepository RoleAssignmentRepo { get; }
        public ITeacherRepository TeacherRepo { get; }
        public ISchoolRepository SchoolRepo { get; }
        public IStudentClassesRepository StudentClassesRepo { get; }
        public ISubjectRepository SubjectRepo{ get; }
        public IBuildingRepository BuildingRepo { get; }
        public IRoomRepository RoomRepo { get; }
        public ISubjectGroupRepository SubjectGroupRepo { get; }
        public IDistrictRepository DistrictRepo { get; }
        public IProvinceRepository ProvinceRepo { get; }
        public IOtpRepository OTPRepo { get; }
        public ISchoolYearRepository SchoolYearRepo { get; }
        public IDepartmentRepository DepartmentRepo { get; }
        public ITeacherAssignmentRepository TeacherAssignmentRepo { get; }
        public ITeachableSubjectRepository TeachableSubjectRepo { get; }
        public ITermRepository TermRepo { get; }
        public ISubjectInGroupRepository SubjectInGroupRepo { get; }
        public IRoomSubjectRepository RoomSubjectRepo { get; }
        public ISchoolScheduleRepository SchoolScheduleRepo { get; }
        public Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
