using HiveServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class TaskDTO : _Entity, IValidatableObject
    {
        public string name { get; set; }
        public string taskDescription { get; set; }

        public string type { get; set; }

        public virtual long forFieldID { get; set; }

        public virtual long assignedByID { get; set; }

        public virtual long assignedToID { get; set; }

        public DateTime completedOnDate { get; set; }

        public DateTime dueDate { get; set; }

        public virtual List<TaskEvent> eventLog { get; set; }

        public string state { get; set; }

        public double payRate { get; set; }

        /// <summary> Time taken for the job in Seconds </summary>
        public uint timeTaken { get; set; }


        public TaskDTO ()
        {
            eventLog = new List<TaskEvent>();
            
        }

        public static explicit operator TaskDTO(Models.TaskDb v)
        {
            TaskDTO dto =  new TaskDTO
            {
                id = v.Id,
                name = v.Name,
                taskDescription = v.TaskDescription,

                state = v.State,
                payRate = v.PayRate,
                type = v.Type,
                dueDate = v.DueDate, 
                assignedByID = v.AssignedByID,
                assignedToID = v.AssignedToID,
                timeTaken = v.TimeTaken,
                completedOnDate = v.DateFinished,

                forFieldID = v.ForFieldID,
                createdOn = v.CreatedOn,
                updatedOn = v.UpdatedOn,
                version = v.Version,
                markedDeleted = v.MarkedDeleted
            };


            dto.eventLog = JsonConvert.DeserializeObject<List<TaskEvent>>(v.EventLog) ?? null;
            

            return dto; 
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(name))
            {
                yield return new ValidationResult("Provide name for the Job please");
            }
            if (string.IsNullOrEmpty(state))
            {
                yield return new ValidationResult("You have to spesify the current state");
            }
            if (string.IsNullOrEmpty(type))
            {
                yield return new ValidationResult("You have to spesify the Job type");
            }
            if (string.IsNullOrEmpty(taskDescription))
            {
                taskDescription = string.Empty;
            }
            if ((version == null || version.Count() < 5) && oldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
            if (forFieldID == 0)
            {
                yield return new ValidationResult("You must provide the field, where the job is assigned");
            }
            if (id == 0 && oldObject)
            {
                yield return new ValidationResult("You must provide the id");
            }
            if (assignedByID == 0)
            {
                yield return new ValidationResult("You must provide ID of the assigner");
            }
            if (assignedToID == 0)
            {
                yield return new ValidationResult("You must provide ID of the assignee");
            }
            if(TaskDb.ValidStates.Contains(state))
            {
                yield return new ValidationResult("You did not provide a valid state");
            }
            if (timeTaken == null || timeTaken > 31000000)
            {
                yield return new ValidationResult("You must spesify time spent and it must be sane");
            }
        }

    }
}