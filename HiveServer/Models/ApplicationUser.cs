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
using HiveServer.Models.FarmData;

namespace HiveServer.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<long, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {

       
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

        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] OTPSecret { get; set; }

        /// <summary>
        /// The list of organisations the person is related to
        /// </summary>
        public virtual List<BondDb> Bound { get; set; }

        public virtual List<JobDb> JobsGiven { get; set; }

        public virtual List<JobDb> JobsRecieved { get; set; }

        public ApplicationUser() 
        {
            Bound = new List<BondDb>();
            JobsGiven = new List<JobDb>();
            JobsRecieved = new List<JobDb>(); 
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, long> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            
            // Add custom user claims here
            return userIdentity;
        }

    }


}