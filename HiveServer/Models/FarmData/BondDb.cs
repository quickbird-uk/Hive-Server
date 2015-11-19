using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.Models.FarmData
{
    /// <summary>
    /// Bidnings link people to Organisations, the type of binding dictates if it's an agicultural worker or owner of the property
    /// </summary>
    public class BondDb : _Entity
    {
        public BondDb()
        {
        }

        /// <summary>
        /// Recommended constructor to use when creating a bond through a user
        /// </summary>
        /// <param name="Organisation"></param>
        /// <param name="BondType"></param>
        public BondDb(OrganisationDb organisation, string role)
        {
            Organisation = organisation;
            Role = role;

        }

        public long PersonID { get; set; }

        public virtual ApplicationUser Person { get; set; }

        public long OrganisationID { get; set; }

        public virtual OrganisationDb Organisation { get; set; }

        /// <summary> Explains the relationship between the person and the Organisation. THere are Mangers, Agronomists and Crew. ALl have different privilages.  </summary>
        public string Role { get; set; }

        public const string RoleOwner = "Own";
        public const string RoleManager = "Man";
        public const string RoleSpecialist = "Spec";
        public const string RoleCrew = "Crew";
        public const string RoleAny = "Any";
        public const string RoleNone = "NA";
        public static readonly string[] ValidStates = { RoleOwner, RoleManager, RoleSpecialist, RoleCrew };

        public static readonly string[] CanAssignJobsToOthers = { RoleOwner, RoleManager, RoleSpecialist };
        public static readonly string[] CanManageStaff = { RoleOwner, RoleManager};
        public static readonly string[] CanEditOrganisation = { RoleOwner};

    }

}