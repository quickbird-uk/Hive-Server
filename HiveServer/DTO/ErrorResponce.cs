using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    /// <summary>
    /// Contains Errors that are dispatched to the client!
    ///  Constructor is private, that way no unexpected erros are created elsewehre through the applciation. All the types must be described here 
    /// </summary>
    public class ErrorResponse
    {
        public int error { get; set; }
        public string error_description { get; set; }

        /// <summary>
        ///  Constructor is private, that way no unexpected erros are created elsewehre through the applciation. All the types must be described here 
        /// </summary>
        private ErrorResponse ()
        {

        }

        /// <summary>
        /// Returns an ErrorResponse with correct error code and adds description of the error.
        /// </summary>
        /// <param name="errors">A formatted list of errors that occured throughout vlaidation.
       ///  They should be formatted in some manner iwthing a single string</param>
        /// <returns></returns>
        public static ErrorResponse CreateModelError(string errors)
        {
            ErrorResponse reponse = new ErrorResponse { error = ModelErrors.error, error_description = ModelErrors.error_description };
            reponse.error_description += errors;
            return reponse; 
        }

        //Login and Registration errors
        public static readonly ErrorResponse PhoneTaken = new ErrorResponse { error = 101, error_description = "user with this phone number already exists"};
        public static readonly ErrorResponse EmailTaken = new ErrorResponse { error = 102, error_description = "user with this email already exists" };
        public static readonly ErrorResponse CantLogin = new ErrorResponse { error = 103, error_description = "Login Details are incorrect" };
        public static readonly ErrorResponse PasswordIsBad = new ErrorResponse { error = 104, error_description = "This password is not good enough or invalid" };

        //Data Errors
        public static readonly ErrorResponse CantView = new ErrorResponse { error = 201, error_description = "User is not authorised to view this data" };
        public static readonly ErrorResponse CantEdit = new ErrorResponse { error = 202, error_description = "User is not authorised to edit this data" };
        public static readonly ErrorResponse IllegalChanges = new ErrorResponse { error = 203, error_description = "Attempted changes are illegal" };
        public static readonly ErrorResponse DoesntExist = new ErrorResponse { error = 204, error_description = "The item you attemped to access does not exist" };
        public static readonly ErrorResponse NoDataRecieved = new ErrorResponse { error = 205, error_description = "Server recieved no data from client"};
        private static readonly ErrorResponse ModelErrors = new ErrorResponse { error = 206, error_description = "Model has given following errors:" };
        public static readonly ErrorResponse CantOverrite = new ErrorResponse { error = 207, error_description = "You attempted to create a new item, but an identical item already exists. Edit the existing item instead" };
        public static readonly ErrorResponse PermissionsTooLow = new ErrorResponse { error = 208, error_description = "You lack permissions to do this, but a user with higer access level to this item could do this" };

        public static readonly ErrorResponse AbuseWarning = new ErrorResponse { error = 700, error_description = "Your Usage of the API is considered abusive. Thisusually happens when you send requests that are too large , or too frequently. Continue doing so at your peril! " };

        public static readonly ErrorResponse SomethingHappened= new ErrorResponse { error = 999, error_description = "SomethingHappened - no clue what" };

        //public static implicit operator string (ErrorResponse error)
        //{

        //    return error.ToString(); 
        //}
    }
}