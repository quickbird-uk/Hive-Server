using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebWIthIdentity.Models.FarmData
{
    public class Field
    {

        public Field()
        {
            disabled = false;
            created = DateTime.Now;

        }

        public Field(string inName, string inDesctiption = "", double inLatt = 0, double inLong = 0)
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

        public String name { get; set; }
        public String description { get; set; }
        public double lattitude { get; set; }
        public double longitude { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }

        public string ParcelNumber { get; set; }

        public bool disabled { get; set; }

    }
}