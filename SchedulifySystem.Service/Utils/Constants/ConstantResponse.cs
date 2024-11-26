using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils.Constants
{
    public static class ConstantResponse
    {
        //User
        public const string ACCOUNT_NOT_EXIST = "Account not exist.";
        public const string SCHOOL_ACCOUNT_NOT_EXIST = "Not found school manager!";
        public const string ACCOUNT_CAN_NOT_ACCESS = "Account can not access.";
        public const string PASSWORD_INCORRECT = "Password incorrect.";
        public const string SCHOOL_NOT_FOUND = "School Not found.";
        public const string SCHOOL_ALREADY_ASSIGNED = "School has been assigned to another account.";
        public const string SCHOOL_MANAGER_CREAT_SUCCESSFUL = "Create school manager successful.";
        public const string REQUEST_RESET_PASSWORD_SUCCESSFUL = "Send otp reset password success.";
        public const string REQUEST_RESET_PASSWORD_FAILED = "Send otp reset password failed.";
        public const string OTP_NOT_VALID = "OTP unvalid.";
        public const string CONFIRM_RESET_PASSWORD_SUCCESSFUL = "Confirm otp reset password success. You can reset password now.";
        public const string RESET_PASSWORD_SUCCESS = "Reset password success.";
        public const string GET_ACCOUNT_DETAIL_SUCCESS = "Get account detail success.";
        public const string UPDATE_ACCOUNT_DETAIL_SUCCESS = "Update account detail success.";
        public const string CHANGE_PASSWORD_SUCCESSFUL = "Change password success.";
        public const string CHANGE_PASSWORD_FAILED = "Your password invalid. Try again.";
        public const string CHANGE_ACCOUNT_STATUS = "Account status must be different than Pending status or not null";
        public const string SCHOOL_MANAGERR_ALREADY_CONFIRM = "School manager has been verified!";
        public const string SCHOOL_MANAGERR_CONFIRM = "Confirm create school manager account success";

        public const string ACCOUNT_LIST_NOT_EXIST = "Account list not exist";


        //auth
        public const string INVALID_REFRESH = "Invalid Refresh token!";
        public const string REFRESH_TOKEN_SUCCESS = "Refresh token successful.";
        public const string REFRESH_TOKEN_INVALID = "Refresh token invalid or expired time";
        public const string ACCOUNT_NOT_EXIST_AUTH = "Account not exist!";

        //Room
        public const string ROOM_NOT_EXIST = "The room does not exist.";
        public const string ROOM_NAME_EXISTED = "The room name already exists.";
        public const string ROOM_NAME_DUPLICATED = "The room name is duplicated";
        public const string ROOM_CODE_EXISTED = "The room code already exists.";
        public const string ROOM_CODE_DUPLICATED = "The room code is duplicated";
        public const string ROOM_CODE_OR_NAME_EXISTED = "The room code or room name already exists.";
        public const string ADD_ROOM_SUCCESS = "Add room success.";
        public const string UPDATE_ROOM_SUCCESS = "Update room success.";
        public const string DELETE_ROOM_SUCCESS = "Delete room success.";
        public const string DELETE_ROOM_FAILED = "Xóa phòng thất bại vì đang có lớp học sử dụng.";
        public const string GET_ROOM_SUCCESS = "Get rooms success.";
        public const string ROOM_TYPE_BAD_REQUEST = "Each pactical room require at least one subjects abreviation and it exists in database.";
        public const string ROOM_ALREADY_IN_USE = "The room already in use by another class.";

        //Building
        public const string BUILDING_NOT_EXIST = "The building does not exist.";
        public const string BUILDING_NAME_EXISTED = "The building name already exists.";
        public const string BUILDING_NAME_DUPLICATED = "The building name is duplicated";
        public const string BUILDING_CODE_NOT_EXIST = "The building code does not exist.";
        public const string BUILDING_CODE_EXISTED = "The building code already exists.";
        public const string BUILDING_CODE_DUPLICATED = "The building code is duplicated";
        public const string BUILDING_CODE_OR_NAME_EXISTED = "The building code or building name already exists.";
        public const string ADD_BUILDING_SUCCESS = "Add building success.";
        public const string UPDATE_BUILDING_SUCCESS = "Update building success.";
        public const string DELETE_BUILDING_SUCCESS = "Delete building success.";
        public const string DELETE_BUILDING_FAILED = "Xóa cơ sở thất bại vì đang có phòng học sử dụng.";
        public const string GET_BUILDING_SUCCESS = "Get buildings success.";

        //subject group type
        public const string SUBJECT_GROUP_TYPE_NOT_EXISTED = "Subject group type not exist.";

        //curriiculum 
        public const string CURRICULUM_NAME_EXISTED = "Curriculum name already exist.";
        public const string CURRICULUM_CODE_EXISTED = "Curriculum code already exist.";
        public const string CURRICULUM_NAME_OR_CODE_EXISTED = "Curriculum name or code already exist.";
        public const string ADD_CURRICULUM_SUCCESS = "Add curriculum success.";
        public const string UPDATE_CURRICULUM_SUCCESS = "Update curriculum success.";
        public const string DELETE_CURRICULUM_SUCCESS = "Delete curriculum success.";
        public const string CURRICULUM_NOT_EXISTED = "Curriculum not exist.";
        public const string GET_CURRICULUM_LIST_FAILED = "Get curriculum list has no items";
        public const string GET_CURRICULUM_LIST_SUCCESS = "Get curriculum list successful";
        public const string CURRICULUM_HAS_SUBJECTS_REGISTERED = "curriculum has assign, can not update grade.";
        public const string INVALID_NUMBER_SUBJECT = "Để tạo tổ hợp môn cần chọn 4 môn tự chọn và 3 môn chuyên đề.";
        public const string INVALID_UPDATE_GRADE_DIFFERENT_STUDENT_CLASS_GROUP = "Không thể cập nhật khối vì khác khối của chương trình hiện tại của nhóm lớp.";

        //subject 
        public const string SUBJECT_NAME_EXISTED = "Subject name already exists in school.";
        public const string ADD_SUBJECT_SUCCESS = "Add subject success.";
        public const string SUBJECT_NOT_EXISTED = "Subject not exist.";
        public const string UPDATE_SUBJECT_SUCCESS = "Update subject successful";
        public const string GET_SUBJECT_LIST_SUCCESS = "Get subject list successful";
        public const string GET_SUBJECT_SUCCESS = "Get subject successful";
        public const string SUBJECT_NAME_ALREADY_EXIST_IN_LIST = "Subject name duplicate in list.";
        public const string SUBJECT_NAME_ALREADY_EXIST = "Subject name already exist.";
        public const string ADD_SUBJECT_LIST_SUCCESS = "Operation completed";
        public const string GET_CURRICULUM_DETAIL_SUCCESS = "Get subject group detail success.";
        public const string REQUIRE_ELECTIVE_SUBJECT = "Môn lựa chọn phải khác môn bắt buộc.";
        public const string INVALID_SPECIALIZED_SUBJECT = "Môn chuyên đề phải nằm trong nhóm môn bắt buộc hoặc là trong tổ hợp môn";
        public const string DELETE_SUBJECT_SUCCESS = "Delete subject success.";
        public const string SUBJECT_ALREADY_USED = "Subject already use by school.";

        //subject in group
        public const string SUBJECT_IN_GROUP_NOT_FOUND = "Subject in group not found.";
        public const string UPDATE_SUBJECT_IN_GROUP_SUCCESS = "Update subject in group success.";


        //Room Type
        public const string ROOM_TYPE_NOT_EXIST = "The room type does not exist.";
        public const string ROOM_TYPE_CODE_NOT_EXIST = "The room type code does not exist.";

        //Student Class
        public const string CLASS_NOT_EXIST = "The class does not exist.";
        public const string CLASS_NAME_EXISTED = "The class name already exists.";
        public const string CLASS_NAME_DUPLICATED = "The class name is duplicated";
        public const string CLASS_ID_DUPLICATED = "The class id is duplicated";
        public const string ADD_CLASS_SUCCESS = "Add class success.";
        public const string UPDATE_CLASS_SUCCESS = "Update class success.";
        public const string DELETE_CLASS_SUCCESS = "Delete class success.";
        public const string GET_CLASS_SUCCESS = "Get classes success.";
        public const string HOMEROOM_TEACHER_ASSIGNED = "Homeroom teacher was assigned to other class.";
        public const string HOMEROOM_TEACHER_LIMIT = "Each teacher can only assign to a class.";
        public const string HOMEROOM_ASSIGN_SUCCESS = "Homeroom teacher assign success";
        public const string GET_SUBJECT_IN_CLASS_SUCCESS = "Get subject in classgroup success.";
        public const string CURRICULUM_ASSIGN_SUCCESS = "The curriculum assign success.";
        public const string CURRICULUM_GRADE_MISMATCH = "The curriculum and student class group is different about grade.";

        //School Year
        public const string SCHOOL_YEAR_NOT_EXIST = "The school year does not exist.";
        public const string GET_SCHOOL_YEAR_SUCCESS = "Get school year success.";

        //Teacher
        public const string TEACHER_NOT_EXIST = "The teacher does not exist.";
        public const string TEACHER_ABBREVIATION_NOT_EXIST = "The teacher abbreviation does not exist.";
        public const string TEACHER_ID_DUPLICATED = "The teacher id is duplicated.";
        public const string TEACHER_EMAIL_EXISTED = "The teacher email already exists.";
        public const string ADD_TEACHER_SUCCESS = "Add teacher success.";
        public const string UPDATE_TEACHER_SUCCESS = "Update teacher success.";
        public const string DELETE_TEACHER_SUCCESS = "Delete teacher success.";
        public const string DELETE_TEACHER_FAILED = "Xóa giáo viên thất bại vì giáo viên đang chủ nhiệm.";
        public const string GET_TEACHER_SUCCESS = "Get teachers success.";
        public const string GENERATE_TEACHER_HAS_EXISTED = "Teacher already existed with teacher account.";
        public const string GENERATE_TEACHER_SUCCESS = "Generate teacher account success.";

        //Grade
        public const string GRADE_NOT_EXIST = "The grade does not exist.";
        public const string GRADE_CODE_NOT_EXIST = "The grade code does not exist.";

        //Department
        public const string DEPARTMENT_NOT_EXIST = "The department does not exist.";
        public const string ADD_DEPARTMENT_SUCCESS = "Add department success.";
        public const string UPDATE_DEPARTMENT_SUCCESS = "Update department success.";
        public const string GET_DEPARTMENT_SUCCESS = "Get departments success.";
        public const string DELETE_DEPARTMENT_SUCCESS = "Delete department success.";
        public const string DEPARTMENT_NAME_DUPLICATE = "Department name is dupicated.";
        public const string DEPARTMENT_CODE_DUPLICATE = "Department code is dupicated.";
        public const string DEPARTMENT_NAME_OR_CODE_EXISTED = "Department name or code does existed.";
        public const string GENERATE_TEACHER_IN_DEPARTMENT_FAILED = "Teacher invalid to generate teacher account.";
        public const string GENERATE_TEACHER_IN_DEPARTMENT_SUCCESS = "Generate teacher in department success.";

        //TeacherAssignment
        public const string TEACHER_ASSIGNMENT_NOT_EXIST = "The teaching assignment does not exist.";
        public const string ADD_TEACHER_ASSIGNMENT_SUCCESS = "Add teaching assignment success.";
        public const string UPDATE_TEACHER_ASSIGNMENT_SUCCESS = "Update teaching assignment success.";
        public const string GET_TEACHER_ASSIGNTMENT_SUCCESS = "Get teacher assignment success.";
        public const string INVALID_NUMBER_MAIN_SUBJECR_OF_TEACHER = "Each teacher only have 1 main subject.";

        //TeachableSubject
        public const string TEACHABLE_SUBJECT_NOT_EXIST = "The teachable subject does not exist.";
        public const string GET_TEACHABLE_SUBJECT_SUCCESS = "Get teachable subject success.";
        public const string GET_TEACHABLE_BY_SUBJECT_FAILED = "School has no teacher can teach the subject in this grade.";
        public const string DELETE_TEACHABLE_SUBJECT_SUCCESS = "Xóa môn có thể dạy của giáo viên thành công.";

        //Term
        public const string TERM_NOT_EXIST = "The term does not exist.";

        //Student class and student class in group
        public const string STUDENT_CLASS_NOT_EXIST = "The student class not exist.";
        public const string STUDENT_CLASS_GROUP_NOT_EXIST = "The student class group not exist.";
        public const string STUDENT_CLASS_NOT_HAVE_ASSIGNMENT = "The student class not have assignment.";
        public const string GET_STUDENT_CLASS_ASSIGNMENT_SUCCESS = "Get student class assignments success.";
        public const string INVALID_UPDATE_GRADE_DIFFERENT_STUDENT_CLASS_GROUP_V1 = "Không thể cập nhật khối vì khác khối của các lớp hiện tại trong nhóm lớp.";


        //Term
        public const string TERM_NOT_FOUND = "Term not exist.";
        public const string GET_TERM_SUCCESS = "Get term success.";
        public const string CREATE_TERM_SUCCESS = "Create term success.";
        public const string UPDATE_TERM_SUCCESS = "Update the term success.";

        //Timetable
        public const string TIMETABLE_NOT_FOUND = "The timetable does not exist.";
        public const string GET_TIMETABLE_SUCCESS = "Get the timetable success.";

        //Notification
        public const string GET_NOTIFICATION_NOT_EXIST = "Do not have any notification.";
        public const string GET_NOTIFICATION_SUCCESS = "Get notification success.";
        public const string GET_NOT_READ_NOTIFICATION_SUCCESS = "Get unread notification success.";
        public const string IS_READ_SUCCESS = "Notification is read.";


        //Student Class Group
        public const string ADD_STUDENT_CLASS_GROUP_SUCCESS = "Add student class group success.";
        public const string GET_STUDENT_CLASS_GROUP_SUCCESS = "Get student class group success.";
        public const string UPDATE_STUDENT_CLASS_GROUP_SUCCESS = "Update student class group success.";
        public const string DELETE_STUDENT_CLASS_GROUP_SUCCESS = "Delete student class group success.";
        public const string DELETE_STUDENT_CLASS_GROUP_FAILED = "Xóa nhóm lớp thất bại do đang có lớp học bên trong.";
        public const string STUDENT_CLASS_GROUP_NAME_OR_CODE_EXISTED = "Student class group name or code does existed.";
        public const string STUDENT_CLASS_GROUP_NAME_OR_CODE_DUPLICATED = "Student class group name or code does douplicated.";
        public const string STUDENT_CLASS_GROUP_NOT_FOUND = "Student class group does not found.";

        //Room subject
        public const string ADD_ROOM_SUBJECT_SUCCESS = "Thêm lớp ghép thành công.";
        public const string ROOM_SUBJECT_NAME_OR_CODE_EXIST = "Tên hoặc mã lớp gộp đã tồn tại trong hệ thống.";
        public const string ROOM_CAPILITY_NOT_ENOUGH = "Số lượng lớp vượt quá sức chứa của phòng.";

    }
}
