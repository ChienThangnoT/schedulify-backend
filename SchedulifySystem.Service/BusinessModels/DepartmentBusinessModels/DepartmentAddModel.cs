using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.BusinessModels.DepartmentBusinessModels
{
    public class DepartmentAddModel
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ngày họp không được để trống.")]
        public int? MeetingDay { get; set; }

        [Required(ErrorMessage = "Mã Tổ không được để trống.")]
        [StringLength(10, ErrorMessage = "Mã tổ không được vượt quá 10 ký tự.")]
        public string? DepartmentCode { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
    }
}
