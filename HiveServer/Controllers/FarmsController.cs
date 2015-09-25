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
    [RoutePrefix("Farms")]
    public class FarmsController : ApiController
    {



        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> Returns a list of fgarms, the the role this user has on those farms </summary>
        /// <returns>The list of farms that the user is attached to/returns>
        [ResponseType(typeof(List<DTO.Farm>))]
        public async Task<dynamic> Get()
        {
            var userId = long.Parse(User.Identity.GetUserId());
            
            List<BondDb> relevantFarms = await QueryAllRelevantBonds(userId, db).ToListAsync();
            List<Farm> farmsDTO = new List<Farm>();

            foreach (var farm in relevantFarms)
            {
                farmsDTO.Add((Farm)farm); 
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, farmsDTO);
        }



        /// <summary>
        /// Creates a new farm. The ID you assign to hte farm will be ignored. ONly name and description is respected 
        /// </summary>
        /// <param name="newFarm">returns 200 if successfull, or ErrorResponce</param>
        public async Task<dynamic> Post([FromBody] Farm newFarm)
        {
            HttpResponseMessage responce = Utils.CheckModel(newFarm, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var UserId = long.Parse(User.Identity.GetUserId());

            FarmDb farmDb = new FarmDb
            {
                Name = newFarm.name,
                Description = newFarm.farmDescription
            };

            Models.FarmData.BondDb bond = new Models.FarmData.BondDb
            {
                Farm = farmDb,
                PersonID = UserId,
                Role = Models.FarmData.BondDb.RoleOwner
            };

            db.Farms.Add(farmDb);
            db.Bindings.Add(bond);

            await db.SaveChangesAsync();
            return Ok(); 
        }

        /// <summary>
        /// You can alter the farm's name and description, if you are the owner.  You can also Undelete a farm, by setting Deleted to false
        /// </summary>
        /// <param name="id">Id of the farm</param>
        /// <param name="newFarm"> The farm object</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody] Farm newFarm)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(newFarm, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var farmBinding = await GetThisBondAndFarm(UserId, id, db);

            if (farmBinding == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (farmBinding.Role != Models.FarmData.BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            var farmDb = farmBinding.Farm; 
            farmDb.Name = newFarm.name;
            farmDb.Description = newFarm.farmDescription;
            farmDb.Version = newFarm.Version;
            farmDb.UpdatedAt = DateTime.UtcNow;
            farmDb.Deleted = newFarm.Deleted;
            

            await db.SaveChangesAsync();
            return Ok();
        }



        /// <summary>
        /// Will soft-delete the spesified farm, but only if you are the owner
        /// </summary>
        /// <param name="id">ID of the Farm</param>
        /// <returns>Returns OK if done, and ErrorResponce if there is an error</returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var farmBinding = await GetThisBondAndFarm(UserId, id, db);

            if (farmBinding == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (farmBinding.Role != Models.FarmData.BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            farmBinding.Farm.Deleted = true;  
            await db.SaveChangesAsync();
            return Ok();
        }

        internal static string FindAndGetRole(FarmDb farm, long userId)
        {
            var bindings = farm.Bonds;
            string access = BondDb.RoleNone;

            foreach (var bond in bindings)
            {
                if (bond.PersonID == userId)
                {
                    access = bond.Role;
                }
            }

            return access;
        }

        internal static async Task<BondDb> GetThisBondAndFarmAndBonds(long userId, long farmId, ApplicationDbContext db)
        {
            var farmBinding = await db.Bindings.Where(b => b.PersonID == userId && b.FarmID == farmId).Include(f => f.Farm).Include(f => f.Farm.Bonds).FirstOrDefaultAsync();
            return farmBinding; 
        }

        internal static async Task<BondDb> GetThisBondAndFarm(long userId, long farmId, ApplicationDbContext db)
        {
            var farmBinding = await db.Bindings.Where(b => b.PersonID == userId && b.FarmID == farmId).Include(f => f.Farm).FirstOrDefaultAsync();
            return farmBinding;
        }

        internal static IQueryable<FarmDb> QueryAllRelevantFarms(long userId, ApplicationDbContext db)
        {
            var farmBinding =  db.Bindings.Where(b => b.PersonID == userId).Select(f => f.Farm);
            return farmBinding;
        }

        internal static IQueryable<BondDb> QueryAllRelevantBonds(long userId, ApplicationDbContext db)
        {
            var Bonds = db.Bindings.Where(b => b.PersonID == userId);
            return Bonds;
        }

        //internal static Func<ApplicationDbContext, IQueryable<BondDb>> QueryBonds()
        //{
        //    int userId = 5; 

        //    Func<ApplicationDbContext, IQueryable<BondDb>> QueryRelevantBonds = d =>
        //    {
        //        var Bonds = d.Bindings.Where(b => b.PersonID == userId);
        //        return Bonds;
        //    };
        //}

        


}
}
