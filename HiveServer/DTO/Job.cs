using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class Job : Base.Entity, IValidatableObject
    {
        public string name { get; set; }
        public string jobDescription { get; set; }

        public string type { get; set; }

        public virtual long onFieldId { get; set; }

        public virtual long assignedById { get; set; }

        public virtual long assignedToId { get; set; }

        public string state { get; set; }

        public string lastAction { get; set; }

        public double rate { get; set; }

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
            if (Version == null || Version.Count() < 5)
            {
                yield return new ValidationResult("Version information is missing or too short");
            }
            if (onFieldId == 0)
            {
                yield return new ValidationResult("You must provide the field, where the job is assigned");
            }
            if (Id == 0)
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

        }

    }
}