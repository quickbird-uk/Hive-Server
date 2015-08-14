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
        /// This is a Contact book, it is a list of people that use the system that the user has saved 
        /// </summary>
        //public virtual List<Contact> ContactBook { get; set; }
        /// <summary>
        /// The list of farms owned by the person
        /// </summary>
        public virtual List<Farm> FarmsOwned { get; set; }
        /// <summary>
        /// The list of farms where the person works
        /// </summary>
        public virtual List<Farm> FarmsWorking { get; set; }

        public ApplicationUser() 
        {
            FarmsOwned = new List<Farm>();
            FarmsWorking = new List<Farm>();
           // ContactBook = new List<Contact>();
        }

        /*house Number*/
        public int HouseNumber { get; set; }

        /*three lines for address, one should be mandatory*/
        public String Address1 { get; set; } //Not Null
        public String Address2 { get; set; }
        public String Address3 { get; set; }

        public String City { get; set; } //Not Null

        public String Country { get; set; } //Not Null

        public String Postcode { get; set; } //Not Null

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            
            // Add custom user claims here
            return userIdentity;
        }
    }


}