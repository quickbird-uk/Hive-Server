using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace HiveServer.DTO
{
      
    public class Staff: Base.Person
    {

        /// <summary> Id of the staff isthe ID of the relationship between a person and a farm. IF a person A is assigned to farm X, 
        /// he will show up with a PersonID of A on both farms, but with a different StaffId
        /// </summary>
        /// 
        public long personID{ get; set; }

        public long atFarmID { get; set; }

        /// <summary> Person's role at the farm       /// </summary>
        public string role { get; set; }


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