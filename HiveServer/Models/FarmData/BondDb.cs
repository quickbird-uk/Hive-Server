using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.Models.FarmData
{
    /// <summary>
    /// Bidnings link people to Farms, the type of binding dictates if it's an agicultural worker or owner of the property
    /// </summary>
    public class BondDb : Base.Entity
    {
        public BondDb()
        {
        }

        /// <summary>
        /// Recommended constructor to use when creating a bond through a user
        /// </summary>
        /// <param name="Farm"></param>
        /// <param name="BondType"></param>
        public BondDb(FarmDb farm, string role)
        {
            Farm = farm;
            Role = role;

        }

        public long PersonID { get; set; }

        public virtual ApplicationUser Person { get; set; }

        public long FarmID { get; set; }

        public virtual FarmDb Farm { get; set; }

        /// <summary> Explains the relationship between the person and the farm. THere are Mangers, Agronomists and Crew. ALl have different privilages.  </summary>
        public string Role { get; set; }

        public const string RoleOwner = "Own";
        public const string RoleManager = "Man";
        public const string RoleAgronomist = "Agro";
        public const string RoleCrew = "Crew";
        public const string RoleAny = "Any";
        public const string RoleNone = "NA";

    }

}