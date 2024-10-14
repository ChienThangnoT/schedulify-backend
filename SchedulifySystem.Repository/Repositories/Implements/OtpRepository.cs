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
    public class OtpRepository : GenericRepository<OTP>, IOtpRepository
    {
        private readonly SchedulifyContext _context;

        public OtpRepository(SchedulifyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OTP> GetOTPByCodeAsync(int code)
        {
            return await _context.OTPs.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
