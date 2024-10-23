using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Enums
{
    public enum ETimetableFlag
    {
        None = 0,    // vị trí không xếp
        Filled = 1,  // Vị trí đã được lấp đầy (đã có lớp học hoặc giáo viên).
        Unfilled = 2,// Vị trí chưa được lấp đầy (chưa có lớp học hoặc giáo viên).
        Fixed = 3,   // Vị trí đã được cố định, không thể thay đổi (ví dụ như các lớp học bắt buộc phải diễn ra vào thời gian cụ thể).
    }
}
