using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.DTO
{
    public class Organisation: Base.Entity ,  IValidatableObject
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
            if ((Version == null || Version.Count() < 5) && OldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
        }

        public static explicit operator Organisation(Models.FarmData.BondDb v)
        {
            var result = new Organisation
            {
                Id = v.Organisation.Id,
                name = v.Organisation.Name,
                orgDescription = v.Organisation.Description,
                CreatedAt = v.Organisation.CreatedAt,
                UpdatedAt = v.Organisation.UpdatedAt,
                role = v.Role,
                Version = v.Version,
                Deleted = v.Organisation.Deleted
            };
            return result;
        }

    }
}