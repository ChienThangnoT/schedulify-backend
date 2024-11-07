using SchedulifySystem.Service.BusinessModels.SubjectBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectGroupBusinessModels
{
    public class QuickAssignPeriodModel
    {
        public List <SubjectAssignmentConfig> SubjectAssignmentConfigs { get; set; }
        public List<int> SubjectGroupApplyIds { get; set; }
        public int SlotSpecialized {  get; set; }
    }
}
