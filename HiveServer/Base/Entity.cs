using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Base
{
    /// <summary> Base class for every item in the database, it spesifies common properties used by all of them </summary>
    public class Entity
    {
        public Entity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Deleted = false; 
        }

        /// <summary> Unique identifier of this item </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary> Spesifies when it was created for the first time </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary> Spesifies when this entity was updated for the last time </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary> Version of the item in question, this is used for concurrency </summary>
        [Timestamp]
        public byte[] Version { get; set; }

        /// <summary> marks Entity As deleted </summary>
        public bool Deleted { get; set; }
    }
}