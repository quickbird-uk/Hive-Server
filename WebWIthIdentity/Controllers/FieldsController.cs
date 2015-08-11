using System;
using System.Collections.Generic;
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
    public class FieldsController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET Fields
        public async Task<IHttpActionResult> Get()
        {
            
            var userID = User.Identity.GetUserId();

            var foundfields =  db.Fields.Where(f => f.ApplicationUserId.Equals(userID)).ToList();

            List<FieldViewModel> filtered = new List<FieldViewModel>();

            foreach (var field in foundfields)
            {
                if (!field.disabled)
                {
                    filtered.Add(FieldToViewModel(field));
                }
            }
            
            return Ok(filtered);
        }

        [Route("Fields/{fieldId}")]
        public IHttpActionResult Get([FromUri] long fieldId)
        {
            var userId = User.Identity.GetUserId();

            var requestedField = db.Fields.Find(fieldId);

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

        
        public async Task<IHttpActionResult> Post([FromBody]FieldBindingModel model)
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
                var newfield = new Field(
                    model.name,
                    model.description,
                    model.longitude ,
                    model.lattitude
                );

                var UserID = User.Identity.GetUserId(); 
                var userDb = db.Users.Find(UserID);
                if (userDb == null)
                {
                    return Unauthorized();
                }
                else
                {
                    userDb.Fields.Add(newfield);

                    await db.SaveChangesAsync();

                    return Ok(FieldToViewModel(newfield));
                }
            }
        }

        [Route("Fields/{fieldId}")]
        public async Task<IHttpActionResult> Put([FromUri]long fieldId, [FromBody]FieldBindingModel model)
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

                var editedField = db.Fields.Find(fieldId);

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

        [Route("Fields/{fieldId}")]
        public async Task<IHttpActionResult> Delete([FromUri]long fieldId)
        {
            var userId = User.Identity.GetUserId();

            var editedField = db.Fields.Find(fieldId);

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



        internal static FieldViewModel FieldToViewModel(Field field)
        {
            return (new FieldViewModel()
            {
                id = field.Id,
                name = field.name,
                description = field.description,
                lattitude = field.lattitude,
                longitude = field.longitude,
                created = field.created,
                lastUpdated = field.lastUpdated
            });

        }
    }
}
