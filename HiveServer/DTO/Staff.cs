using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HiveServer.DTO
{
      
    public class Staff: Base.Person, IValidatableObject
    {

        /// <summary> Id of the staff isthe ID of the relationship between a person and a farm. IF a person A is assigned to farm X, 
        /// he will show up with a PersonID of A on both farms, but with a different StaffId
        /// </summary>
        /// 
        public long personID{ get; set; }

        public long atFarmID { get; set; }

        /// <summary> Person's role at the farm       /// </summary>
        public string role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(role))
            {
                yield return new ValidationResult("Provide the person's role please");
            }
            if (atFarmID == 0)
            {
                yield return new ValidationResult("Provide farmID please");
            }
            if (personID == 0)
            {
                yield return new ValidationResult("you must provide person ID");
            }
            if (Version == null || Version.Count() < 5)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
        }


        public static explicit operator Staff(Models.FarmData.BondDb v)
        {
            var result = new Staff
            {
                Id = v.Id,
                personID = v.PersonID,
                firstName = v.Person.FirstName,
                lastName = v.Person.LastName,
                phone = v.Person.PhoneNumber,
                atFarmID = v.FarmID,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                role = v.Role,
                Version = v.Version,
                Deleted = v.Deleted
            };
            return result;
        }
    }
}