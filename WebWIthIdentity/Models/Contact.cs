using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebWIthIdentity.Models
{
    /// <summary>
    /// Records form a contact book. Contact book is a list of people each user knows on the system. 
    /// </summary>
    public class Contact
    {
        //[Required]
        //[Key, Column(Order = 0), ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string OwnerID { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        
        //[Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
       // [ForeignKey("Connection")]
        public string ConnectionID { get; set; }

        public virtual ApplicationUser Connection { get; set; }

        public string Nickname { get; set; }

    }
}