using HiveServer.Base;
using HiveServer.DTO;
using HiveServer.Models;
using HiveServer.Models.FarmData;
using Microsoft.AspNet.Identity;
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

        /// <summary> gets a list of Jobs. Some are assigned to this person, some are assigned by this person. 
        /// Others are jsut jobs happening on the farm </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<Job>))]
        public async Task<dynamic> Get()
        {


            return Ok();
        }



      /// <summary> Creates a job. You can assign a job to yourself, or to staff on the farm if you role is manager or owner </summary>
      /// <param name="newJob">Job to be added</param>
      /// <returns>200 if successfull, and ErrorResponce Otherwise</returns>
        public async Task<dynamic> Post([FromBody] Job newJob)
        {

            return Ok();
        }

        /// <summary> Edits a job that already exists </summary>
        /// <param name="id">Id of the job you want to alter</param>
        /// <param name="newJob">Job to be added</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody]  Job newJob)
        {
            return Ok(); 
        }



        /// <summary> Not sure yet if we should delete it </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            return Ok();
        }




    }
}
