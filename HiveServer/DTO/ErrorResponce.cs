using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    /// <summary>
    /// Contains Errors that are dispatched to the client! 
    /// </summary>
    public class ErrorResponse
    {
        public int code { get; set; }
        public string message { get; set; }

        //Login and Registration errors
        public static readonly ErrorResponse PhoneTaken = new ErrorResponse { code = 101, message = "user with this phone number already exists"};
        public static readonly ErrorResponse EmailTaken = new ErrorResponse { code = 102, message = "user with this email already exists" };
        public static readonly ErrorResponse CantLogin = new ErrorResponse { code = 103, message = "User or password is incorrect" };
        public static readonly ErrorResponse PasswordIsBad = new ErrorResponse { code = 104, message = "This password is not secure enough" };

        //Data Errors
        public static readonly ErrorResponse CantView = new ErrorResponse { code = 201, message = "User is not authorised to view this data" };
        public static readonly ErrorResponse CantEdit = new ErrorResponse { code = 202, message = "User is not authorised to edit this data" };
        public static readonly ErrorResponse IllegalChanges = new ErrorResponse { code = 203, message = "Attempted changes are illegal" };
        public static readonly ErrorResponse DoesntExist = new ErrorResponse { code = 204, message = "The item you attemped to access does not exist" };

    }
}