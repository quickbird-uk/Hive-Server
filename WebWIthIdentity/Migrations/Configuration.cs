using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebWIthIdentity.Models;
using WebWIthIdentity.Models.FarmData;

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
            bool emptyDatabase = false;
            string[] crops = { "Corn", "Maise", "Potato", "Nothing", "Wheat", "Rapeseed", "Barley", "Peas", "Oats", "Buckwheat", "Wheat" }; //11 crops to seed the database

            List<ApplicationUser> users = new List<ApplicationUser>();
            users.Add(new ApplicationUser
            {
                RealName = "Bob Stone",
                Twitter = "@Hairy",
                PhoneNumber = 777777777,
                Email = "Bob@stoned.uk"
            });

            users.Add(new ApplicationUser
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
            });

            users.Add(new ApplicationUser
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
            });

            users.Add(new ApplicationUser
            {
                RealName = "Test User",
                Twitter = "test",
                PhoneNumber = 545648,
                Email = "test@test.uk"
            });

            users.Add(new ApplicationUser
            {
                RealName = "Vladimir Akopyan",
                Twitter = "@ClumsyPilot",
                PhoneNumber = 7842723489,
                Email = "vlad@quickbird.uk",
                HouseNumber = 7,
                Address1 = "Saunton Way",
                City = "Birmingham",
                Country = "UK",
                Postcode = "B29 6TP"
            });

            users.Add(new ApplicationUser
            {
                RealName = "Manuel",
                Twitter = "UnknownMonstrosity",
                PhoneNumber = +44745272505,
                Email = "manuel@quickbird.uk",
                HouseNumber = 0,
                Address1 = "somewhere in a pit",
                City = "Birmingham",
                Country = "hell",
                Postcode = "B29 6TP"
            });

            if (!context.Farms.Any()) //seed if there are no farms 
            {
                emptyDatabase = true;
                List<Farm> AllFarms = new List<Farm>();

                //Give every user a farm! 
                foreach (var user in users)
                {
                    var farm = new Farm(user.RealName + "'s Farm");
                    var bond = new Bond
                    {
                        Type = BondType.Owner,
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
                        users[2].Bound.Add(new Bond
                        {
                            Farm = user.FarmsOwned()[0],
                            Type = BondType.Agrinomist
                        });
                    }
                }

                //Add some crew to vlad's farm
                {
                    users[0].Bound.Add(new Bond
                    {
                        Farm = users[4].FarmsOwned()[0],
                        Type = BondType.Crew
                    });
                    users[1].Bound.Add(new Bond
                    {
                        Farm = users[4].FarmsOwned()[0],
                        Type = BondType.Crew
                    });
                    users[5].Bound.Add(new Bond
                    {
                        Farm = users[4].FarmsOwned()[0],
                        Type = BondType.Manager
                    });
                    users[3].Bound.Add(new Bond
                    {
                        Farm = users[4].FarmsOwned()[0],
                        Type = BondType.Crew
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

                        farm.Fields.Add(new Field(crops[RNG.Next(11)] + " " + (RNG.Next(2, 6) * fieldSize).ToString("0.#") + " ha"));
                    }

                }
                //context.SaveChanges(); 

                foreach (var user in users)
                {
                    user.UserName = user.Id;
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
                    foreach (var contactId in contacts)
                    {
                        user.ContactBook.Add(new CBRecord
                        {
                            OwnerID = user.Id,
                            Owner = user,
                            Contact = users[contactId],
                            ContactID = users[contactId].Id,
                            Nickname = users[contactId].RealName
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
