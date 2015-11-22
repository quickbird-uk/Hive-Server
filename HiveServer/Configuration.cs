using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using HiveServer.Models;
using HiveServer.Models.FarmData;

namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<global::HiveServer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(global::HiveServer.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            var passwordHasher = new PasswordHasher();
            bool emptyDatabase = false;
            string[] crops = { "Corn", "Maise", "Potato", "Nothing", "Wheat", "Rapeseed", "Barley", "Peas", "Oats", "Buckwheat", "Wheat" }; //11 crops to seed the database

            List<ApplicationUser> users = new List<ApplicationUser>();
            users.Add(new ApplicationUser
            {
                FirstName = "Nobody",
                LastName = "Nobody",
                PhoneNumber = 0000000000,

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Winston",
                LastName = "Chirchill",
                PhoneNumber = 666666,
                Email = "win@uk",

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Some",
                LastName = "Asshole",
                PhoneNumber = 999999999,

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = 545648,
                Email = "test@test.uk"
            });

            users.Add(new ApplicationUser
            {
                FirstName = "Vladimir",
                LastName = "Akopyan",
                PhoneNumber = 7842723489,
                Email = "vlad@quickbird.uk",

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Manuel",
                LastName = "Sanabria",
                PhoneNumber = 7796604116,
                Email = "manuel@quickbird.uk",

            });

            if (!context.Organisations.Any()) //seed if there are no organisation 
            {
                emptyDatabase = true;
                List<OrganisationDb> AllOrgs = new List<OrganisationDb>();

                //Give every user a farm! 
                foreach (var user in users)
                {
                    var org = new OrganisationDb(user.FirstName + "'s Farm");
                    var bond = new BondDb
                    {
                        Role = BondDb.RoleOwner,
                        Organisation = org,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };

                    context.Organisations.Add(org);
                    user.Bound.Add(bond);

                    AllOrgs.Add(org); //put all the organisation into a list to add fields later
                }

                //Make user 2 (ash) someone an agronomist working ofr everyone
                foreach (var user in users)
                {
                    if (user.Id != users[2].Id) //can;t add another relationship to his own organisation
                    {
                        users[2].Bound.Add(new BondDb
                        {
                            Organisation = user.Bound[0].Organisation,
                            Role = BondDb.RoleSpecialist,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        });
                    }
                }

                //Add some crew to vlad's organisation
                {
                    users[0].Bound.Add(new BondDb
                    {
                        Organisation = users[4].Bound[0].Organisation,
                        Role = BondDb.RoleCrew,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
                    users[1].Bound.Add(new BondDb
                    {
                        Organisation = users[4].Bound[0].Organisation,
                        Role = BondDb.RoleCrew,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
                    users[5].Bound.Add(new BondDb
                    {
                        Organisation = users[4].Bound[0].Organisation,
                        Role = BondDb.RoleCrew,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
                    users[3].Bound.Add(new BondDb
                    {
                        Organisation = users[4].Bound[0].Organisation,
                        Role = BondDb.RoleCrew,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
                }

                Random RNG = new Random();

                //Create some fields for each organisation
                foreach (var org in AllOrgs)
                {
                    int nOfFields = RNG.Next(3, 20);
                    double fieldSize = 0;
                    while (fieldSize < 0.1)
                    {
                        fieldSize = RNG.NextDouble();
                    }
                    fieldSize = fieldSize * 10;

                    for (int i = 0; i < nOfFields; i++)
                    {

                        org.Fields.Add(new FieldDb
                        {
                            Name = crops[RNG.Next(11)],
                            AreaInHectares = Math.Round(RNG.Next(2, 6) * fieldSize, 1),
                            ParcelNumber = string.Empty,
                            FieldDescription = string.Empty
                        }
                       );
                    }

                }
                //context.SaveChanges(); 

                foreach (var user in users)
                {
                    user.UserName = Guid.NewGuid().ToString();
                    user.SecurityStamp = Guid.NewGuid().ToString("D");
                    user.PasswordHash = passwordHasher.HashPassword("Password1");
                    user.OTPSecret = Base.LoginUtils.generateSalt();
                    user.PhoneNumberConfirmed = true;
                }

                //generate a list of contacts for each user in the DB
                foreach (var user in users)
                {//determine number of contacts the user will have
                    int nOfContacts = RNG.Next(1, users.Count);

                    //array that hold indexes for contacts
                    int[] contacts = new int[nOfContacts];

                    for (int i = 0; i < contacts.Length; i++)
                    {
                        int newValue = RNG.Next(0, users.Count); //generate a random index for the contact 
                        //we cannot have two links two records point ot he same contact, so they need to be unique
                        while (contacts.Contains(newValue))
                        {
                            newValue = RNG.Next(0, users.Count);
                        }
                        contacts[i] = newValue; //when we assured that the value does not already exist, we assign it into the array

                    }
                    //Push the records and contacts in
                    foreach (var contactId in contacts)
                    {
                        context.Contacts.Add(new ContactDb
                        {
                            Person1Id = user.Id,
                            Person1 = user,
                            Person2 = users[contactId],
                            Person2Id = users[contactId].Id,
                            State = ContactDb.StateFriend,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        });
                    }

                }



                foreach (var user in users)
                {
                    context.Users.AddOrUpdate(user);
                }
            }

        }
    }
}
