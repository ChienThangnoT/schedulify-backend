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
    public class UserRepository : GenericRepository<Account>, IUserRepository
    {
        private readonly SchedulifyContext _context;

        public UserRepository(SchedulifyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByEmail(string email)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }
    }
}
