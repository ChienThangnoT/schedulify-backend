using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.TeacherBusinessModels
{
    public class TeacherGenerateAccount
    {
        public required int SchoolId { get; set; }
        public required int TeacherId { get; set; }
    }
}
