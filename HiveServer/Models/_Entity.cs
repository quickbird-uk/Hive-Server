using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models
{
    /// <summary> Base class for every item in the database, it spesifies common properties used by all of them </summary>
    public class _Entity
    {
        public _Entity()
        {
            CreatedOn = DateTime.UtcNow;
            UpdatedOn = DateTime.UtcNow;
            MarkedDeleted = false; 
        }

        /// <summary> Unique identifier of this item </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary> Spesifies when it was created for the first time </summary>
        public DateTimeOffset CreatedOn { get; set; }


        /// <summary> Spesifies when this entity was updated for the last time </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary> Version of the item in question, this is used for concurrency </summary>
        [Timestamp]
        public byte[] Version { get; set; }

        /// <summary> marks Entity As deleted </summary>
        public bool MarkedDeleted { get; set; }

    }
}