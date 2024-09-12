using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.EntityModels
{
    public partial class Building : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Floor {  get; set; }
        public string? Address { get; set; }
        public int SchoolId { get; set; }

        public School? School { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        
    }
}
