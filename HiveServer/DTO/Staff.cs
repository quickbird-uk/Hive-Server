using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HiveServer.DTO
{
      
    public class Staff: _Person, IValidatableObject
    {

        /// <summary> Id of the staff is the ID of the relationship between a person and a organisation. IF a person A is assigned to organisation X, 
        /// he will show up with a PersonID of A on both organisations, but with a different StaffId
        /// </summary>
        /// 
        public long personID{ get; set; }

        public long onOrganisationID { get; set; }

        /// <summary> Person's role at the organisation       /// </summary>
        public string role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(role))
            {
                yield return new ValidationResult("Provide the person's role please");
            }
            if (onOrganisationID == 0)
            {
                yield return new ValidationResult("Provide organisationID please");
            }
            if (personID == 0)
            {
                yield return new ValidationResult("you must provide person ID");
            }
            if ((version == null || version.Count() < 5) && oldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }

            if(! BondDb.ValidStates.Contains(role))
            {
                yield return new ValidationResult("You did not spesify a role, or it's invalid");
            }
        }


        public static explicit operator Staff(Models.FarmData.BondDb v)
        {
            var result = new Staff
            {
                id = v.Id,
                personID = v.PersonID,
                firstName = v.Person.FirstName,
                lastName = v.Person.LastName,
                phone = v.Person.PhoneNumber,
                onOrganisationID = v.OrganisationID,
                createdOn = v.CreatedOn,
                updatedOn = v.UpdatedOn,
                role = v.Role,
                version = v.Version,
                markedDeleted = v.MarkedDeleted
            };
            return result;
        }
    }
}