﻿using System;
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

        //School Year
        public const string SCHOOL_YEAR_NOT_EXIST = "The school year does not exist.";

        //Teacher
        public const string TEACHER_NOT_EXIST = "The teacher does not exist.";
        public const string TEACHER_ABBREVIATION_NOT_EXIST = "The teacher abbreviation does not exist.";
        public const string TEACHER_ID_DUPLICATED = "The teacher id is duplicated";

        //Grade
        public const string GRADE_NOT_EXIST = "The grade does not exist.";
        public const string GRADE_CODE_NOT_EXIST = "The grade code does not exist.";
    }
}