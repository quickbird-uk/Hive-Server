using HiveServer.Base;
using HiveServer.DTO;
using HiveServer.Models;
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
    [RoutePrefix("Contacts")]
    public class ContactsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> Returns a list of contacts that belong to this user </summary>
        /// <returns>List of contacts that hte user has</returns>
        [ResponseType(typeof(List<DTO.Contact>))]
        public async Task<dynamic> Get()
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var Contacts =
                await db.Contacts.Where(p => p.Person1Id == UserId || p.Person2Id == UserId).ToListAsync();

            List <DTO.Contact> result = new List<DTO.Contact>();

            foreach(ContactDb contact in Contacts)
            {
                result.Add(contact.ToContact(UserId)); 
            }

            return Request.CreateResponse(HttpStatusCode.OK, result); 
        }



        /// <summary>
        /// By submitting ID of the user in the body of the request, you can attempt to "Friend" them
        /// </summary>
        /// <param name="contactID">returns 200 if successfull, or will reply if unsuccessfull</param>
        public async Task<dynamic> Post([FromBody]long contactID)
        {


            var UserId = long.Parse(User.Identity.GetUserId());

            bool exists = await db.Contacts.AnyAsync(
                        p => (p.Person1Id == UserId && p.Person2Id == contactID )
                    || (p.Person1Id == contactID && p.Person2Id == UserId) );

            if(exists)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CantOverrite);}

            if(! await db.Users.AnyAsync(u => u.Id == contactID))
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

           ContactDb newContact = new ContactDb
            {
                State = ContactDb.StatePendingP2,
                Person1Id = UserId,
                Person2Id =contactID
            };

            db.Contacts.Add(newContact);
            await db.SaveChangesAsync(); 

            return Ok(newContact.ToContact(UserId)); 
        }

        /// <summary>
        /// Will alter the person's Contacts as requested. At the moment the only thing you can alter is State, all other changed will be disregarded. 
        /// That means you cannot rename the other person, or do anything else silly
        /// </summary>
        /// <param name="id">Id of the connection, NOT of the user</param>
        /// <param name="contactWeb"> The contact POCO</param>
        /// <returns></returns>
        public async Task<dynamic> Put([FromUri] long id, [FromBody]Contact contactWeb)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            HttpResponseMessage responce = Utils.CheckModel(contactWeb, Request);
            if (!responce.IsSuccessStatusCode)
                return responce;

            var contactDb = await db.Contacts.FirstOrDefaultAsync(
                        p => (p.Person1Id == UserId && p.Id == id)
                    || (p.Id == id && p.Person2Id == UserId));

            if (contactDb == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist); }

            var contact = contactDb.ToContact(UserId);

            //when you can become friends
            if (contact.state == ContactDb.StatePendingP1 && contactWeb.state == ContactDb.StateFriend)
            { contactDb.State = ContactDb.StateFriend; }

            //when you can block the person
            else if (contact.state == ContactDb.StateFriend && contactWeb.state == ContactDb.StateBlockedP2
                || contact.state == ContactDb.StatePendingP1 && contactWeb.state == ContactDb.StateBlockedP2)
            {
                if (contactDb.Person1Id == UserId)
                { contactDb.State = ContactDb.StateBlockedP2; }
                else
                { contactDb.State = ContactDb.StateBlockedP1; }
            }

            else 
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            await db.SaveChangesAsync();
            return Ok();
        }

 

        /// <summary>
        /// Will delete a ralationship between you and another user, as long as you are not the one that was blocked
        /// </summary>
        /// <param name="id">ID of the RELATIONSHIP not the user</param>
        /// <returns>Returns OK if done, and ErrorResponce if there is an error</returns>
        public async Task<dynamic> Delete([FromUri] long id)
        {
            var UserId = long.Parse(User.Identity.GetUserId());

            var contactDb = await db.Contacts.FirstOrDefaultAsync(
                        p => (p.Person1Id == UserId && p.Id == id)
                    || (p.Id == id && p.Person2Id == UserId));

            if(contactDb == null)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.DoesntExist);}

            var contact = contactDb.ToContact(UserId); 

            if(contact.state == ContactDb.StateBlockedP1)
            { return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.IllegalChanges); }

            db.Contacts.Remove(contactDb);
            await db.SaveChangesAsync();
            return Ok(); 
        }

        /// <summary>
        /// When given a list of phone numbers, it will produce a responce listing people found in the database
        /// </summary>
        /// <param name="search">List of people to search in teh DB</param>
        /// <returns>Return a list of people</returns>
        [ResponseType(typeof(List<_Person>))]
        [Route("Search")]
        public async Task<dynamic> Search([FromBody]List<long> search)
        {
            if (search.Count > 500)
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.AbuseWarning);

            var foundUsers =
                await db.Users.Where(p => search.Any(s => s == p.PhoneNumber)).ToArrayAsync();

            List<_Person> peopleDTO = new List<_Person>();

            foreach(var user in foundUsers)
            {
                peopleDTO.Add((_Person)user);
            }

            return Request.CreateResponse(HttpStatusCode.OK, peopleDTO);
        } 
    }
}
