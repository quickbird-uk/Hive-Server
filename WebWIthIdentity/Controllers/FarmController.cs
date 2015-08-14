using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebWIthIdentity.Models;


namespace WebWIthIdentity.Controllers
{
    [Authorize]
    public class FarmController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET Farms
        public async Task<IHttpActionResult> Get()
        {
            
            var userID = User.Identity.GetUserId();

            
            var foundfarmsManaged =  await db.Farms.Where(f => f.Managers.Contains(
                db.Users.Find(userID))).ToListAsync();

            var foundfarmsWorking = await db.Farms.Where(f => f.Crew.Contains(
               db.Users.Find(userID))).ToListAsync();

            List<FarmViewModel> filtered = new List<FarmViewModel>();

            foreach (var farm in foundfarmsManaged)
            {
                if (!farm.disabled)
                {
                    filtered.Add(FieldToViewModel(farm, true));
                }
            }

            foreach (var farm in foundfarmsWorking)
            {
                if (!farm.disabled)
                {
                    filtered.Add(FieldToViewModel(farm, false));
                }
            }

            return Ok(filtered);
        }

        /*
        [Route("Farms/{farmId}")]
        public IHttpActionResult Get([FromUri] long fieldId)
        {
            var userId = User.Identity.GetUserId();

            var requestedField = db.Farms.Find(fieldId);

            if (requestedField == null)
            {
                return NotFound();
            }
            else if (requestedField.ApplicationUserId.Equals(userId))
            {
                
                return Ok(FieldToViewModel(requestedField));
            }
            else
            {
                return Unauthorized();
            }

        }

        
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
                var newFarm = new Farm(
                    model.name,
                    model.description,
                    model.longitude ,
                    model.lattitude
                );

                var UserID = User.Identity.GetUserId(); 
                var userDb =  db.Users.Find(UserID);
                var userManaged = await UserManager.FindByIdAsync(UserID); 

                if (userDb == null)
                {
                    return Unauthorized();
                }
                else
                {
                    
                    
                    userDb.FarmsOwned.Add(newFarm);
                    userManaged.FarmsOwned.Add(newFarm);

                    await db.SaveChangesAsync();
                    await UserManager.UpdateAsync(userManaged);

                    return Ok(FieldToViewModel(newFarm));
                }
            }
        }

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


        internal static FarmViewModel FieldToViewModel(Farm farm, bool owner)
        {
            return (new FarmViewModel()
            {
                id = farm.Id,
                name = farm.name,
                description = farm.description,
                lattitude = farm.lattitude,
                longitude = farm.longitude,
                created = farm.created,
                lastUpdated = farm.lastUpdated,
                Owner = owner
            });

        }
    }
}
