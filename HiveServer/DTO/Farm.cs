using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;



namespace HiveServer.DTO
{
    public class Farm: Base.Entity
    {
        public string name { get; set; }

        public string farmDescription { get; set; }

        public string role { get; set; }


        public static explicit operator Farm(Models.FarmData.BondDb v)
        {
            var result = new Farm
            {
                Id = v.Farm.Id,
                name = v.Farm.Name,
                farmDescription = v.Farm.Description,
                CreatedAt = v.Farm.CreatedAt,
                UpdatedAt = v.Farm.UpdatedAt,
                role = v.Role,
                Version = v.Version,
                Deleted = v.Farm.Deleted
            };
            return result;
        }

    }
}