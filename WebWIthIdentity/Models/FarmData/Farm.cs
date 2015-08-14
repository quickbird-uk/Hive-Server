using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Owin.BuilderProperties;

namespace WebWIthIdentity.Models
{
    public class Farm
    {


        public Farm()
        {
            disabled = false;
            created = DateTime.Now;
            Managers = new List<ApplicationUser>();
            Crew = new List<ApplicationUser>();
        }

        public Farm(string inName, string inDesctiption = "", double inLatt = 0,  double inLong = 0)
        {
            name = inName;
            description = inDesctiption;
            lattitude = inLatt;
            longitude = inLong;
            disabled = false;
            created = DateTime.Now;
            lastUpdated = DateTime.Now;
            Managers = new List<ApplicationUser>();
            Crew = new List<ApplicationUser>();
        }

        [Key]
        public long Id { get; set; }

        public virtual List<ApplicationUser> Managers { get; set; }
        public virtual List<ApplicationUser> Crew { get; set; }

        public String name { get; set; }
        public String description { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }


        /*house Number*/
        public int HouseNumber { get; set; }

        /*three lines for address, one should be mandatory*/
        public String Address1 { get; set; } //Not Null
        public String Address2 { get; set; }
        public String Address3 { get; set; }

        public String City { get; set; } //Not Null

        public String Country { get; set; } //Not Null

        public String Postcode { get; set; } //Not Null



        public bool disabled { get; set; }

    }

    public class FarmBindingModel
    {
        [Required]
        [Display(Name = "FieldName")]
        public string name { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "FieldDescription")]
        public string description { get; set; }

        [Display(Name = "Longitude")]
        public double longitude { get; set; }


        [Display(Name = "Lattitude")]
        public double lattitude { get; set; }

    }


    public class FarmViewModel
    {
        public long id { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public double longitude { get; set; }

        public double lattitude { get; set; }

        public DateTime created { get; set; }

        public DateTime lastUpdated { get; set; }
        
        public bool Owner { get; set; }
    }
}