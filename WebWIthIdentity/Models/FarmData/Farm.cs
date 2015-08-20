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
using WebWIthIdentity.Controllers;
using WebWIthIdentity.Models.FarmData;

namespace WebWIthIdentity.Models
{
    public class Farm 
    {


        public Farm()
        {
            Disabled = false;
            Created = DateTime.Now;
            Bound = new List<Bond>();
            Fields = new List<Field>();
        }

        public Farm(string inName, string inDesctiption = "")
        {
            Name = inName;
            Description = inDesctiption;
            Disabled = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            Bound = new List<Bond>();
            Fields = new List<Field>();
        }


        public long Id { get; set; }

        public virtual List<Bond> Bound { get; set; }


        public virtual List<Field> Fields { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }


        /*house Number*/
        public int HouseNumber { get; set; }

        /*three lines for address, one should be mandatory*/
        public String Address1 { get; set; } //Not Null
        public String Address2 { get; set; }
        public String Address3 { get; set; }

        public String City { get; set; } //Not Null

        public String Country { get; set; } //Not Null

        public String Postcode { get; set; } //Not Null


        public bool Disabled { get; set; }


        public List<ApplicationUser> Owners()
        {
            return SelectBonds(BondType.Owner);
        }

    
        public List<ApplicationUser> Managers()
        {
            return SelectBonds(BondType.Manager);
        }


        public List<ApplicationUser> Agronomists()
        {
            return SelectBonds(BondType.Agrinomist);
        }


        public List<ApplicationUser> Crew()
        {
            return SelectBonds(BondType.Crew);
        }

        /// <summary>  Gets all the people working on this farm  </summary>
        public List<ApplicationUser> getAllPeople()
        {
            return SelectBonds();
        }

        private List<ApplicationUser> SelectBonds(BondType selectBondType = BondType.Any)
        {
            List<ApplicationUser> people = new List<ApplicationUser>();
            foreach (var bond in Bound)
            {
                if (bond.Type == selectBondType)
                {
                   people.Add(bond.Person);
                }
                else if (selectBondType == BondType.Any)
                {
                    people.Add(bond.Person);
                }
            }
            return people;
        }

    }

    public class FarmBindingModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "FieldDescription")]
        public string Description { get; set; }

    }


    public class FarmViewModel
    {
        public long Id { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public BondType bondType { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public List<FieldViewModel> Fields { get; set; }
        
        public List<RecordViewModel> Staff { get; set; }

        public static explicit operator FarmViewModel(Bond v)
        {
            var toReturn = new FarmViewModel
            {
                Id = v.Farm.Id,
                Name = v.Farm.Name,
                Description = v.Farm.Description,
                Created = v.Created,
                LastUpdated = v.Farm.LastUpdated,
                Fields = new List<FieldViewModel>(),
                Staff = new List<RecordViewModel>(),
                bondType = v.Type
            };

            foreach (var field in v.Farm.Fields)
            {
                toReturn.Fields.Add((FieldViewModel) field);
            }

            foreach (var bond in v.Farm.Bound)
            {
                toReturn.Staff.Add((RecordViewModel)bond);
            }

            return toReturn;
        }
    }
}