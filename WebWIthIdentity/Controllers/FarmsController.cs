using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebWIthIdentity.Models;
using WebWIthIdentity.Models.FarmData;


namespace WebWIthIdentity.Controllers
{
    [Authorize]
    public class FarmsController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        /// <summary>
        /// Get a full list of farms that belong to this user
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<FarmViewModel>))]
        public async Task<IHttpActionResult> Get()
        {
            var userId = User.Identity.GetUserId();
            var farmlist = await GetUsersFarms(Guid.Parse(userId), db); 
        
            return Ok(farmlist);
        }

        internal static async Task<List<FarmViewModel>> GetUsersFarms(Guid userID, ApplicationDbContext db)
        {
            var stingGUID = userID.ToString();

            List<FarmViewModel> filtered = new List<FarmViewModel>();
            
            var BoundFarms = await db.Bindings.Where(b => b.PersonID == stingGUID && ! b.Farm.Disabled) //Get the farms where binding include this person
                .Include(b => b.Farm.Fields)  //include fields of the farm
                .Include(f => f.Farm.Bound.Select(p => p.Person))  //include Staff wokring in each farm
                .ToListAsync();


            foreach (var bond in BoundFarms)
            {
                filtered.Add((FarmViewModel)bond);
            }


            return filtered;
        }

        /// <summary>
        /// Gets details of the spesific farm only
        /// </summary>
        /// <param name="farmId"></param>
        /// <returns></returns>
        [ResponseType(typeof(FarmViewModel))]
        [Route("Farms/{farmId}")]
        public async Task<IHttpActionResult> Get([FromUri] long farmId)
        {
            var userId = User.Identity.GetUserId();
            bool exist = await db.Bindings.AnyAsync(p => p.FarmID == farmId);

            if (!exist)
            {
                return NotFound();
            }
            else
            {

                var foundFarm = await
                    db.Bindings.Where(b => b.PersonID == userId)
                        .Where(b => b.FarmID == farmId && ! b.Farm.Disabled ) //find a bond where he owns this 
                        .Include(b => b.Farm.Fields) //include fields of the farm
                        .Include(f => f.Farm.Bound.Select(p => p.Person)) //include Staff wokring in the farm
                        .FirstOrDefaultAsync();

                if (foundFarm == null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok((FarmViewModel)foundFarm);
                }
            }

        }

        /// <summary>
        /// Creates a new farm for the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post([FromBody]FarmBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (model == null)
            {
                return BadRequest("You sent no data");
            }
            else
            {
                var UserID = User.Identity.GetUserId();
                var user = db.Users.Find(UserID);

                var newFarm = new Farm(
                   model.Name,
                   model.Description
               );
                var newBond = new Bond(newFarm, BondType.Owner);
            
                user.Bound.Add(newBond);
                await db.SaveChangesAsync();

                return Ok();
                
            }
        }

      /// <summary>
      /// Updates an existing farm. It must belong to the user. 
      /// </summary>
      /// <param name="farmId"></param>
      /// <param name="model"></param>
      /// <returns></returns>
        [Route("Farms/{farmId}")]
        public async Task<IHttpActionResult> Put([FromUri]long farmId, [FromBody]FarmBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (model == null)
            {
                return BadRequest("You sent no data");
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                List<Bond> bindings = await db.Bindings.Where(b => b.FarmID == farmId &&!  b.Farm.Disabled )
                    .ToListAsync();

                if (bindings.Count == 0)
                {
                    return NotFound(); //this farm does not exist
                }
                else
                {
                    Bond bond = bindings.FirstOrDefault(p => p.PersonID == userId);
                    if (bond == null)
                    {
                        return Unauthorized(); //user is not attached to this farm
                    }
                    else if (bond.Type != BondType.Manager && bond.Type != BondType.Owner)
                    {
                        return Unauthorized(); //user is not authorised to make changes to this farm
                    }
                    else
                    {//user is authorised, make nessesary changes! 
                        var farm = bond.Farm;
                        if (!string.IsNullOrWhiteSpace(model.Name))
                        { farm.Name = model.Name;}
                        if (!string.IsNullOrWhiteSpace(model.Description))
                        { farm.Description = model.Description;}

                        await db.SaveChangesAsync();
                        return Ok();
                    }
                }

            }
        }


        /// <summary>
        /// Removes the required farm completely 
        /// </summary>
        /// <param name="farmId"></param>
        /// <returns></returns>
        [Route("Farms/{farmId}")]
        public async Task<IHttpActionResult> Delete([FromUri]long farmId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            List<Bond> bindings = await db.Bindings.Where(b => b.FarmID == farmId && ! b.Farm.Disabled)
                .ToListAsync();

            if (bindings.Count == 0)
            {
                return NotFound(); //this farm does not exist
            }
            else
            {
                Bond bond = bindings.FirstOrDefault(p => p.PersonID == userId);
                if (bond == null)
                {
                    return Unauthorized(); //user is not attached to this farm
                }
                else if (bond.Type != BondType.Owner)
                {
                    return Unauthorized(); //user is not authorised to delete this farm

                }
                else
                {
                    //Cut this person's connection with the farm
                    if (bond.Farm.Fields.Exists(p => ! p.Disabled) && bond.Farm.Fields.Count > 0)
                    {
                        return BadRequest("You cannot delete the farm as long as there are still fields attached to it"); 
                    }
                    else
                    {
                        bond.Farm.Disabled = true; 
                        await db.SaveChangesAsync();
                        return Ok();
                    }

                }
            }
        }
        
        

    }
}
