using HiveServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class _Person : _Entity
    {

        public string firstName { get; set; }

        public string lastName { get; set; }

        public long phone { get; set; }


        public static explicit operator _Person(ApplicationUser v)
        {
            return new _Person
            {
                id = v.Id,
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