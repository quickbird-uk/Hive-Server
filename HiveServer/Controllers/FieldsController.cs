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
    [RoutePrefix("Fields")]
    public class FieldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> gets a list of fields. Includes the ones that were soft-deleted</summary>
        /// <returns></returns>
        [ResponseType(typeof(List<Field>))]
        public async Task<dynamic> Get()
        {
            var userId = long.Parse(User.Identity.GetUserId());
            List<FieldDb>[] orgFieldList = await db.Bindings.Where(b => b.PersonID == userId)
                .Select(f => f.Organisation).Select(f => f.Fields).ToArrayAsync();

            var fieldsDto = new List<Field>();

            foreach(var fieldList in orgFieldList)
            {
                foreach (var field in fieldList)
                {
                    fieldsDto.Add((Field)field);
                }
            }

            return fieldsDto;
        }



        /// <summary> Creates a new field on a spesified organisation if the user has the right permissions </summary>
        /// <param name="newField">returns 200 if successfull, or ErrorResponce</param>
        public async Task<dynamic> Post([FromBody] Field newField)
        {
            var userId = long.Parse(User.Identity.GetUserId());
            if(newField != null)
                newField.oldObject = false; 

            HttpResponseMessage responce = Utils.CheckModel(newField, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var bond = await OrganisationsController.GetThisBondAnOrg(userId, newField.onOrganisationID, db); 

            if(bond == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist);}

            if(bond.Role != BondDb.RoleOwner)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            FieldDb newFieldDB = new FieldDb
            {
                onOrganisationID = newField.onOrganisationID,
                OnOrganisation = bond.Organisation,
                Name = newField.name,
                AreaInHectares = newField.areaInHectares,
                FieldDescription = newField.fieldDescription,
                Lattitude = newField.latitude,
                Longitude = newField.longitude,
                ParcelNumber = newField.parcelNumber
            };

            db.Fields.Add(newFieldDB);
            await db.SaveChangesAsync();
            return Ok((Field) newFieldDB);
        }

        /// <summary>
        /// You can alter the field's name and description, if you are the owner of the organisation.  You can also Undelete a field, by setting Deleted to false
        /// </summary>
        /// <param name="id">Id of the field</param>
        /// <param name="newField">The field object</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody] Field newField)
        {
            var userId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(newField, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var field = await db.Fields.Where(f => f.Id == id).Include(f => f.OnOrganisation).Include(f => f.OnOrganisation.Bonds).FirstOrDefaultAsync();
            var organisation = field?.OnOrganisation;
            var staff = organisation?.Bonds;

            if(field == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            string role = OrganisationsController.FindAndGetRole(organisation, userId); 

            if(role != BondDb.RoleManager)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            if (field.Id != newField.id)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            //for now we do not allow people ot move fields. That could change later. 
            if (field.onOrganisationID != newField.onOrganisationID)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            field.Name = newField.name;
            field.FieldDescription = newField.fieldDescription;
            field.Version = newField.version;
            field.MarkedDeleted = newField.markedDeleted;
            field.ParcelNumber = newField.parcelNumber;
            field.Longitude = newField.longitude;
            field.Lattitude = newField.latitude;
            field.AreaInHectares = newField.areaInHectares;
            field.UpdatedOn = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Ok((Field)field); 
        }



        /// <summary> Will soft-delete the spesified field, but only if you are the owner </summary>
        /// <param name="id">ID of the Field</param>
        /// <returns>Returns OK if done, and ErrorResponce if there is an error</returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            var userId = long.Parse(User.Identity.GetUserId());

            var field = await db.Fields.Where(f => f.Id == id).Include(f => f.OnOrganisation).Include(f => f.OnOrganisation.Bonds).FirstOrDefaultAsync();
            var organisation = field?.OnOrganisation;
            var staff = organisation?.Bonds;

            if (field == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            string role = OrganisationsController.FindAndGetRole(organisation, userId);

            if (role != BondDb.RoleManager)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantEdit); }

            field.MarkedDeleted = true;

            await db.SaveChangesAsync();

            return Ok();
        }




    }
}
