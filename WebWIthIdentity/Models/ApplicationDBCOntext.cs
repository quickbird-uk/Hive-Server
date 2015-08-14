using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebWIthIdentity.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Farm> Farms { get; set; }



        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false; 
           
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.FarmsOwned).WithMany(p => p.Managers).Map(cs =>
            {
                cs.MapLeftKey("FarmOwners");
                cs.MapRightKey("FarmsOwned");
                cs.ToTable("FarmOwnership"); 
                
            });
            
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.FarmsWorking).WithMany(p => p.Crew).Map(cs =>
            {
                cs.MapLeftKey("FarmCrew");
                cs.MapRightKey("FarmsWorked");
                cs.ToTable("FarmWorkforce");
            });


        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}