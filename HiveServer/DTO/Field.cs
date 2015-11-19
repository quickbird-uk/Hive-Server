using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class Field : _Entity, IValidatableObject
    {
        public string name { get; set; }

        public double areaInHectares { get; set; }

        public string fieldDescription { get; set; }

        public long onOrganisationID { get; set; }

        public double lattitude { get; set; }

        public double longitude { get; set; }


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
            if ((version == null || version.Count() < 5) && oldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
            if (onOrganisationID == 0)
            {
                yield return new ValidationResult("You must provide the organisation, to which the field is attached");
            }
            if (id == 0 && oldObject)
            {
                yield return new ValidationResult("You must provide the id");
            }
           
        }


        public static explicit operator Field(FieldDb v)
        {
            return new Field
            {
                id = v.Id,
                onOrganisationID = v.onOrganisationID,
                name = v.Name,
                areaInHectares = v.AreaInHectares,
                fieldDescription = v.FieldDescription,
                createdOn = v.CreatedOn,
                updatedOn = v.UpdatedOn,
                version = v.Version,
                markedDeleted = v.MarkedDeleted,
                lattitude = v.Lattitude,
                longitude = v.Longitude
            };
        }

    }
}