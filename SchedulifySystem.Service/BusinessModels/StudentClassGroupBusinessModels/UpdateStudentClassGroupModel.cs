using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.StudentClassGroupBusinessModels
{
    public class UpdateStudentClassGroupModel
    {
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public string? StudentClassGroupCode { get; set; }
        public int Grade { get; set; }

    }
}
