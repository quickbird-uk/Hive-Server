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
            long me = DateTime.UtcNow.ToBinary(); 


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

        public DateTime CompleteOn { get; set; }

        public string state { get; set; }

        public string EventLog { get; set; }

        public double rate { get; set; }

        public static string StateVanilla = "Van";
        public static string StateOrphan = "Orf";
        public static string StateAssigned = "Ass";
        public static string StateInProgress = "InP";
        public static string StatePaused = "Pau";
        public static string StateFinished= "Fin";

        public static readonly string[] ValidStates = { StateVanilla, StateOrphan, StateAssigned, StateInProgress, StatePaused, StateFinished };
    }

  
}

