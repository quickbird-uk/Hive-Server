using HiveServer.DTO;
using HiveServer.Models;
using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace HiveServer.DTO
{
      
    public class Contact: Base.Person, IValidatableObject
    {
        public long personID { get; set; }
        //State of the contact, such as active, blocked, ets. 
        public string state { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(state))
            {
                yield return new ValidationResult("Provide state for the contact");
            }
            

            if (Id == 0)
            {
                yield return new ValidationResult("Pleave provide ID for the Contact entity");
            }

            if (personID == 0)
            {
                yield return new ValidationResult("Pleave provide ID for the relevant person");
            }

            if (Version == null || Version.Count() < 5)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }

            if(! ContactDb.ValidStates.Contains(state))
            {
                yield return new ValidationResult("You have not supplied a valid state");
            }
        }
    }
}