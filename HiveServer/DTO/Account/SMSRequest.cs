using HiveServer.Base;
using HiveServer.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class SMSRequest :  IValidatableObject
    {
        public string lastName { get; set; }


        public long phone { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                yield return new ValidationResult("Provide your last name please");
            }
            else if (lastName.Length > 100)
            {
                lastName = lastName.Substring(0, 100);
            }

            if (phone < 1000000000)
            {
                yield return new ValidationResult("Provide valid Phone Number");
            }
        }
    }
}