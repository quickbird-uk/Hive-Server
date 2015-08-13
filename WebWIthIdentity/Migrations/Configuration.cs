using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebWIthIdentity.Models;

namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebWIthIdentity.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebWIthIdentity.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            var passwordHasher = new PasswordHasher();

            ApplicationUser[] users = new ApplicationUser[4];
            users[0] = new ApplicationUser
            {
                RealName = "Bob Stone",
                Twitter = "@Hairy",
                PhoneNumber = 777777777,
                Email = "Bob@stoned.uk"
            };

            users[1] = new ApplicationUser
            {
                RealName = "Winston Churchill",
                Twitter = "@Churchill",
                PhoneNumber = 666666,
                Email = "win@uk",
                HouseNumber = 10,
                Address1 = "Downing Street",
                City = "London",
                Country = "UK",
                Postcode = "SW1A 2AA"
            };

            users[2] = new ApplicationUser
            {
                RealName = "Animesh Mishra",
                Twitter = "@Thathustlerkid",
                PhoneNumber = 7796604116,
                Email = "ash@quickbird.uk",
                HouseNumber = 118,
                Address1 = "Dawlish Road",
                City = "Birmingham",
                Country = "UK",
                Postcode = "B29 7AA"
            };

            users[3] = new ApplicationUser
            {
                RealName = "Test Uset",
                Twitter = "test",
                PhoneNumber = 545648,
                Email = "test@test.uk"
            };

            foreach (var user in users)
            {
                user.UserName = user.Id;
                user.SecurityStamp = Guid.NewGuid().ToString("D");
                user.PasswordHash = passwordHasher.HashPassword("Password1"); 
                context.Users.AddOrUpdate(user);
            }
        }
    }
}
