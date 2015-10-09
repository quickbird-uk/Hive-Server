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
    [RoutePrefix("Organisations")]
    public class OrganisationController : ApiController
    {



        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> Returns a list of fgarms, the the role this user has on those organisation </summary>
        /// <returns>The list of organisations that the user is attached to/returns>
        [ResponseType(typeof(List<DTO.Organisation>))]
        public async Task<dynamic> Get()
        {
            var userId = long.Parse(User.Identity.GetUserId());
            
            List<BondDb> relevantOrganisations = await QueryAllRelevantBonds(userId, db).ToListAsync();
            List<Organisation> orgDTO = new List<Organisation>();

            foreach (var organisation in relevantOrganisations)
            {
                orgDTO.Add((Organisation)organisation); 
            }

            return Request.CreateResponse(HttpStatusCode.OK, orgDTO);
        }



        /// <summary>
        /// Creates a new organisation. The ID you assign to the organisation will be ignored. ONly name and description is respected 
        /// </summary>
        /// <param name="neworganisation">returns 200 if successfull, or ErrorResponce</param>
        public async Task<dynamic> Post([FromBody] Organisation neworganisation)
        {
            HttpResponseMessage responce = Utils.CheckModel(neworganisation, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var UserId = long.Parse(User.Identity.GetUserId());

            OrganisationDb orgDb = new OrganisationDb
            {
                Name = neworganisation.name,
                Description = neworganisation.orgDescription
            };

            Models.FarmData.BondDb bond = new Models.FarmData.BondDb
            {
                Organisation = orgDb,
                PersonID = UserId,
                Role = Models.FarmData.BondDb.RoleOwner
            };

            db.Organisations.Add(orgDb);
            db.Bindings.Add(bond);

            await db.SaveChangesAsync();
            return Ok(); 
        }

        /// <summary>
        /// You can alter the organisation's name and description, if you are the owner.  You can also Undelete a organisation, by setting Deleted to false
        /// </summary>
        /// <param name="id">Id of the organisation</param>
        /// <param name="newOrganisation"> The organisation object</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody] Organisation newOrganisation)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(newOrganisation, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var orgBonds = await GetThisBondAnOrg(UserId, id, db);

            if (orgBonds == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (orgBonds.Role != Models.FarmData.BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            var orgDb = orgBonds.Organisation; 
            orgDb.Name = newOrganisation.name;
            orgDb.Description = newOrganisation.orgDescription;
            orgDb.Version = newOrganisation.Version;
            orgDb.UpdatedAt = DateTime.UtcNow;
            orgDb.Deleted = newOrganisation.Deleted;
            

            await db.SaveChangesAsync();
            return Ok();
        }



        /// <summary>
        /// Will soft-delete the spesified organisation, but only if you are the owner
        /// </summary>
        /// <param name="id">ID of the Organisation</param>
        /// <returns>Returns OK if done, and ErrorResponce if there is an error</returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var OrgBonds = await GetThisBondAnOrg(UserId, id, db);

            if (OrgBonds == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            if (OrgBonds.Role != Models.FarmData.BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            OrgBonds.Organisation.Deleted = true;  
            await db.SaveChangesAsync();
            return Ok();
        }

        internal static string FindAndGetRole(OrganisationDb organisation, long userId)
        {
            var bindings = organisation.Bonds;
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

        internal static async Task<BondDb> GetThisBondAndOrgAndBonds(long userId, long orgId, ApplicationDbContext db)
        {
            var orgBonds = await db.Bindings.Where(b => b.PersonID == userId && b.OrganisationID == orgId).Include(f => f.Organisation).Include(f => f.Organisation.Bonds).FirstOrDefaultAsync();
            return orgBonds; 
        }

        internal static async Task<BondDb> GetThisBondAnOrg(long userId, long orgId, ApplicationDbContext db)
        {
            var orgBonds = await db.Bindings.Where(b => b.PersonID == userId && b.OrganisationID == orgId).Include(f => f.Organisation).FirstOrDefaultAsync();
            return orgBonds;
        }

        internal static IQueryable<OrganisationDb> QueryAllRelevantOrgs(long userId, ApplicationDbContext db)
        {
            var orgBonds =  db.Bindings.Where(b => b.PersonID == userId).Select(f => f.Organisation);
            return orgBonds;
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
