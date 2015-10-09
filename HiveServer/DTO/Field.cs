using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class Field : Base.Entity, IValidatableObject
    {
        public string name { get; set; }

        public double size { get; set; }

        public string fieldDescription { get; set; }

        public long onOrg { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                yield return new ValidationResult("Provide name for the field please");
            }
            if (string.IsNullOrEmpty(fieldDescription))
            {
                fieldDescription = string.Empty;
            }
            if (Version == null || Version.Count() < 5)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
            if (onOrg == 0)
            {
                yield return new ValidationResult("You must provide the organisation, to which the field is attached");
            }
            if (Id == 0)
            {
                yield return new ValidationResult("You must provide the id");
            }
           
        }


        public static explicit operator Field(FieldDb v)
        {
            return new Field
            {
                Id = v.Id,
                onOrg = v.OrgId,
                name = v.Name,
                size = v.size,
                fieldDescription = v.FieldDescription,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                Version = v.Version,
                Deleted = v.Deleted               
            };
        }

    }
}