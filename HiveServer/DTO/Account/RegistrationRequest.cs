using HiveServer.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class RegistrationRequest : Base.Person,  IValidatableObject
    {

        [Required]
        public String Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (phone < 1000000000)
            {
                yield return new ValidationResult("Provide valid Phone Number");
            }
            else if (string.IsNullOrEmpty(Password) || Password?.Length < 9)
            {
                yield return new ValidationResult("Password is not provided or too short");
            }
            else if (string.IsNullOrEmpty(firstName) || firstName?.Length < 3 || firstName?.Length > 100)
            {
                yield return new ValidationResult("Please provide a proper first name");
            }
            else if (string.IsNullOrEmpty(lastName) || lastName?.Length < 3 || lastName?.Length > 100)
            {
                yield return new ValidationResult("Please provide a proper last name");
            }
        }
    }
}