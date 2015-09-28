namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEventsToJobs1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "size", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fields", "size");
        }
    }
}
