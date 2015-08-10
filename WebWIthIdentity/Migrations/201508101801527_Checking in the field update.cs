namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Checkinginthefieldupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "lastUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fields", "lastUpdated");
        }
    }
}
