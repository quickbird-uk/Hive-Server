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
        private long UserId;
        private List<TaskEvent> eventLog = new List<TaskEvent>();
        private readonly bool[][] Permissions = new bool[8][]; 
 

        public TasksController()
        {
            UserId = long.Parse(User.Identity.GetUserId());
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
            var relevantJobs =  await db.Organisations.Where(f => f.Bonds.Any(b => b.PersonID == UserId) && f.Deleted == false)
                .Select(b => b.Fields.Where(f => f.Deleted == false).Select(d => d.Jobs)).ToListAsync();

            var personalJobs = await db.Tasks.Where(j => j.assignedById == UserId || j.assignedToId == UserId).ToArrayAsync();

            List<DTO.TaskDTO> jobsDTO = new List<DTO.TaskDTO>(); 
            
            foreach(JobDb job in relevantJobs)
            {
                jobsDTO.Add((DTO.TaskDTO)job); 
            }
            foreach(JobDb job in personalJobs)
            {
                jobsDTO.Add((DTO.TaskDTO)job);
            }
           

            return jobsDTO; 
        }

      /// <summary> Creates a job. You can assign a job to yourself, or to staff at this organisation if you role is manager or owner </summary>
      /// <param name="newJob">Job to be added</param>
      /// <returns>200 if successfull, and ErrorResponce Otherwise</returns>
        public async Task<dynamic> Post([FromBody] DTO.TaskDTO newJob)
        {
            HttpResponseMessage responce = Utils.CheckModel(newJob, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var theField = await db.Fields.Where(f => f.Id == newJob.onFieldId).Include(f => f.Org).Include(f => f.Org.Bonds).FirstOrDefaultAsync(); 

            if(theField == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if(! theField.Org.Bonds.Any(p => p.PersonID == newJob.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            string role = OrganisationController.FindAndGetRole(theField.Org, UserId);
            bool editRights = BondDb.CanAssignJobsToOthers.Contains(role);
            bool requestAuthorised = false; 

            //Check user's access
            if (role == BondDb.RoleNone)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantView); }

            //Self-create
            if (newJob.assignedToId == UserId || editRights)
            { requestAuthorised = true; }

            JobDb job = new JobDb
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

        /// <summary> Edits a job that already exists, according to the following truth-table

        /// </summary>
        /// <param name="id">Id of the job you want to alter</param>
        /// <param name="newJob">Job to be added</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody]  DTO.TaskDTO newJob)
        {
            bool EditsMade = false;
            bool TimeAdded = false;
            bool editRights = false;
            bool assignedtoSelf = false;
            bool anythingChanged = false;

            bool validEdit = false;

            //Check Data integrity
            HttpResponseMessage responce = Utils.CheckModel(newJob, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var oldJob = await db.Tasks.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.Org).Include(f => f.onField.Org.Bonds).FirstOrDefaultAsync();

            if (oldJob == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (oldJob.onField.Org.Bonds.Any(p => p.PersonID == newJob.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            //Setup variables
            string role = OrganisationController.FindAndGetRole(oldJob.onField.Org, UserId);
            editRights = BondDb.CanAssignJobsToOthers.Contains(role);
            assignedtoSelf = oldJob.assignedToId == UserId  || (editRights && newJob.assignedToId == UserId);
            TimeAdded = oldJob.timeSpent < newJob.timeSpent;
            EditsMade = (oldJob.rate - newJob.rate) > 0.01 || oldJob.DueDate != newJob.DueDate || oldJob.assignedToId != newJob.assignedToId
                || oldJob.type != newJob.type || oldJob.name != newJob.name || oldJob.jobDescription != newJob.jobDescription || oldJob.Deleted != newJob.Deleted;
            anythingChanged = EditsMade || TimeAdded || newJob.state != oldJob.state; 

            
            //Check user's access
            if (role == BondDb.RoleNone)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantView); }

            //Accept the job
            if (! editRights && !assignedtoSelf)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            ////Reject the job
            //if ((oldJob.state == JobDb.StateAssigned || oldJob.state == JobDb.StatePending) && newJob.state == JobDb.StateAssigned && assignedtoSelf && !TimeAdded)
            //    validEdit = true;

            ////going round the loop, doing the job
            //if ((oldJob.state == JobDb.StateAssigned || oldJob.state == JobDb.StateInProgress || oldJob.state == JobDb.StatePaused || oldJob.state == JobDb.StatePending)
            //    && (newJob.state == JobDb.StateFinished || newJob.state == JobDb.StatePaused || newJob.state == JobDb.StateInProgress)
            //    && assignedtoSelf)
            //{
            //    validEdit = true;
            //}



            eventLog = JsonConvert.DeserializeObject <List<TaskEvent>>(oldJob.EventLog); 

            

            //if (oldJob.timeSpent != newJob.timeSpent)
            //{ }

            oldJob.name = newJob.name;
            oldJob.jobDescription = newJob.jobDescription;
            oldJob.rate = newJob.rate;
            oldJob.state = newJob.state;
            oldJob.DueDate = newJob.DueDate;
            oldJob.timeSpent = newJob.timeSpent; 
            oldJob.Deleted = newJob.Deleted;            
            oldJob.assignedToId = newJob.assignedToId;

            var it = new TaskEvent((DTO.TaskDTO)oldJob, UserId);
            eventLog.Add(it); 

            oldJob.EventLog = JsonConvert.SerializeObject(eventLog);
            await db.SaveChangesAsync();

            return Ok(); 
        }

        /// <summary> Not sure yet if we should delete it </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {


            var oldJob = await db.Tasks.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.Org).Include(f => f.onField.Org.Bonds).FirstOrDefaultAsync();

            if (oldJob == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }


            //Setup variables
            string role = OrganisationController.FindAndGetRole(oldJob.onField.Org, UserId);
            if (BondDb.CanAssignJobsToOthers.Contains(role))
            {
                oldJob.Deleted = true;
                return Ok();
            }
            else
                return BadRequest();
        }





    }
}
