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
            
            var BoundFarms = await db.Bindings.Where(b => b.PersonID == stingGUID) //Get the farms where binding include this person
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
                        .Where(b => b.FarmID == farmId) //find a bond where he owns this 
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

      
        /*
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

                var editedField = db.Farms.Find(farmId);

                if (editedField == null)
                {
                    return NotFound();
                }
                else if (editedField.ApplicationUserId.Equals(userId))
                {
                    editedField.name = model.name;
                    if (model.description?.Length > 0)
                        editedField.description = model.description;
                    if (model.longitude != 0 && model.lattitude != 0)
                    {
                        editedField.lattitude = model.lattitude;
                        editedField.longitude = model.longitude; 
                    }
                    
                    await db.SaveChangesAsync();

                    return Ok(FieldToViewModel(editedField));
                    
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

        [Route("Farms/{farmId}")]
        public async Task<IHttpActionResult> Delete([FromUri]long farmId)
        {
            var userId = User.Identity.GetUserId();

            var editedField = db.Farms.Find(farmId);

            if (editedField == null)
            {
                return NotFound();
            }
            else if (editedField.ApplicationUserId.Equals(userId))
            {
                editedField.disabled = true; 
                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        */

    }
}
