namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedOrganisation : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Fields", "onOrganisationID");
            RenameColumn(table: "dbo.Fields", name: "Org_Id", newName: "onOrganisationID");
            RenameIndex(table: "dbo.Fields", name: "IX_Org_Id", newName: "IX_onOrganisationID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Fields", name: "IX_onOrganisationID", newName: "IX_Org_Id");
            RenameColumn(table: "dbo.Fields", name: "onOrganisationID", newName: "Org_Id");
            AddColumn("dbo.Fields", "onOrganisationID", c => c.Long(nullable: false));
        }
    }
}
