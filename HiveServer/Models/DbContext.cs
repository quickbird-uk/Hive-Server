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

        public DbSet<OrganisationDb> Organisations { get; set; }
        public DbSet<FieldDb> Fields { get; set; }

        public DbSet<TaskDb> Tasks { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {   Configuration.LazyLoadingEnabled = true;    }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Enable logging
            Database.Log = Console.Write;


            modelBuilder.Entity<ApplicationUser>().Property(i => i.LastName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(i => i.FirstName).IsRequired().HasMaxLength(100);            
            modelBuilder.Entity<ApplicationUser>().Property(i => i.OTPSecret).IsRequired().HasMaxLength(256);
            modelBuilder.Entity<ApplicationUser>().Property(i => i.PhoneNumber).HasColumnAnnotation(IndexAnnotation.AnnotationName,
            new IndexAnnotation(new IndexAttribute("IX_PhoneNumber", 1) { IsUnique = true })); 


            //COnfigure the User-organisation Bond
            modelBuilder.Entity<ApplicationUser>().ToTable("People");
            modelBuilder.Entity<ApplicationUser>().HasMany(p => p.Bound).WithRequired(p => p.Person);
            modelBuilder.Entity<OrganisationDb>().HasMany(p => p.Bonds).WithRequired(p => p.Organisation);


            modelBuilder.Entity<BondDb>().ToTable("Bindings");
            modelBuilder.Entity<BondDb>().Property(p => p.CreatedOn).IsRequired();
            modelBuilder.Entity<BondDb>().Property(p => p.Role).IsRequired();
            modelBuilder.Entity<BondDb>().Property(p => p.Role).HasMaxLength(4);



            //Configure the Contacts
            //modelBuilder.Entity<ContacDb>().HasKey(p => new { p.Person1Id, p.Friend2Id});
            modelBuilder.Entity<ContactDb>().HasRequired(p => p.Person2).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ContactDb>().HasRequired(p => p.Person1).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ContactDb>().Property(p => p.State).HasMaxLength(2).IsFixedLength();
            modelBuilder.Entity<ContactDb>().ToTable("Contacts");


            //configure Fields
            modelBuilder.Entity<OrganisationDb>().HasMany(p => p.Fields).WithRequired(f => f.Org);
            modelBuilder.Entity<OrganisationDb>().ToTable("Organisations");

            modelBuilder.Entity<FieldDb>().ToTable("Fields");

            //configure jobs
            modelBuilder.Entity<TaskDb>().ToTable("Tasks");
            modelBuilder.Entity<TaskDb>().HasRequired(j => j.AssignedBy).WithMany(p => p.JobsGiven).WillCascadeOnDelete(false);
            modelBuilder.Entity<TaskDb>().HasRequired(j => j.AssignedTo).WithMany(p => p.JobsRecieved).WillCascadeOnDelete(false);
            modelBuilder.Entity<TaskDb>().HasRequired(j => j.ForField).WithMany(f => f.Jobs).WillCascadeOnDelete(false);
            modelBuilder.Entity<TaskDb>().Property(j => j.Name).IsRequired();
            modelBuilder.Entity<TaskDb>().Property(j => j.State).IsRequired(); // .IsFixedLength().HasMaxLength(3)
            modelBuilder.Entity<TaskDb>().Property(j => j.Type).IsRequired();
            modelBuilder.Entity<TaskDb>().Property(j => j.DateFinished).HasColumnType("datetime2");
            modelBuilder.Entity<TaskDb>().Property(j => j.DueDate).HasColumnType("datetime2");


        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}