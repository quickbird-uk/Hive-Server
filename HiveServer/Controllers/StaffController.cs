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
    [RoutePrefix("Staff")]
    public class StaffController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Gets all the staff assigned to all farms that the person works on
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<DTO.Staff>))]
        public async Task<dynamic> Get()
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var relevantFarms = db.Bindings.Where(b => b.PersonID == UserId).Select(f => f.FarmID);

            var staffBonds = await db.Bindings.Where(b => relevantFarms.Any(f => b.FarmID == f)).Include(p => p.Person).ToListAsync();

            List<Staff> staffList = new List<Staff>();
            foreach(var staffBond in staffBonds)
            {
                staffList.Add((Staff)staffBond); 
            }

            return Request.CreateResponse(HttpStatusCode.OK, staffList);
        }



        /// <summary>
        /// Adds staff to the farm
        /// </summary>
        /// <param name="newStaff">Staff to be added. Essentially you choose the farmID of the farm to add staff to, the ID of the person to add, and his role</param>
        /// <returns></returns>
        public async Task<dynamic> Post([FromBody] Staff newStaff)
        {

            var UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(newStaff, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var contacts = await db.Contacts.FirstOrDefaultAsync(b => (b.Person1Id == UserId && b.Person2Id == newStaff.personID)
                || ( b.Person1Id == newStaff.personID && b.Person2Id == UserId ));

            if(contacts == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if(contacts.State != ContactDb.StateFriend)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PersonNotAvaliable); }

            var bindingsList = await db.Bindings.Where(b => b.FarmID == newStaff.atFarmID).Include(b => b.Farm).ToArrayAsync();   

            if (bindingsList.Count() == 0) //does the spesified farm exist
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            var usersRole = bindingsList.FirstOrDefault(b => b.PersonID == UserId)?.Role; 

            if (usersRole != BondDb.RoleManager && usersRole != BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            //is the person already assigned as staff for this farm? 
            if (bindingsList.Any(b => b.PersonID == newStaff.personID))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantOverrite); }

            if (usersRole == BondDb.RoleManager && newStaff.role == BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PermissionsTooLow); }

            BondDb staffDB = new BondDb
            {
                FarmID = newStaff.atFarmID,
                PersonID = newStaff.personID,
                Role = newStaff.role
            };

            db.Bindings.Add(staffDB);
            await db.SaveChangesAsync();
            return Ok();
        }

        /// <summary> Allows the person to alter Role of staff already assigned to the farm, manager cannot alter the Role of the farm owner, only managers and owners can do this</summary>
        /// <param name="StaffId">id of the staff that should be alteres</param>
        /// <param name="changedStaff">the class containing changes to be made. Essentially you can only change the role</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody] Staff changedStaff)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(changedStaff, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var bindingsList = await db.Bindings.Where(b => b.FarmID == changedStaff.atFarmID).Include(b => b.Farm).ToArrayAsync();
            var selectedBond = bindingsList?.FirstOrDefault(b => b.Id == id);

            if (selectedBond == null) //does the spesified farm exist
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            var usersRole = bindingsList.FirstOrDefault(b => b.PersonID == UserId)?.Role;

            if (usersRole != BondDb.RoleManager && usersRole != BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            if (changedStaff.personID != selectedBond.PersonID)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            if (usersRole == BondDb.RoleManager && (changedStaff.role == BondDb.RoleOwner || selectedBond.Role == BondDb.RoleOwner))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.PermissionsTooLow); }

            if(selectedBond.PersonID == UserId)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            selectedBond.UpdatedAt = DateTime.UtcNow;
            selectedBond.Version = changedStaff.Version;
            selectedBond.Role = changedStaff.role;

            await db.SaveChangesAsync();
            return Ok();
        }



        /// <summary> Will delete staff from the farm, but you cannot delete yourself if you are the last owner </summary>
        /// <param name="id"> Id of the staff </param>
        /// <returns>Returns OK if done, and ErrorResponce if there is an error</returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var bondsList =  await db.Farms.Where(f => f.Bonds.Any(b=> b.Id == id)).Select(f => f.Bonds).FirstOrDefaultAsync();

            if(bondsList.Count == 0)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            var usersRole = bondsList.FirstOrDefault(b => b.PersonID == UserId)?.Role;
            var selectedBond = bondsList.FirstOrDefault(b => b.Id == id);

            if (usersRole != BondDb.RoleManager && usersRole != BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            //can't delete yourself if you are the last Owner
            if (selectedBond.PersonID == UserId && bondsList.Where(b => b.Role == BondDb.RoleOwner).Count() == 1)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            if (usersRole == BondDb.RoleManager && selectedBond.Role == BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            db.Bindings.Remove(selectedBond);
            await db.SaveChangesAsync();

            return Ok(); 
        }


    }
}
