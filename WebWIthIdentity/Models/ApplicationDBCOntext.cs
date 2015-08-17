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
        public DbSet<CBRecord> ContactBook { get; set; }


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

            modelBuilder.Entity<CBRecord>().HasKey(p => new { p.OwnerID, p.ContactID});
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.ContactBook).WithRequired(p => p.Owner).WillCascadeOnDelete(false);
            modelBuilder.Entity<CBRecord>().HasRequired(p => p.Contact);
            modelBuilder.Entity<CBRecord>().ToTable("ContactBook");



        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}