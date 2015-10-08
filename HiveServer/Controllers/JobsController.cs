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
    [RoutePrefix("Jobs")]
    public class JobsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private long UserId;
        private List<JobEvent> eventLog = new List<JobEvent>();

        public JobsController()
        {
            UserId = long.Parse(User.Identity.GetUserId());
        }

        /// <summary> gets a list of Jobs. Some are assigned to this person, some are assigned by this person. 
        /// Others are just jobs happening on the farm where this person is working</summary>
        /// <returns></returns>
        [ResponseType(typeof(List<Job>))]
        public async Task<dynamic> Get()
        {
            var relevantJobs =  await db.Farms.Where(f => f.Bonds.Any(b => b.PersonID == UserId) && f.Deleted == false)
                .Select(b => b.Fields.Where(f => f.Deleted == false).Select(d => d.Jobs)).ToListAsync();

            var personalJobs = await db.Jobs.Where(j => j.assignedById == UserId || j.assignedToId == UserId).ToArrayAsync(); 

            List <Job> jobsDTO = new List<Job>(); 
            
            foreach(JobDb job in relevantJobs)
            {
                jobsDTO.Add((Job)job); 
            }
            foreach(JobDb job in personalJobs)
            {
                jobsDTO.Add((Job)job);
            }

            return jobsDTO; 
        }

      /// <summary> Creates a job. You can assign a job to yourself, or to staff on the farm if you role is manager or owner </summary>
      /// <param name="newJob">Job to be added</param>
      /// <returns>200 if successfull, and ErrorResponce Otherwise</returns>
        public async Task<dynamic> Post([FromBody] Job newJob)
        {
            HttpResponseMessage responce = Utils.CheckModel(newJob, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var theField = await db.Fields.Where(f => f.Id == newJob.onFieldId).Include(f => f.OnFarm).Include(f => f.OnFarm.Bonds).FirstOrDefaultAsync(); 

            if(theField == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if(theField.OnFarm.Bonds.Any(p => p.PersonID == newJob.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            string role = FarmsController.FindAndGetRole(theField.OnFarm, UserId);
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

            eventLog.Add(new JobEvent((Job)job, UserId, "Job crated"));
            job.EventLog = JsonConvert.SerializeObject(eventLog);

            if (requestAuthorised)
            {
                db.Jobs.Add(job);
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }
        }

        /// <summary> Edits a job that already exists </summary>
        /// <param name="id">Id of the job you want to alter</param>
        /// <param name="newJob">Job to be added</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody]  Job newJob)
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

            var oldJob = await db.Jobs.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.OnFarm).Include(f => f.onField.OnFarm.Bonds).FirstOrDefaultAsync();

            if (oldJob == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (oldJob.onField.OnFarm.Bonds.Any(p => p.PersonID == newJob.assignedToId))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            //Setup variables
            string role = FarmsController.FindAndGetRole(oldJob.onField.OnFarm, UserId);
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



            eventLog = JsonConvert.DeserializeObject <List<JobEvent>>(oldJob.EventLog); 

            

            if (oldJob.timeSpent != newJob.timeSpent)
            { }

            oldJob.name = newJob.name;
            oldJob.jobDescription = newJob.jobDescription;
            oldJob.rate = newJob.rate;
            oldJob.state = newJob.state;
            oldJob.DueDate = newJob.DueDate;
            oldJob.timeSpent = newJob.timeSpent; 
            oldJob.Deleted = newJob.Deleted;            
            oldJob.assignedToId = newJob.assignedToId;

            var it = new JobEvent((Job)oldJob, UserId);
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


            var oldJob = await db.Jobs.Where(f => f.Id == id).Include(f => f.onField)
                .Include(f => f.onField.OnFarm).Include(f => f.onField.OnFarm.Bonds).FirstOrDefaultAsync();

            if (oldJob == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }


            //Setup variables
            string role = FarmsController.FindAndGetRole(oldJob.onField.OnFarm, UserId);
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
