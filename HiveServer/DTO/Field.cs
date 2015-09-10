using HiveServer.Models.FarmData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveServer.DTO
{
    public class Field : Base.Entity
    {
        public string name { get; set; }

        public string fieldDescription { get; set; }


        public static explicit operator Field(FieldDb v)
        {
            return new Field
            {
                Id = v.Id,
                name = v.Name,
                fieldDescription = v.FieldDescription,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                Version = v.Version,
                Deleted = v.Deleted

            };
        }

    }
}