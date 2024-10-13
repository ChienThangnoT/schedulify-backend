using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class SubjectGroupViewModel
    {
        public int id { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string? GroupDescription { get; set; }
        public int SubjectGroupTypeId { get; set; }
        public int SubjectGroupTypeName { get; set; }
        
    }
}
