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
            string[] crops = {"Corn", "Maise", "Potato", "Nothing", "Wheat", "Rapeseed", "Barley", "Peas", "Oats", "Buckwheat", "Wheat"}; //11 crops to seed the database

            List<ApplicationUser> users = new List<ApplicationUser>();
            users.Add(  new ApplicationUser
            {
                RealName = "Bob Stone",
                Twitter = "@Hairy",
                PhoneNumber = 777777777,
                Email = "Bob@stoned.uk"
            });

            users.Add( new ApplicationUser
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

            users.Add( new ApplicationUser
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

            users.Add( new ApplicationUser
            {
                RealName = "Test Uset",
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
                RealName = "Many",
                Twitter = "UnknownMonstrosity",
                PhoneNumber = +44745272505,
                Email = "manuel@quickbird.uk",
                HouseNumber = 0,
                Address1 = "somewhere in a pit",
                City = "Birmingham",
                Country = "hell",
                Postcode = "B29 6TP"
            });

            if (! context.Farms.Any()) //seed if there are no farms 
            {
                emptyDatabase = true;
                List<Farm> AllFarms = new List<Farm>();

                foreach (var user in users)
                {
                    var farm = new Farm(user.RealName + "'s Farm"); 
                    user.FarmsOwned.Add(farm);
                    AllFarms.Add(farm);
                }


                foreach (var  user in users)
                {
                    users[1].FarmsWorking.Add(user.FarmsOwned[0]);
                }
                users[0].FarmsWorking.Add(users[3].FarmsOwned[0]);

                Random RNG = new Random();
                
                users[4].FarmsOwned.Add(new Farm("Another Farm", "this is a huge macdonalds farm!"));
                AllFarms.Add(users[4].FarmsOwned[1]);
                users[1].FarmsWorking.Add(users[4].FarmsOwned[1]);



                foreach (var farm in AllFarms)
                {
                    int nOfFields = RNG.Next(3, 20);
                    double fieldSize = 0;
                    while(fieldSize  < 0.1)
                    {
                        fieldSize = RNG.NextDouble();
                    }
                    fieldSize = fieldSize*10; 

                    for (int i=0; i < nOfFields; i++)
                    {
                       
                        farm.Fields.Add(new Field(crops[RNG.Next(11)] + " " + (RNG.Next(2, 6) * fieldSize).ToString("0.#") + " ha"));
                    }
                            
                }

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
                    int[] Contacts = new int[nOfContacts];

                    for(int i=0; i < Contacts.Length; i++)
                    {
                        int newValue = RNG.Next(0, users.Count); //generate a random index for the contact 
                        //we cannot have two links two records point ot he same contact, so they need to be unique
                        while (Contacts.Contains(newValue))
                        {
                            newValue = RNG.Next(0, users.Count);
                        }
                        Contacts[i] = newValue; //when we assured that the value does not already exist, we assign it into the array
                        
                    }
                    //Push the records and contacts in
                    foreach (var contactId in Contacts)
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
