using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    /// <summary>  Theis is a model used to sign in the person. The user's phone number should be in  </summary>
    public class LoginRequest : IValidatableObject
    {
        [Required]
        public long phone { get; set; }
        [Required]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (phone < 1000000000 )
            {
                yield return new ValidationResult("Provide valid Phone Number");
            }
            else if (string.IsNullOrEmpty(Password) || Password?.Length < 7)
            {
                yield return new ValidationResult("Password is not provided or too short");
            }
        }
    }


}