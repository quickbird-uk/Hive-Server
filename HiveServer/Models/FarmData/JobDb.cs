using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HiveServer.Models
{
    public class JobDb : Base.Entity
    {

        public JobDb()
        {



        }

        public string name { get; set; }
        public string jobDescription { get; set; }

        public string type { get; set; }

        public virtual FieldDb  onField { get; set; }
        public virtual long onFieldId { get; set; }

        public virtual ApplicationUser assignedBy { get; set; }
        public virtual long assignedById { get; set; }

        public virtual ApplicationUser assignedTo { get; set; }
        public virtual long assignedToId { get; set; }

        public String state { get; set; }

        public String lastAction { get; set; }

        public double rate { get; set; }



    }

  
}

