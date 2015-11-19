namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedAlmostEverything : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tasks", new[] { "assignedById" });
            DropIndex("dbo.Tasks", new[] { "assignedToId" });
            RenameColumn(table: "dbo.Fields", name: "OrgId", newName: "Org_Id");
            RenameColumn(table: "dbo.Tasks", name: "onFieldId", newName: "ForFieldID");
            RenameIndex(table: "dbo.Fields", name: "IX_OrgId", newName: "IX_Org_Id");
            RenameIndex(table: "dbo.Tasks", name: "IX_onFieldId", newName: "IX_ForFieldID");
            AddColumn("dbo.Bindings", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.Bindings", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Bindings", "MarkedDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organisations", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Organisations", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Organisations", "MarkedDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Fields", "AreaInHectares", c => c.Double(nullable: false));
            AddColumn("dbo.Fields", "onOrganisationID", c => c.Long(nullable: false));
            AddColumn("dbo.Fields", "Lattitude", c => c.Double(nullable: false));
            AddColumn("dbo.Fields", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Fields", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Fields", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Fields", "MarkedDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tasks", "TaskDescription", c => c.String());
            AddColumn("dbo.Tasks", "PayRate", c => c.Double(nullable: false));
            AddColumn("dbo.Tasks", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Tasks", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Tasks", "MarkedDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Contacts", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Contacts", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Contacts", "MarkedDeleted", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Tasks", "AssignedByID");
            CreateIndex("dbo.Tasks", "AssignedToID");
            DropColumn("dbo.Bindings", "CreatedAt");
            DropColumn("dbo.Bindings", "UpdatedAt");
            DropColumn("dbo.Bindings", "Deleted");
            DropColumn("dbo.Organisations", "CreatedAt");
            DropColumn("dbo.Organisations", "UpdatedAt");
            DropColumn("dbo.Organisations", "Deleted");
            DropColumn("dbo.Fields", "size");
            DropColumn("dbo.Fields", "CreatedAt");
            DropColumn("dbo.Fields", "UpdatedAt");
            DropColumn("dbo.Fields", "Deleted");
            DropColumn("dbo.Tasks", "jobDescription");
            DropColumn("dbo.Tasks", "rate");
            DropColumn("dbo.Tasks", "timeSpent");
            DropColumn("dbo.Tasks", "CreatedAt");
            DropColumn("dbo.Tasks", "UpdatedAt");
            DropColumn("dbo.Tasks", "Deleted");
            DropColumn("dbo.Contacts", "CreatedAt");
            DropColumn("dbo.Contacts", "UpdatedAt");
            DropColumn("dbo.Contacts", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Contacts", "UpdatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Contacts", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Tasks", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tasks", "UpdatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Tasks", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Tasks", "timeSpent", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Tasks", "rate", c => c.Double(nullable: false));
            AddColumn("dbo.Tasks", "jobDescription", c => c.String());
            AddColumn("dbo.Fields", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Fields", "UpdatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Fields", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Fields", "size", c => c.Double(nullable: false));
            AddColumn("dbo.Organisations", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organisations", "UpdatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Organisations", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Bindings", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Bindings", "UpdatedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Bindings", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            DropIndex("dbo.Tasks", new[] { "AssignedToID" });
            DropIndex("dbo.Tasks", new[] { "AssignedByID" });
            DropColumn("dbo.Contacts", "MarkedDeleted");
            DropColumn("dbo.Contacts", "UpdatedOn");
            DropColumn("dbo.Contacts", "CreatedOn");
            DropColumn("dbo.Tasks", "MarkedDeleted");
            DropColumn("dbo.Tasks", "UpdatedOn");
            DropColumn("dbo.Tasks", "CreatedOn");
            DropColumn("dbo.Tasks", "PayRate");
            DropColumn("dbo.Tasks", "TaskDescription");
            DropColumn("dbo.Fields", "MarkedDeleted");
            DropColumn("dbo.Fields", "UpdatedOn");
            DropColumn("dbo.Fields", "CreatedOn");
            DropColumn("dbo.Fields", "Longitude");
            DropColumn("dbo.Fields", "Lattitude");
            DropColumn("dbo.Fields", "onOrganisationID");
            DropColumn("dbo.Fields", "AreaInHectares");
            DropColumn("dbo.Organisations", "MarkedDeleted");
            DropColumn("dbo.Organisations", "UpdatedOn");
            DropColumn("dbo.Organisations", "CreatedOn");
            DropColumn("dbo.Bindings", "MarkedDeleted");
            DropColumn("dbo.Bindings", "UpdatedOn");
            DropColumn("dbo.Bindings", "CreatedOn");
            RenameIndex(table: "dbo.Tasks", name: "IX_ForFieldID", newName: "IX_onFieldId");
            RenameIndex(table: "dbo.Fields", name: "IX_Org_Id", newName: "IX_OrgId");
            RenameColumn(table: "dbo.Tasks", name: "ForFieldID", newName: "onFieldId");
            RenameColumn(table: "dbo.Fields", name: "Org_Id", newName: "OrgId");
            CreateIndex("dbo.Tasks", "assignedToId");
            CreateIndex("dbo.Tasks", "assignedById");
        }
    }
}
