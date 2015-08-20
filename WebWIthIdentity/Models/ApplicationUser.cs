using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebWIthIdentity.Models.FarmData;

namespace WebWIthIdentity.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

 
        [Index(IsUnique = false)]
        new public long PhoneNumber
        {
            get
            {
                long phoneNumber; 
                Int64.TryParse(base.PhoneNumber, out phoneNumber);
                return phoneNumber;
            }
            set { base.PhoneNumber = value.ToString(); }
        } //overrides the default 

        
        public string RealName { get; set; }

        public string Twitter { get; set; }

        /// <summary>
        /// This is a CBRecord book, it is a list of people that use the system that the user has saved 
        /// </summary>
        public virtual List<CBRecord> ContactBook { get; set; }
        /// <summary>
        /// The list of farms the person is related to
        /// </summary>
        public virtual List<Bond> Bound { get; set; }

        public List<Farm> FarmsOwned(){
            return SelectFarms(BondType.Owner);
        }

        public List<Farm> FarmsManaged() {
                return SelectFarms(BondType.Manager);
        }

        public List<Farm> FarmsConsulted(){
                return SelectFarms(BondType.Agrinomist);
        }

        public List<Farm> FarmsWorked(){
                return SelectFarms(BondType.Crew);
        }


        public ApplicationUser() 
        {
            Bound = new List<Bond>();
            ContactBook = new List<CBRecord>();
        }

        /*house Number*/
        public int HouseNumber { get; set; }

        /*three lines for address, one should be mandatory*/
        public string Address1 { get; set; } //Not Null
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        public string City { get; set; } //Not Null

        public string Country { get; set; } //Not Null

        public string Postcode { get; set; } //Not Null

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            
            // Add custom user claims here
            return userIdentity;
        }

        private List<Farm> SelectFarms (BondType selectBondType)
        {
            List<Farm> selectedFarms = new List<Farm>();
            foreach (var bond in Bound)
            {
                if(bond.Type == selectBondType)
                { selectedFarms.Add(bond.Farm);}
            }
            return selectedFarms;
        }

        public RecordViewModel ViewModel()
        {
            return (RecordViewModel) this; 

        }
    }


}