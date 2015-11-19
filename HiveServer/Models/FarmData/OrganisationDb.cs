using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Microsoft.Owin.BuilderProperties;
using HiveServer.Controllers;
using HiveServer.Models.FarmData;
using HiveServer.DTO;

namespace HiveServer.Models
{
    public class OrganisationDb : _Entity
    {


        public OrganisationDb()
        {
            Bonds = new List<BondDb>();
            Fields = new List<FieldDb>();
        }

        public OrganisationDb(string inName, string inDesctiption = "")
        {
            Name = inName;
            Description = inDesctiption;
            Bonds = new List<BondDb>();
            Fields = new List<FieldDb>();
        }

        public virtual List<BondDb> Bonds { get; set; }


        public virtual List<FieldDb> Fields { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }





        public List<ApplicationUser> Owners()
        {
            return SelectBonds(BondDb.RoleOwner);
        }

    
        public List<ApplicationUser> Managers()
        {
            return SelectBonds(BondDb.RoleManager);
        }


        public List<ApplicationUser> Agronomists()
        {
            return SelectBonds(BondDb.RoleSpecialist);
        }


        public List<ApplicationUser> Crew()
        {
            return SelectBonds(BondDb.RoleCrew);
        }

        /// <summary>  Gets all the people working for this organisation  </summary>
        public List<ApplicationUser> getAllPeople()
        {
            return SelectBonds();
        }

        private List<ApplicationUser> SelectBonds(string selectBondType = BondDb.RoleAny)
        {
            List<ApplicationUser> people = new List<ApplicationUser>();
            foreach (var bond in Bonds)
            {
                if (bond.Role == selectBondType)
                {
                   people.Add(bond.Person);
                }
                else if (selectBondType == BondDb.RoleAny)
                {
                    people.Add(bond.Person);
                }
            }
            return people;
        }

    }       
    
}