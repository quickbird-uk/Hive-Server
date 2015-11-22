using HiveServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class TaskEvent
    {
        public TaskEvent()
        {

        }

        public TaskEvent(TaskDTO job, long userId, string title = "Changes where made by")
        {
            Title = title;
            Timestamp = DateTime.UtcNow;
            changedPersonID = userId;

            rate = job.payRate;
            assignedToID = job.assignedToID;
            type = job.type;
            name = job.name;
            taskDescription = job.taskDescription;
            deleted = job.markedDeleted;
            timeTaken = job.timeTaken;
        }

        public string Title { get; set; }


        /// <summary> Id the the person that made these changes </summary>
        public long changedPersonID { get; set; }

        public double rate { get; set; }

        public long assignedToID { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        public string taskDescription { get; set; }

        public bool deleted { get; set; }

        public long timeTaken { get; set; }

        public DateTimeOffset Timestamp { get; set; }


    }
}