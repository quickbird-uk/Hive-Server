using HiveServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
namespace HiveServer.DTO
{
      
    public class Contact: Base.Person
    {
        public long personID { get; set; }
        //State of the contact, such as active, blocked, ets. 
        public string state { get; set; }
    }
}