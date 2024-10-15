using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils.Constants
{
    public static class ConstantResponse
    {
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

        //Room
        public const string ROOM_NOT_EXIST = "The room is not exist.";
        public const string ROOM_NAME_EXISTED = "The room name is existed.";
        public const string ROOM_NAME_DUPLICATED = "The room name is duplicated";
        public const string ROOM_CODE_EXISTED = "The room code is existed.";
        public const string ROOM_CODE_DUPLICATED = "The room code is duplicated";
        public const string ROOM_CODE_OR_NAME_EXISTED = "The room code or room name is existed.";
        public const string ADD_ROOM_SUCCESS = "Add room success.";
        public const string UPDATE_ROOM_SUCCESS = "Update room success.";
        public const string DELETE_ROOM_SUCCESS = "Delete room success.";
        public const string GET_ROOM_SUCCESS = "Get room success.";

        //Building
        public const string BUILDING_NOT_EXIST = "The building is not exist.";
        public const string BUILDING_CODE_NOT_EXIST = "The building code is not exist.";


        //Room Type
        public const string ROOM_TYPE_NOT_EXIST = "The room type is not exist.";
        public const string ROOM_TYPE_CODE_NOT_EXIST = "The room type code is not exist.";
    }
}
