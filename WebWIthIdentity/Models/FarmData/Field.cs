using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebWIthIdentity.Models.FarmData;

namespace WebWIthIdentity.Models.FarmData
{
    public class Field
    {

        public Field()
        {
            Disabled = false;
            Created = DateTime.Now;

        }

        public Field(string inName, string inDesctiption = "", double inLatt = 0, double inLong = 0)
        {
            Name = inName;
            Description = inDesctiption;

            Disabled = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;

        }

        
        public long Id { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }

        public string ParcelNumber { get; set; }

        public bool Disabled { get; set; }

        public FieldViewModel ViewModel()
        {
            return (FieldViewModel) this;
        }
    }

  
}


public class FieldBindingModel
{
    public String Name { get; set; }
    public String Description { get; set; }

}

public class FieldViewModel
{

    public long Id { get; set; }

    public String Name { get; set; }

    public String Description { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }

    public string ParcelNumber { get; set; }

    public bool Disabled { get; set; }

    public static explicit operator FieldViewModel(Field v)
    {
        return new FieldViewModel
        {
            Id = v.Id,
            Name = v.Name,
            Description = v.Description,
            Created = v.Created,
            LastUpdated = v.LastUpdated,
            Disabled = v.Disabled
        }; 
    }
}