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

        public Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
