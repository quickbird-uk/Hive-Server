using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.DTO
{
    /// <summary> Base class for every item in the database, it spesifies common properties used by all of them </summary>
    public class _Entity
    {
        public _Entity()
        {
            createdOn = DateTime.UtcNow;
            updatedOn = DateTime.UtcNow;
            markedDeleted = false; 
        }

        /// <summary> Unique identifier of this item </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        /// <summary> Spesifies when it was created for the first time </summary>
        public DateTimeOffset createdOn { get; set; }


        /// <summary> Spesifies when this entity was updated for the last time </summary>
        public DateTimeOffset updatedOn { get; set; }

        /// <summary> Version of the item in question, this is used for concurrency </summary>
        [Timestamp]
        public byte[] version { get; set; }

        /// <summary> marks Entity As deleted </summary>
        public bool markedDeleted { get; set; }

        /// <summary> Setting this false makes validation not fail when a new object is created by the client,
       ///  and  fields such as version are not set </summary>
        internal bool oldObject = true; 
    }
}