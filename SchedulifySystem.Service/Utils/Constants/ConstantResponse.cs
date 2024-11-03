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
        public const string GET_ROOM_SUCCESS = "Get rooms success.";
        public const string ROOM_TYPE_BAD_REQUEST = "Each pactical room require at least one subjects abreviation and it exists in database.";

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
        public const string GET_BUILDING_SUCCESS = "Get buildings success.";

        //subject group type
        public const string SUBJECT_GROUP_TYPE_NOT_EXISTED = "Subject group type not exist.";

        //subject group
        public const string SUBJECT_GROUP_NAME_OR_CODE_EXISTED = "Subject group name or code already exist.";
        public const string ADD_SUBJECT_GROUP_SUCCESS = "Add subject group success.";
        public const string UPDATE_SUBJECT_GROUP_SUCCESS = "Update subject group success.";
        public const string DELETE_SUBJECT_GROUP_SUCCESS = "Delete subject group success.";
        public const string SUBJECT_GROUP_NOT_EXISTED = "Subject group not exist.";
        public const string GET_SUBJECT_GROUP_LIST_FAILED = "Get subject group list has no items";
        public const string GET_SUBJECT_GROUP_LIST_SUCCESS = "Get subject group list successful";
        public const string SUBJECT_GROUP_HAS_SUBJECTS_REGISTERED = "Subject group has assign, can not update grade.";
        public const string INVALID_NUMBER_SUBJECT = "Để tạo tổ hợp môn cần chọn 4 môn tự chọn và 3 môn chuyên đề.";

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
        public const string GET_SUBJECT_GROUP_DETAIL_SUCCESS = "Get subject group detail success.";
        public const string REQUIRE_ELECTIVE_SUBJECT = "Môn lựa chọn phải khác môn bắt buộc.";
        public const string INVALID_SPECIALIZED_SUBJECT = "Môn chuyên đề phải nằm trong nhóm môn bắt buộc hoặc là trong tổ hợp môn";
        public const string DELETE_SUBJECT_SUCCESS = "Delete subject success.";

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
        public const string SUBJECT_GROUP_ASSIGN_SUCCESS = "The subject group assign success.";

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
        public const string GET_TEACHER_SUCCESS = "Get teachers success.";

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

        //TeacherAssignment
        public const string TEACHER_ASSIGNMENT_NOT_EXIST = "The teaching assignment does not exist.";
        public const string ADD_TEACHER_ASSIGNMENT_SUCCESS = "Add teaching assignment success.";
        public const string UPDATE_TEACHER_ASSIGNMENT_SUCCESS = "Update teaching assignment success.";
        public const string GET_TEACHER_ASSIGNTMENT_SUCCESS = "Get teacher assignment success.";

        //TeachableSubject
        public const string TEACHABLE_SUBJECT_NOT_EXIST = "The teachable subject does not exist.";
        public const string GET_TEACHABLE_SUBJECT_SUCCESS = "Get teachable subject success.";

        //Term
        public const string TERM_NOT_EXIST = "The term does not exist.";

        //Student class and student class in group
        public const string STUDENT_CLASS_NOT_EXIST = "The student class not exist.";

        //Term
        public const string TERM_NOT_FOUND = "Term not exist.";
        public const string GET_TERM_SUCCESS = "Get term success.";
        public const string CREATE_TERM_SUCCESS = "Create term success.";
        public const string UPDATE_TERM_SUCCESS = "Update the term success.";

        //Timetable
        public const string TIMETABLE_NOT_FOUND = "The timetable does not exist.";
        public const string GET_TIMETABLE_SUCCESS = "Get the timetable success.";

    }
}
