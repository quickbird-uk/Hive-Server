using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class Action
    {
        public DateTime Timesamp {get; set;}

        public string Name { get; set; }

        public string Description { get; set; }

        public string PersonName { get; set; }

        public long PersonId { get; set; }

    }
}