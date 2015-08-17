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

            ApplicationUser[] users = new ApplicationUser[5];
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

            users[4] = new ApplicationUser
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
            };

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

                users[1].ContactBook.Add(new CBRecord
                {
                    Owner = users[1],
                    OwnerID = users[1].Id,
                    Contact = users[0],
                    ContactID = users[0].Id,
                    Nickname = users[0].RealName
                });

                users[1].ContactBook.Add(new CBRecord
                {
                    Owner = users[1],
                    OwnerID = users[1].Id,
                    Contact = users[3],
                    ContactID = users[3].Id,
                    Nickname = users[3].RealName
                });


                foreach (var user in users)
                {
                    context.Users.AddOrUpdate(user);
                }
            }



        }
    }
}
