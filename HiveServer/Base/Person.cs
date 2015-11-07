using HiveServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.Base
{
    public class Person : Entity
    {

        public string firstName { get; set; }

        public string lastName { get; set; }

        public long phone { get; set; }


        public static explicit operator Person(ApplicationUser v)
        {
            return new Person
            {
                Id = v.Id,
                firstName = v.FirstName,
                lastName = v.LastName,
                phone = v.PhoneNumber,

                //CreatedAt = v.CreatedAt,
                //UpdatedAt = ,
                //Version = null,

                //Deleted = false

            };
        }
    }

}