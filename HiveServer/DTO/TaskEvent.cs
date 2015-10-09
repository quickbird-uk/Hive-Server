using HiveServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class TaskEvent
    {
        public TaskEvent(TaskDTO job, long userId, string title = "Changes where made by")
        {
            Title = title;
            Timestamp = DateTime.UtcNow;
            ChangedBy = userId;

            Rate = job.rate;
            AssignedTo = job.assignedToId;
            Type = job.type;
            Name = job.name;
            JobDescription = job.jobDescription;
            Deleted = job.Deleted;
        }

        public string Title { get; set; }


        /// <summary> Id the the person that made these changes </summary>
        public long ChangedBy { get; set; }

        public double Rate { get; set; }

        public long AssignedTo { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string JobDescription { get; set; }

        public bool Deleted { get; set; }

        public TimeSpan TimeSpent { get; set; }
        public DateTime Timestamp { get; set; }


    }
}