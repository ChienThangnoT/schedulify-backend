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
    public class UserRepository : GenericRepository<Account>, IUserRepository
    {
        public UserRepository(SchedulifyContext context) : base(context)
        {
        }
    }
}
