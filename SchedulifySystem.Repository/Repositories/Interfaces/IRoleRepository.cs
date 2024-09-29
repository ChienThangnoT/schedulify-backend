using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        public Task<Role> GetRoleByNameAsync(string name);
        public Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds);
    }
}
