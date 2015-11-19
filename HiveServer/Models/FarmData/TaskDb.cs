using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;




namespace HiveServer.Models
{

 
    public enum TaskState 
    {
        Pending = "Pending",
        Assigned = "Assigned",
        InProgress = "In Progress",
        Paused = "Paused",
        Finished = "Finished"

        // Might be tempting to use attributes instead of 
        // specifying a base type for the Enum but it's poor
        // design. something like this
        //
        // enum TaskState {
        //	[StringValue("Pending")]
        //	Pending = 1,
        //  [StringValue("Assigned")]
        //  Assigned = 2,
        //  .
        //  .
        //  .
        //  This is very slow because it uses reflection to read the 
        //  attribute value. 
    };

    public class TaskDb : _Entity
    {

        public TaskDb()
        {

        }

        public string Name { get; set; }
        public string TaskDescription { get; set; }

        public string Type { get; set; }

        public virtual FieldDb  ForField { get; set; }
        public virtual long ForFieldID { get; set; }

        public virtual ApplicationUser AssignedBy { get; set; }
        public virtual long AssignedByID { get; set; }

        public virtual ApplicationUser AssignedTo { get; set; }
        public virtual long AssignedToID { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime DateFinished { get; set; }

        public string State { get; set; }

        public string EventLog { get; set; }

        public double PayRate { get; set; }

        public uint TimeTaken { get; set; }

        public static string StatePending = "Pen";
        public static string StateAssigned = "Ass";
        public static string StateInProgress = "InP";
        public static string StatePaused = "Pau";
        public static string StateFinished= "Fin";

        public static readonly string[] ValidStates = { StatePending, StateAssigned, StateInProgress, StatePaused, StateFinished };
    }

  
}

