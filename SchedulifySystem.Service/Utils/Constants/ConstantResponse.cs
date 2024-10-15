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


        //Room Type
        public const string ROOM_TYPE_NOT_EXIST = "The room type does not exist.";
        public const string ROOM_TYPE_CODE_NOT_EXIST = "The room type code does not exist.";
    }
}
