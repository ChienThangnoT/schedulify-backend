using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubjectBusinessModels
{
    public class SubjectUpdateModel
    {
        public required string SubjectName { get; set; }
        public required string Abbreviation { get; set; }
        public required string Description { get; set; }
        //[JsonIgnore]
        //public DateTime UpdateDate { get; set; }
    }
}
