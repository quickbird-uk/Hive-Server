using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.Base
{
    public class Address
    {
        /*house Number*/
        public int HouseNumber { get; set; }

        /*three lines for address, one should be mandatory*/
        public string Address1 { get; set; } //Not Null
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        public string City { get; set; } //Not Null

        public string Country { get; set; } //Not Null

        public string Postcode { get; set; } //Not Null
    }
}