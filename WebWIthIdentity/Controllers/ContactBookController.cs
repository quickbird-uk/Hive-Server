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


namespace WebWIthIdentity.Controllers
{
    [Authorize]public class ContactBookController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> Gets a list of contacts that this user knows </summary>
        /// <returns>List of Contacts that this user has</returns>
        [ResponseType(typeof(List<RecordViewModel>))]
        public async Task<IHttpActionResult> Get()

        {
            var userId = User.Identity.GetUserId();
            var contactBook = await db.ContactBook.Where(p => p.OwnerID == userId).Include(u => u.Contact).ToListAsync();

            List<RecordViewModel> listOfContacts = ToViewModel(contactBook);
               

            return Ok(listOfContacts); 
        }

        /// <summary>
        /// Converts ContactBook list of contacts to Viewmodel we can return to the user
        /// </summary>
        /// <param name="contactBook"></param>
        /// <returns>View model for contacts</returns>
        internal static List<RecordViewModel> ToViewModel(List<CBRecord> contactBook)
        {
            List<RecordViewModel> outListOfContacts = new List<RecordViewModel>();

            foreach (var record in contactBook)
            {
                outListOfContacts.Add(new RecordViewModel
                {
                    ID = Guid.Parse(record.ContactID),
                    Nickname = record.Nickname,
                    Name = record.Contact.RealName,
                    Email = record.Contact.Email,
                    Phone = record.Contact.PhoneNumber
                });
            }

            return outListOfContacts;
        }

        /// <summary>
        /// Adds Contacts to the user's contactBook. Can accept a signle user or an entire array, which must consist of user's GUID and, optionally, nickname
        /// If the contacts already exist, it will Update nicknames
        /// </summary>
        /// <param name="models"></param>
        /// <returns>Returns an updated list of Contacts taht his user has</returns>
        [ResponseType(typeof(RecordChangeViewModel))]
        [Route("ContactBook")]
        public async Task<IHttpActionResult> Post([FromBody]List<RecordBindingModel> models)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (models.Count > 1000)
            {
                return BadRequest("That many users? Fuck off");
            }
            else if (models.Count < 1)
            {
                return BadRequest("No data");
            }
            else
            {
                var userId = User.Identity.GetUserId();
                RecordChangeViewModel responce = new RecordChangeViewModel();

                var thisUser = await db.Users.Where(p => p.Id == userId)
                                .Include(p => p.ContactBook.Select(u => u.Contact)) //Includes The contactbook, and all the users in the contactbook in a single SQL statement
                                .FirstOrDefaultAsync();

                if (thisUser == null)
                {
                    return BadRequest("Something is wrong with your token. Get a new token"); 
                }
                else
                {
                    var contactBook = thisUser?.ContactBook;

                    //iterate for each record recieved
                    foreach (var inRecord in models)
                    {
                        //Reference to a record in the user's contact book, will be null if the record does not exist yet
                        var bookRecord = contactBook?.FirstOrDefault(p => p.ContactID == inRecord.ContactID.ToString());
                        //Reference to the contact referenced by the book record, wil be bull if this record does not exist yet
                        var contact = bookRecord?.Contact;
                        bool alreadyExists = contact != null;
                        //Was a nickname supplied for this record? 
                        bool noNickname = String.IsNullOrWhiteSpace(inRecord.Nickname);

                        if (alreadyExists)
                        {
                            //if there is no nickname, switch back to using the contact's real name, otherwise use the nickname provided
                            bookRecord.Nickname = noNickname ? contact.RealName : inRecord.Nickname;
                            responce.Altered++;
                        }
                        else
                        {
                            contact = await db.Users.FirstOrDefaultAsync(p => p.Id == inRecord.ContactID.ToString());
                            bool invalidContactId = contact == null;

                            if (invalidContactId)
                            {
                                responce.Invalid++;
                            }
                            else
                            {
                                var nickName = noNickname ? contact.RealName : inRecord.Nickname;

                                contactBook?.Add(new CBRecord
                                {
                                    Nickname = nickName,
                                    ContactID = inRecord.ContactID.ToString(),
                                    Contact = contact
                                });

                                responce.Created++;
                            }
                        }
                    }
                    await db.SaveChangesAsync();
                    responce.ContactBook = ToViewModel(contactBook);

                    return Ok(responce);

                }
            }
        }


        /// <summary>
        /// Will delete a list of contacts provided. Omit a nickname here, as it does nothing
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [ResponseType(typeof(RecordChangeViewModel))]
        [Route("ContactBook")]
        public async Task<IHttpActionResult> Delete([FromBody]List<RecordBindingModel> models)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (models.Count > 1000)
            {
                return BadRequest("That many users? Fuck off");
            }
            else if (models.Count < 1)
            {
                return BadRequest("No data");
            }
            else
            {
                var userId = User.Identity.GetUserId();

                RecordChangeViewModel responce = new RecordChangeViewModel();

                var thisUser = await db.Users.Where(p => p.Id == userId)
                    .Include(p => p.ContactBook.Select(u => u.Contact))
                    //Includes The contactbook, and all the users in the contactbook in a single SQL statement
                    .FirstOrDefaultAsync();
                if (thisUser == null)
                {
                    return BadRequest("Something is wrong with your token. Get a new token");
                }
                else
                {
                    var contactBook = thisUser?.ContactBook;

                    //iterate for each record recieved
                    foreach (var inRecord in models)
                    {
                        var record =
                            thisUser.ContactBook.FirstOrDefault(p => p.ContactID == inRecord.ContactID.ToString());
                        if (record != null)
                        {
                            bool success = thisUser.ContactBook.Remove(record);
                            responce.Removed++;
                        }
                        else
                        {
                            responce.Invalid++;
                        }
                    }

                    await db.SaveChangesAsync();
                    responce.ContactBook = ToViewModel(contactBook);

                    return Ok(responce);
                }
            }
        }


        ///<summary>Searches for users based on the array yousubmit. 
        /// The array is of strings, and can have mixed phone numbers and emails, thiswill be prssesed regardless</summary>
        /// 
        [ResponseType(typeof(SearchViewModel))]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("ContactBook/SearchUsers")]
        public async Task<IHttpActionResult> SearchUsers(List<string> searchContacts)
        {
            SearchViewModel reply = new SearchViewModel();
            List<ApplicationUser> foundUsers = new List<ApplicationUser>();

            if (searchContacts == null)
                return BadRequest("No Data");

            if (searchContacts.Count == 0)
                return BadRequest("No Data");
            if (searchContacts.Count > 1000)
                return BadRequest("You shall not search more than 1000 users at once, because fuck you");


            foreach (var searchItem in searchContacts)
            {
                if (!string.IsNullOrWhiteSpace(searchItem))
                {
                    if (searchItem.Length > 5)
                    {
                        long phone;
                        if (Int64.TryParse(searchItem, out phone))
                        {
                            var user = await db.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phone);
                            if (user != null)
                            {
                                reply.Found++;
                                foundUsers.Add(user);
                            }
                            else
                            {
                                reply.NotFound++;
                            }
                        }
                        else if (searchItem.Contains("@"))
                        {
                            var user = await db.Users.FirstOrDefaultAsync(p => p.Email == searchItem);
                            if (user != null)
                            {
                                reply.Found++;
                                foundUsers.Add(user);
                            }
                            else
                            {
                                reply.NotFound++;
                            }
                        }
                        else
                        {
                            reply.Invalid++;
                        }
                    }
                    else
                    {
                        reply.Invalid++;
                    }
                }
                else
                {
                    reply.Invalid++;
                }
            }

            foreach (ApplicationUser user in foundUsers)
            {
                reply.FoundContacts.Add(new RecordViewModel
                {
                    Phone = user.PhoneNumber,
                    Name = user.RealName,
                    Nickname = user.RealName,
                    Email = user.Email,
                    ID = Guid.Parse(user.Id)
                });
            }

            return Ok(reply);
        }

    }
}
