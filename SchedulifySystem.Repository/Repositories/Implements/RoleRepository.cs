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
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly SchedulifyContext _context;

        public RoleRepository(SchedulifyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            var existRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return existRole;
        }

        public async Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds)
        {
            return await _context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        }
    }
}
