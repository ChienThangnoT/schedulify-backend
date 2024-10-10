using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class OTP : BaseEntity
    {
        public int AccountId { get; set; }
        public int Code { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool isUsed { get; set; }

        public Account? Account { get; set; }
    }
}
