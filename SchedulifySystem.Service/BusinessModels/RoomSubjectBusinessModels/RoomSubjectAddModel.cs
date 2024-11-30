using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.RoomSubjectBusinessModels
{
    public class RoomSubjectAddModel
    {
        public required int SubjectId { get; set; }
        public required int RoomId { get; set; }
        public required int SchoolId { get; set; }
        public required int TermId { get; set; }
        public required string RoomSubjectCode { get; set; }
        public required string RoomSubjectName { get; set; }
        public required ERoomSubjectModel Model { get; set; }
        public MainSession? Session { get; set; }
        public required int TeacherId { get; set; }
        public required List<int> StudentClassId { get; set; }
    }

    public class RoomSubjectsViewModel : BaseEntity
    {
        public int SubjectId { get; set; }
        public int RoomId { get; set; }
        public int SchoolId { get; set; }
        public int TermId { get; set; }
        public string? RoomSubjectCode { get; set; }
        public string? RoomSubjectName { get; set; }
        public ERoomSubjectModel Model { get; set; }
        public MainSession? Session { get; set; }
        public List<StudentClassList>? StudentClass { get; set; }
    }
    public class StudentClassList
    {
        public int Id { get; set; }
        public string? StudentClassName { get; set; }
    }
}
