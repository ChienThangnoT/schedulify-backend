using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.DBContext;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Implements
{
    public class RoleAssignmentRepository : GenericRepository<RoleAssignment>, IRoleAssignmentRepository
    {
        private readonly SchedulifyContext _context;

        public RoleAssignmentRepository(SchedulifyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoleAssignment>> GetRolesByAccountIdAsync(int id)
        {
            var roles = await _context.RoleAssignments
                .Where(x => x.AccountId == id)
                .ToListAsync();
            return roles;
        }

    }
}
