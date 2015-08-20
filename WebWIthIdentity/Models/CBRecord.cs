using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebWIthIdentity.Models.FarmData;

namespace WebWIthIdentity.Models
{
    /// <summary>
    /// Records form a contact book. CBRecord book is a list of people each user knows on the system. 
    /// </summary>
    public class CBRecord
    {

        public string OwnerID { get; set; }

        public virtual ApplicationUser Owner { get; set; }


        public string ContactID { get; set; }

        public virtual ApplicationUser Contact { get; set; }

        public string Nickname { get; set; }

        /// <summary>
        ///  
        /// To use this fucntion, The contact and it's detail must be loaded into memory from SQL database
        /// </summary>
        /// <returns>Returns Viewmodel of the contact referred in this CBRecord</returns>
        public RecordViewModel ViewModel()
        {
            return (RecordViewModel)(this); 
        }
    }

    /// <summary> This view model is used to display information of other users. You never can see more detail about other users
    /// </summary>
    public class RecordViewModel
    {
        /// <summary> ID of the user, this can be used to add him to your contact book or to assign him to a farm</summary>
        public Guid ID { get; set; }

        /// <summary>  The name that user entered in his account settings </summary>
        public string Name { get; set; }

        /// <summary> Each user can assign a nickname to his contacts. It is visible only to him. 
        /// By default the nickname is the same as the name of each user. If a user renames himself, the nickname does not change </summary>
        public string Nickname { get; set; }

        public BondType Role { get; set; }

        [DataType(DataType.PhoneNumber)]
        public long Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public static explicit operator RecordViewModel(ApplicationUser v)
        {
            return new RecordViewModel
            {
                ID = Guid.Parse(v.Id),
                Name = v.RealName,
                Nickname = string.Empty,
                Phone = v.PhoneNumber,
                Email = v.Email,
                Role = BondType.NotApplicable
            };
        }

        public static explicit operator RecordViewModel(CBRecord v)
        {
            var viewmodel = (RecordViewModel) v.Contact;
            v.Nickname = v.Nickname;
            return viewmodel; 
        }

        public static explicit operator RecordViewModel(Bond v)
        {
            var viewmodel = (RecordViewModel) v.Person;
            viewmodel.Role = v.Type;
            return viewmodel; 
        }
    }

    /// <summary> This view model is used to display information of other users. You never can see more detail about other users
    /// </summary>
    public class RecordBindingModel 
    {

        /// <summary> ID of the contact you wish to add, NOT this user</summary>
        [Required]
        public Guid ContactID { get; set; }


        /// <summary> Each user can assign a nickname to his contacts. It is visible only to him. 
        /// By default the nickname is the same as the name of each user. If a user renames himself, the nickname does not change </summary>
        public string Nickname { get; set; }

    }

    /// <summary>
    /// View model for adding contacts, provides feedback on number of contacts that already exist and number of invalid contacts,
    ///  as well as a refreshed list of contacts. 
    /// </summary>
    public class RecordChangeViewModel
    {
        /// <summary> If some of the contacts you ried to add are already in your contactbook, this number will not equal to zero </summary>
        public int Altered { get; set; }

        /// <summary> This indicates that some ID's you supplied where invalid, i.e. a user with this ID does not exist on the system</summary>
        public int Invalid { get; set; }

        /// <summary>Number of contacts taht where created in your contact book/summary>
        public int Created{ get; set; }

        /// <summary>Number of contacts that where deleted from your contact book/summary>
        public int Removed { get; set; }


        /// <summary> new list of contacts  </summary>
        public  List<RecordViewModel>  ContactBook { get; set; }

        public RecordChangeViewModel()
        {
            ContactBook = new List<RecordViewModel>();
        }
    }


    /// <summary>
    /// A summary of contacts who'se details where found on the system
    /// </summary>
    public class SearchViewModel
    {
        /// <summary> Number of credential that where in incorrect format, so not an email or phone number </summary>
        public int Invalid { get; set; }

        /// <summary> Number of credentials that where not foundo n the system, i.e. the user is not registered with Quickbird </summary>
        public int NotFound { get; set; }

        /// <summary> Number of users found on the system</summary>
        public int Found { get; set; }

        public SearchViewModel()
        {
            FoundContacts = new List<RecordViewModel>();
        }

        /// <summary> List of people that we found where registered on the quickbird system</summary>
        public List<RecordViewModel> FoundContacts { get; set; }
    }




}