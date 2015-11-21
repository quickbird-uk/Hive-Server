namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bindings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PersonID = c.Long(nullable: false),
                        OrganisationID = c.Long(nullable: false),
                        Role = c.String(nullable: false, maxLength: 10),
                        CreatedOn = c.DateTimeOffset(nullable: false, precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationID, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.PersonID)
                .Index(t => t.OrganisationID);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        AreaInHectares = c.Double(nullable: false),
                        FieldDescription = c.String(),
                        ParcelNumber = c.String(),
                        onOrganisationID = c.Long(nullable: false),
                        Lattitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.onOrganisationID, cascadeDelete: true)
                .Index(t => t.onOrganisationID);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        TaskDescription = c.String(),
                        Type = c.String(nullable: false),
                        ForFieldID = c.Long(nullable: false),
                        AssignedByID = c.Long(nullable: false),
                        AssignedToID = c.Long(nullable: false),
                        DueDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateFinished = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        State = c.String(nullable: false),
                        EventLog = c.String(),
                        PayRate = c.Double(nullable: false),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.AssignedByID)
                .ForeignKey("dbo.People", t => t.AssignedToID)
                .ForeignKey("dbo.Fields", t => t.ForFieldID)
                .Index(t => t.ForFieldID)
                .Index(t => t.AssignedByID)
                .Index(t => t.AssignedToID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PhoneNumber = c.Long(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        OTPSecret = c.Binary(nullable: false, maxLength: 256),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PhoneNumber, unique: true)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.People", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        RoleId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.People", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Person1Id = c.Long(nullable: false),
                        Person2Id = c.Long(nullable: false),
                        State = c.String(maxLength: 2, fixedLength: true),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        MarkedDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.Person1Id)
                .ForeignKey("dbo.People", t => t.Person2Id)
                .Index(t => new { t.Person1Id, t.Person2Id, t.State }, name: "ForIndex")
                .Index(t => new { t.Person2Id, t.Person1Id, t.State }, name: "RevIndex");
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Contacts", "Person2Id", "dbo.People");
            DropForeignKey("dbo.Contacts", "Person1Id", "dbo.People");
            DropForeignKey("dbo.Fields", "onOrganisationID", "dbo.Organisations");
            DropForeignKey("dbo.Tasks", "ForFieldID", "dbo.Fields");
            DropForeignKey("dbo.Tasks", "AssignedToID", "dbo.People");
            DropForeignKey("dbo.Tasks", "AssignedByID", "dbo.People");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.People");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.People");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.People");
            DropForeignKey("dbo.Bindings", "PersonID", "dbo.People");
            DropForeignKey("dbo.Bindings", "OrganisationID", "dbo.Organisations");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Contacts", "RevIndex");
            DropIndex("dbo.Contacts", "ForIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.People", "UserNameIndex");
            DropIndex("dbo.People", new[] { "PhoneNumber" });
            DropIndex("dbo.Tasks", new[] { "AssignedToID" });
            DropIndex("dbo.Tasks", new[] { "AssignedByID" });
            DropIndex("dbo.Tasks", new[] { "ForFieldID" });
            DropIndex("dbo.Fields", new[] { "onOrganisationID" });
            DropIndex("dbo.Bindings", new[] { "OrganisationID" });
            DropIndex("dbo.Bindings", new[] { "PersonID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Contacts");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.People");
            DropTable("dbo.Tasks");
            DropTable("dbo.Fields");
            DropTable("dbo.Organisations");
            DropTable("dbo.Bindings");
        }
    }
}
