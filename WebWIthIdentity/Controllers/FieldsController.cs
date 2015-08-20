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
    [Route("Farms/{farmId}/Fields")]
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

        

        /// <summary> Creates a field, attached to the specified farm. Works only if the user is a Manager or Owner of the farm  </summary>
        /// <param name="model"></param>
        /// <param name="farmId"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post([FromBody]FieldBindingModel model, [FromUri]long farmId)
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
                var userId= User.Identity.GetUserId();
                var bond = await db.Bindings.Where(p => p.FarmID == farmId && p.PersonID == userId)
                    .Include(f => f.Farm)
                    .FirstOrDefaultAsync();


                if (bond?.Type == BondType.Owner || bond?.Type == BondType.Manager)
                {
                    string desctription = model.Description ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(model.Name))
                    {
                        return BadRequest("You must provide a name");
                    }
                    else
                    {
                        var newField = new Field(model.Name, model.Description);
                        bond.Farm.Fields.Add(newField);
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
        }

       

    }
}
