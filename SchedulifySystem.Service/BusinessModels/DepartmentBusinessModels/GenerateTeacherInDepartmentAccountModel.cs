using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class GenerateTeacherInDepartmentAccountModel
    {
        public required int SchoolId { get; set; }
        public required int DepartmentId { get; set; }
    }
}
