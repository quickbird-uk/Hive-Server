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
                FirstName = "Bob Stone",

                PhoneNumber = 777777777,
   
            });

            users.Add(new ApplicationUser
            {
                FirstName = "Winston Churchill",

                PhoneNumber = 666666,
                Email = "win@uk",

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Animesh Mishra",

                PhoneNumber = 7796604116,

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Test User",

                PhoneNumber = 545648,
                Email = "test@test.uk"
            });

            users.Add(new ApplicationUser
            {
                FirstName = "Vladimir Akopyan",

                PhoneNumber = 7842723489,
                Email = "vlad@quickbird.uk",

            });

            users.Add(new ApplicationUser
            {
                FirstName = "Manuel",
                PhoneNumber = +44745272505,
                Email = "manuel@quickbird.uk",

            });

            if (!context.Farms.Any()) //seed if there are no farms 
            {
                emptyDatabase = true;
                List<FarmDb> AllFarms = new List<FarmDb>();

                //Give every user a farm! 
                foreach (var user in users)
                {
                    var farm = new FarmDb(user.FirstName + "'s Farm");
                    var bond = new BondDb
                    {
                        Role = BondDb.RoleOwner,
                        Farm = farm
                    };

                    context.Farms.Add(farm);
                    user.Bound.Add(bond);

                    AllFarms.Add(farm); //put all the farms into a list to add fields later
                }

                //Make user 2 (ash) someone an agronomist working ofr everyone
                foreach (var user in users)
                {
                    if(user.Id != users[2].Id) //can;t add another relationship to his own farm
                    {
                        users[2].Bound.Add(new BondDb
                        {
                            Farm = user.Bound[0].Farm,
                            Role = BondDb.RoleAgronomist
                        });
                    }
                }

                //Add some crew to vlad's farm
                {
                    users[0].Bound.Add(new BondDb
                    {
                        Farm = users[4].Bound[0].Farm,
                        Role = BondDb.RoleCrew
                    });
                    users[1].Bound.Add(new BondDb
                    {
                        Farm = users[4].Bound[0].Farm,
                        Role = BondDb.RoleCrew
                    });
                    users[5].Bound.Add(new BondDb
                    {
                        Farm = users[4].Bound[0].Farm,
                        Role = BondDb.RoleCrew
                    });
                    users[3].Bound.Add(new BondDb
                    {
                        Farm = users[4].Bound[0].Farm,
                        Role = BondDb.RoleCrew
                    });
                }

                Random RNG = new Random();

                //Create some fields for each farm
                foreach (var farm in AllFarms)
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

                       // farm.Fields.Add(new FieldDb(crops[RNG.Next(11)] + " " + (RNG.Next(2, 6) * fieldSize).ToString("0.#") + " ha"));
                    }

                }
                //context.SaveChanges(); 

                foreach (var user in users)
                {
                    user.UserName = Guid.NewGuid().ToString(); 
                    user.SecurityStamp = Guid.NewGuid().ToString("D");
                    user.PasswordHash = passwordHasher.HashPassword("Password1");

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
                    //foreach (var contactId in contacts)
                    //{
                    //    context.ContactBook.Add(new ContacDb
                    //    {
                    //        Person1Id = user.Id,
                    //        Person1 = user,
                    //        Friend2 = users[contactId],
                    //        Friend2Id = users[contactId].Id,
                    //        State = DBRelationshipState.Friends
                    //    });
                    //}

                }



                foreach (var user in users)
                {
                    context.Users.AddOrUpdate(user);
                }
            }

        }

    }
}
