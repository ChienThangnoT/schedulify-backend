using Microsoft.EntityFrameworkCore.Storage;
using SchedulifySystem.Repository.DBContext;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchedulifyContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleAssignmentRepository _roleAssignmentRepository;

        public UnitOfWork(SchedulifyContext context, 
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRoleAssignmentRepository roleAssignmentRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleAssignmentRepository = roleAssignmentRepository;
        }

        public IUserRepository UserRepo => _userRepository;
        public IRoleRepository RoleRepo => _roleRepository;
        public IRoleAssignmentRepository RoleAssignmentRepo => _roleAssignmentRepository;


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
