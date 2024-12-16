using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.SubmitRequest
{
    public class SubmitRequestViewModel : BaseEntity
    {
        public int TeacherId { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherLastName { get; set; }
        public ERequestType RequestType { get; set; }
        public DateTime? RequestTime { get; set; }
        public ERequestStatus Status { get; set; }
        public string? RequestDescription { get; set; }
        public string? ProcessNote { get; set; }
        public string? AttachedFile { get; set; }
        public int? SchoolYearId { get; set; }
        public string? SchoolYearCode { get; set; }
    }
}
