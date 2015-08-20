using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using WebWIthIdentity.Models.FarmData;

namespace WebWIthIdentity.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Farm> Farms { get; set; }
        public DbSet<CBRecord> ContactBook { get; set; }
        public DbSet<Bond> Bindings { get; set; }

        public DbSet<Field> Fields { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = true;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //COnfigure the User-Farm Bound
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.Bound).WithRequired(p => p.Person);
            modelBuilder.Entity<Farm>().HasMany(p => p.Bound).WithRequired(p => p.Farm);
            modelBuilder.Entity<Bond>().HasKey(p => new {p.FarmID, p.PersonID});
            modelBuilder.Entity<Bond>().ToTable("UserFarmBindings");
            modelBuilder.Entity<Bond>().Property(p => p.Created).IsRequired();
            modelBuilder.Entity<Bond>().Property(p => p.Type).IsRequired();


            //
         

            //Configure the COntactBook
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