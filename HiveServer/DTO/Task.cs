using HiveServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class TaskDTO : Base.Entity, IValidatableObject
    {
        public string name { get; set; }
        public string jobDescription { get; set; }

        public string type { get; set; }

        public virtual long onFieldId { get; set; }

        public virtual long assignedById { get; set; }

        public virtual long assignedToId { get; set; }

        public DateTime DateFinished { get; set; }

        public DateTime DueDate { get; set; }

        public virtual List<TaskEvent> Events { get; set; }

        public string state { get; set; }

        public double rate { get; set; }

        public TimeSpan timeSpent { get; set; }


        public TaskDTO ()
        {
            Events = new List<TaskEvent>();
            
        }

        public static explicit operator TaskDTO(Models.TaskDb v)
        {
            TaskDTO dto =  new TaskDTO
            {
                Id = v.Id,
                name = v.name,
                jobDescription = v.jobDescription,

                state = v.state,
                rate = v.rate,
                type = v.type,
                DueDate = v.DueDate, 
                assignedById = v.assignedById,
                assignedToId = v.assignedToId,
                timeSpent = v.timeSpent,
                DateFinished = v.DateFinished,

                onFieldId = v.onFieldId,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                Version = v.Version,
                Deleted = v.Deleted
            };


            dto.Events = JsonConvert.DeserializeObject<List<TaskEvent>>(v.EventLog) ?? null;
            

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
            if (string.IsNullOrEmpty(jobDescription))
            {
                jobDescription = string.Empty;
            }
            if ((Version == null || Version.Count() < 5) && OldObject)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
            if (onFieldId == 0)
            {
                yield return new ValidationResult("You must provide the field, where the job is assigned");
            }
            if (Id == 0 && OldObject)
            {
                yield return new ValidationResult("You must provide the id");
            }
            if (assignedById == 0)
            {
                yield return new ValidationResult("You must provide ID of the assigner");
            }
            if (assignedToId == 0)
            {
                yield return new ValidationResult("You must provide ID of the assignee");
            }
            if(TaskDb.ValidStates.Contains(state))
            {
                yield return new ValidationResult("You did not provide a valid state");
            }
            if (timeSpent == null || timeSpent > new TimeSpan(2,0,0,0,0))
            {
                yield return new ValidationResult("You must spesify time spent and it must be sane");
            }
        }

    }
}