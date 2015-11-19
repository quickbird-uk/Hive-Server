using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.DTO
{
    public class Organisation: _Entity ,  IValidatableObject
    {
        public string name { get; set; }

        public string orgDescription { get; set; }

        public string role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                yield return new ValidationResult("Provide name for the organisation please");
            }
            if (string.IsNullOrEmpty(orgDescription))
            {
                orgDescription = string.Empty;
            }
            if ((version == null || version.Count() < 5) && oldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
        }

        public static explicit operator Organisation(Models.FarmData.BondDb v)
        {
            var result = new Organisation
            {
                id = v.Organisation.Id,
                name = v.Organisation.Name,
                orgDescription = v.Organisation.Description,
                createdOn = v.Organisation.CreatedOn,
                updatedOn = v.Organisation.UpdatedOn,
                role = v.Role,
                version = v.Version,
                markedDeleted = v.Organisation.MarkedDeleted
            };
            return result;
        }

    }
}