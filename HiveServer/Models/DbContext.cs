using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using HiveServer.Models.FarmData;

namespace HiveServer.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole, long, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {

 
        public DbSet<ContactDb> Contacts { get; set; }
        public DbSet<BondDb> Bindings { get; set; }

        public DbSet<FarmDb> Farms { get; set; }
        public DbSet<FieldDb> Fields { get; set; }

        public DbSet<JobDb> Jobs { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {   Configuration.LazyLoadingEnabled = true;    }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(i => i.LastName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(i => i.FirstName).IsRequired().HasMaxLength(100);            
            modelBuilder.Entity<ApplicationUser>().Property(i => i.OTPSecret).IsRequired().HasMaxLength(256);
            modelBuilder.Entity<ApplicationUser>().Property(i => i.PhoneNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName,
            new IndexAnnotation(new IndexAttribute("IX_PhoneNumber", 1) { IsUnique = true })); 


            //COnfigure the User-Farm Bound
            modelBuilder.Entity<ApplicationUser>().ToTable("People");
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.Bound).WithRequired(p => p.Person);
            modelBuilder.Entity<FarmDb>().HasMany(p => p.Bonds).WithRequired(p => p.Farm);


            modelBuilder.Entity<BondDb>().ToTable("Bindings");
            modelBuilder.Entity<BondDb>().Property(p => p.CreatedAt).IsRequired();
            modelBuilder.Entity<BondDb>().Property(p => p.Role).IsRequired();
            modelBuilder.Entity<BondDb>().Property(p => p.Role).HasMaxLength(4);



            //Configure the Contacts
            //modelBuilder.Entity<ContacDb>().HasKey(p => new { p.Person1Id, p.Friend2Id});
            modelBuilder.Entity<ContactDb>().HasRequired(p => p.Person2).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ContactDb>().HasRequired(p => p.Person1).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ContactDb>().Property(p => p.State).HasMaxLength(2);
            modelBuilder.Entity<ContactDb>().ToTable("Contacts");


            //configure Fields
            modelBuilder.Entity<FarmDb>().HasMany(p => p.Fields).WithRequired(f => f.OnFarm);
            modelBuilder.Entity<FarmDb>().ToTable("Farms");

            modelBuilder.Entity<FieldDb>().ToTable("Fields");

            //configure jobs
            modelBuilder.Entity<JobDb>().ToTable("Jobs");
            modelBuilder.Entity<JobDb>().HasRequired(j => j.assignedBy).WithMany(p => p.JobsGiven).WillCascadeOnDelete(false);
            modelBuilder.Entity<JobDb>().HasRequired(j => j.assignedTo).WithMany(p => p.JobsRecieved).WillCascadeOnDelete(false);
            modelBuilder.Entity<JobDb>().HasRequired(j => j.onField).WithMany(f => f.Jobs).WillCascadeOnDelete(false);
            modelBuilder.Entity<JobDb>().Property(j => j.name).IsRequired();
            modelBuilder.Entity<JobDb>().Property(j => j.state).IsRequired(); ;
            modelBuilder.Entity<JobDb>().Property(j => j.type).IsRequired();

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}