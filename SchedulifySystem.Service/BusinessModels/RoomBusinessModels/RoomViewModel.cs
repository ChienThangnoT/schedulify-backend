using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomBusinessModels
{
    public class RoomViewModel : BaseEntity
    {
        public string? Name { get; set; }
        public ERoomType RoomType { get; set; }
        public int MaxClassPerTime { get; set; }
        public string? RoomCode { get; set; }
        public int BuildingId { get; set; }
        public AvailabilityStatus AvailabilityeStatus { get; set; }
        public List<RoomSubjectViewModel> Subjects { get; set; }
    }

    public class RoomScheduleResponse
    {
        public bool IsGroupClass { get; set; }
        public List<ClassPeriodModel> RelatedClasses { get; set; } = new();
        public List<RoomView> AvailableRooms { get; set; } = new();
    }

    public class ClassPeriodModel
    {
        public int? ClassScheduleId { get; set; }
        public int? RoomId { get; set; }
        public int? TeacherId { get; set; }
        public int? SubjectId { get; set; }
        public int StartAt { get; set; }
    }

    public class RoomView
    {
        public int RoomId { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
    }

}
