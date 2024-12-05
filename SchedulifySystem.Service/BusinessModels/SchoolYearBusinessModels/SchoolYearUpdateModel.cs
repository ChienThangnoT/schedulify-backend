using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SchoolYearBusinessModels
{
    public class SchoolYearUpdateModel
    {
        [RegularExpression(@"^\d{4}$", ErrorMessage = "StartYear must be a valid 4-digit year")]
        public required string StartYear { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "StartYear must be a valid 4-digit year")]
        public required string EndYear { get; set; }
        public required string SchoolYearCode { get; set; }
    }
}
