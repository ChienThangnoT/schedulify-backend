using SchedulifySystem.Repository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Interfaces
{
    public interface IOtpRepository : IGenericRepository<OTP>
    {
        public Task<OTP> GetOTPByCodeAsync(int code);
    }
}
