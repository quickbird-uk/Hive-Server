using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.DTO
{
    public class Farm: Base.Entity ,  IValidatableObject
    {
        public string name { get; set; }

        public string farmDescription { get; set; }

        public string role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                yield return new ValidationResult("Provide name for the farm please");
            }
            if (string.IsNullOrEmpty(farmDescription))
            {
                farmDescription = string.Empty;
            }
            if (Version == null || Version.Count() < 5)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
        }

        public static explicit operator Farm(Models.FarmData.BondDb v)
        {
            var result = new Farm
            {
                Id = v.Farm.Id,
                name = v.Farm.Name,
                farmDescription = v.Farm.Description,
                CreatedAt = v.Farm.CreatedAt,
                UpdatedAt = v.Farm.UpdatedAt,
                role = v.Role,
                Version = v.Version,
                Deleted = v.Farm.Deleted
            };
            return result;
        }

    }
}