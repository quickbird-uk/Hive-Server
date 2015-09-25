using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HiveServer.Models.FarmData;

namespace HiveServer.Models.FarmData
{
    public class FieldDb : Base.Entity
    {
        public string Name { get; set; }

        public string FieldDescription { get; set; }

        public string ParcelNumber { get; set; }

        public virtual FarmDb OnFarm { get; set;}

        public long OnFarmId { get; set; }

        public virtual List<JobDb> Jobs { get; set; }

        public FieldDb()
        {
            Jobs = new List<JobDb>();
        }

    }

  
}
