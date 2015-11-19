using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HiveServer.Models.FarmData;

namespace HiveServer.Models.FarmData
{
    public class FieldDb : _Entity
    {
        public string Name { get; set; }

        public double AreaInHectares { get; set; }

        public string FieldDescription { get; set; }

        public string ParcelNumber { get; set; }

        public virtual OrganisationDb Org { get; set;}

        public long onOrganisationID { get; set; }

        public virtual List<TaskDb> Jobs { get; set; }

        public double Lattitude { get; set; }

        public double Longitude { get; set; }

        public FieldDb()
        {
            Jobs = new List<TaskDb>();
        }

    }

  
}
