using HiveServer.Base;
using HiveServer.DTO;
using HiveServer.Models;
using HiveServer.Models.FarmData;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HiveServer.Controllers
{
    [Authorize]
    [RoutePrefix("Tasks")]
    public class TasksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private List<TaskEvent> eventLog = new List<TaskEvent>();
        private readonly bool[][] Permissions = new bool[8][];

        bool TaskManagementRole = false;
        bool AssignedToSelf = false;
        bool AssignedBySelf = false;
        bool AdvancedEdits = false;


        public TasksController() : base()
        {
            int i = 0;
            /// Right Columns indicate whether basic edits are allowed, and whether advanced edits are allowed. 

            ///      Edit Right || To self	|| By self || Basic ||	Advanced
            ///          Y	         Y	         Y	         Y	     Y
            ///          Y	         Y	         N	         Y       Y
            ///          Y           N	         N	         Y	     Y
            ///          Y	         N	         Y	         Y	     Y
            ///          N	         N	         Y	         N	     N
            ///          N	         N	         N	         N	     N
            ///          N	         Y	         N	         Y	     N
            ///          N	         Y	         Y	         Y	     Y
                                       //Edit Right || To self|| By self || Basic || Advanced
            Permissions[i++] = new bool[5]{ true,        true,     true,     true,     true };
            Permissions[i++] = new bool[5]{ true,        true,     false,    true,     true };
            Permissions[i++] = new bool[5]{ true,        false,    false,    true,     true };
            Permissions[i++] = new bool[5]{ true,        false,    true,     true,     true };
            Permissions[i++] = new bool[5]{ false,       false,    true,     false,    false};
            Permissions[i++] = new bool[5]{ false,       false,    false,    false,    false};
            Permissions[i++] = new bool[5]{ false,       true,     false,    true,     false};
            Permissions[i++] = new bool[5]{ false,       true,     true,     true,     true };            
        }

        /// <summary> gets a list of Jobs. Some are assigned to this person, some are assigned by this person. 
        /// Others are just jobs happening on the organisation where this person is working</summary>
        /// <returns></returns>
        [ResponseType(typeof(List<DTO.TaskDTO>))]
        public async Task<dynamic> Get()
        {
            long UserId = long.Parse(User.Identity.GetUserId());

            List<IEnumerable<List<TaskDb>>> relevantJobs =  await db.Organisations.Where(f => f.Bonds.Any(b => b.PersonID == UserId) && f.Deleted == false)
                .Select(b => b.Fields.Where(f => f.Deleted == false).Select(d => d.Jobs)).ToListAsync();

            TaskDb[] personalJobs = await db.Tasks.Where(j => j.assignedById == UserId || j.assignedToId == UserId).ToArrayAsync();

            List<DTO.TaskDTO> jobsDTO = new List<DTO.TaskDTO>();

            foreach (IEnumerable<List<TaskDb>> step1 in relevantJobs)
            {
                foreach (List<TaskDb> step2 in step1)
                {
                    foreach (TaskDb task in step2)
                    {
                        jobsDTO.Add((DTO.TaskDTO)task);
                    }
                }
            }
            foreach(TaskDb job in personalJobs)
            {
                if(! jobsDTO.Exists(f => f.Id == job.Id))
                    jobsDTO.Add((DTO.TaskDTO)job);
            }
                 
            return jobsDTO; 
        }

      /// <summary> Creates a job. You can assign a job to yourself, or to staff at this organisation if you role is manager or owner </summary>
      /// <param name="newJob">Job to be added</param>
      /// <returns>200 if successfull, and ErrorResponce Otherwise</returns>
        public async Task<dynamic> Post([FromBody] DTO.TaskDTO newJob)
        {
            long UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(newJob, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var theField = await db.Fields.Where(f => f.Id == newJob.onFieldId).Include(f => f.Org).Include(f => f.Org.Bonds).FirstOrDefaultAsync(); 

            if(theField == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if(! theField.Org.Bonds.Any(p => p.PersonID == newJob.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            string role = OrganisationsController.FindAndGetRole(theField.Org, UserId);
            bool editRights = BondDb.CanAssignJobsToOthers.Contains(role);
            bool requestAuthorised = false; 

            //Check user's access
            if (role == BondDb.RoleNone)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantView); }

            //Self-create
            if (newJob.assignedToId == UserId || editRights)
            { requestAuthorised = true; }

            TaskDb job = new TaskDb
            {
                name = newJob.name,
                jobDescription = newJob.jobDescription,
                rate = newJob.rate,
                state = newJob.state,
                type = newJob.type,
                DueDate = newJob.DueDate,
                EventLog = string.Empty,

                assignedById = UserId,
                assignedToId = newJob.assignedToId,
                onFieldId = theField.Id,

                timeSpent = TimeSpan.Zero,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Deleted = false
            };

            eventLog.Add(new TaskEvent((DTO.TaskDTO)job, UserId, "Job crated"));
            job.EventLog = JsonConvert.SerializeObject(eventLog);

            if (requestAuthorised)
            {
                db.Tasks.Add(job);
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }
        }

        /// <summary> Edits a job that already exists, according to the truth Table. You can wind back time, we do not check for it. </summary>
        /// <param name="id">Id of the job you want to alter</param>
        /// <param name="newTask">Job to be added</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody]  DTO.TaskDTO newTask)
        {
            bool AdvancedEdits = false;
            long UserId = long.Parse(User.Identity.GetUserId());
            bool TimeAdded = false;
            bool anythingChanged = false;

            //Check Data integrity
            HttpResponseMessage responce = Utils.CheckModel(newTask, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            TaskDb oldTask = await db.Tasks.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.Org).Include(f => f.onField.Org.Bonds).FirstOrDefaultAsync();

            if (oldTask == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (! oldTask.onField.Org.Bonds.Any(p => p.PersonID == newTask.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            //Setup variables
            string role = OrganisationsController.FindAndGetRole(oldTask.onField.Org, UserId);
            TaskManagementRole = BondDb.CanAssignJobsToOthers.Contains(role);


            TimeAdded = oldTask.timeSpent < newTask.timeSpent;


            anythingChanged = AdvancedEdits || TimeAdded || newTask.state != oldTask.state; 

            
            //Check user's access
            if (role == BondDb.RoleNone)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantView); }


            bool[] permission = Permissions.First(p => p[0] == TaskManagementRole && p[1] == AssignedToSelf && p[2] == AssignedBySelf);

            if (! CheckPermission(TaskManagementRole, UserId, oldTask, newTask))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PermissionsTooLow); }

            
            if (anythingChanged)
            {
                eventLog = JsonConvert.DeserializeObject<List<TaskEvent>>(oldTask.EventLog);

                oldTask.name = newTask.name;
                oldTask.jobDescription = newTask.jobDescription;
                oldTask.rate = newTask.rate;
                oldTask.state = newTask.state;
                oldTask.DueDate = newTask.DueDate;
                oldTask.timeSpent = newTask.timeSpent;
                oldTask.Deleted = newTask.Deleted;
                oldTask.assignedToId = newTask.assignedToId;
                oldTask.UpdatedAt = DateTime.UtcNow; 

                
                eventLog.Add(new TaskEvent((DTO.TaskDTO)oldTask, UserId));

                oldTask.EventLog = JsonConvert.SerializeObject(eventLog);
                await db.SaveChangesAsync();
            }

            return Ok(); 
        }

     

        /// <summary> Not sure yet if we should delete it </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            long UserId = long.Parse(User.Identity.GetUserId());

            var oldTask = await db.Tasks.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.Org).Include(f => f.onField.Org.Bonds).FirstOrDefaultAsync();

            if (oldTask == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            //Setup variables
            string role = OrganisationsController.FindAndGetRole(oldTask.onField.Org, UserId);
            TaskManagementRole = BondDb.CanAssignJobsToOthers.Contains(role); 

            if (! CheckPermission(TaskManagementRole, UserId, oldTask))
            {
                oldTask.Deleted = true;
                eventLog = JsonConvert.DeserializeObject<List<TaskEvent>>(oldTask.EventLog);
                var it = new TaskEvent((DTO.TaskDTO)oldTask, UserId, "Task was deleted");
                eventLog.Add(it);
                oldTask.EventLog = JsonConvert.SerializeObject(eventLog);
                await db.SaveChangesAsync();
                return Ok();
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PermissionsTooLow); ;
        }

        private bool CheckPermission(bool taskManagementRole, long UserId, TaskDb oldTask = null, TaskDTO newTask = null)
        {
            TaskDTO task1 = (TaskDTO)oldTask ?? null;
            TaskDTO task2 = newTask ?? null;
            //use local variables to avoid problems
            bool assignedToSelf = false;
            bool assignedBySelf = false;
            bool advancedEdits = false; 

            if(task1 != null && task2 == null)
            { task2 = task1; }
            else if(task2 != null && task1 == null)
            { task1 = task2; }
            else if(task1 != null && task2 != null)
            { //Good!
            }
            else
            {
                throw new Exception("In the Task Controller, both new and old Tasks are null!");
            }

            //You have to check if the new and old job are both self-assigned. Otherwise the user will have admin right over the job abd be able to assign it to someone else
            assignedToSelf = task1.assignedToId == UserId && task2.assignedToId == UserId;
            assignedBySelf = task1.assignedById == UserId && task2.assignedById == UserId;

            advancedEdits = (task1.rate - task2.rate) > 0.01 || task1.DueDate != task2.DueDate || task1.assignedToId != task2.assignedToId
            || task1.type != task2.type || task1.name != task2.name || task1.jobDescription != task2.jobDescription || task1.Deleted != task2.Deleted;

            bool[] permission = Permissions.First(p => p[0] == taskManagementRole && p[1] == assignedToSelf && p[2] == assignedBySelf);

            //move local to global variables
            AssignedToSelf = assignedToSelf;
            AssignedBySelf = assignedBySelf;
            AdvancedEdits = advancedEdits; 

            if (AdvancedEdits)
                return permission[4];
            else
                return permission[3];
        }



    }
}
