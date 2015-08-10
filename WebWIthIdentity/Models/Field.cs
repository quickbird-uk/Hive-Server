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
    public class Field
    {


        public Field()
        {
            disabled = false;
            created = DateTime.Now;
        }

        public Field(string inName, string inDesctiption = "", double inLatt = 0,  double inLong = 0)
        {
            name = inName;
            description = inDesctiption;
            lattitude = inLatt;
            longitude = inLong;
            disabled = false;
            created = DateTime.Now;
            lastUpdated = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }

        
        public string ApplicationUserId { get; set; }

       

        public String name { get; set; }
        public String description { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }



        public bool disabled { get; set; }

    }

    public class FieldBindingModel
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


    public class FieldViewModel
    {
        public long id { get; set; }

        public String name { get; set; }

        public String description { get; set; }

        public double longitude { get; set; }

        public double lattitude { get; set; }

        public DateTime created { get; set; }

        public DateTime lastUpdated { get; set; }
    }
}