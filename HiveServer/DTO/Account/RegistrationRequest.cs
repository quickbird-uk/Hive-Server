using HiveServer.Base;
using HiveServer.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class RegistrationRequest :  IValidatableObject
    {
        public string lastName { get; set; }

        public string firstName { get; set; }

        public long phone { get; set; }

        public String Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                yield return new ValidationResult("Povide your first name please");
            }
            else if(firstName.Length > 100)
            {
                firstName = firstName.Substring(0, 100);
            }

            if (string.IsNullOrEmpty(lastName))
            {
                yield return new ValidationResult("Povide your last name please");
            }
            else if (lastName.Length > 100)
            {
                lastName = lastName.Substring(0, 100);
            }

            if (phone < 1000000000)
            {
                yield return new ValidationResult("Provide valid Phone Number");
            }
            if (string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password is not provided or too short");
            }
            

        }
    }
}