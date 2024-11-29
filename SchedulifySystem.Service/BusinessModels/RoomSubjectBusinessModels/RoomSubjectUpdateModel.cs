using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels
{
    public class RoomSubjectUpdateModel
    {
        public required int SubjectId { get; set; }
        public required int RoomId { get; set; }
        public required int TermId { get; set; }
        public required string RoomSubjectCode { get; set; }
        public required string RoomSubjectName { get; set; }
        public required ERoomSubjectModel Model { get; set; }
        public required List<int> StudentClassIds { get; set; }
    }
}
