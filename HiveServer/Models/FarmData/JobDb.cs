using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HiveServer.Models
{
    public class JobDb : Base.Entity
    {

        public JobDb()
        {

        }


       

        public string name { get; set; }
        public string jobDescription { get; set; }



    }

  
}

