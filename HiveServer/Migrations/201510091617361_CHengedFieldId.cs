namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CHengedFieldId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Fields", name: "Org_Id", newName: "OrgId");
            RenameIndex(table: "dbo.Fields", name: "IX_Org_Id", newName: "IX_OrgId");
            DropColumn("dbo.Fields", "OnOrgId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Fields", "OnOrgId", c => c.Long(nullable: false));
            RenameIndex(table: "dbo.Fields", name: "IX_OrgId", newName: "IX_Org_Id");
            RenameColumn(table: "dbo.Fields", name: "OrgId", newName: "Org_Id");
        }
    }
}
