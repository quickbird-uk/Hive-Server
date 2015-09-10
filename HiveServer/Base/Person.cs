using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.Base
{
    public class Person : Entity
    {
        public long personID { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public long phone { get; set; }
    }
}