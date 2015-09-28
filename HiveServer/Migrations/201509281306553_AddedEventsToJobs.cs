namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEventsToJobs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "EventLog", c => c.String());
            DropColumn("dbo.Jobs", "lastAction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Jobs", "lastAction", c => c.String());
            DropColumn("dbo.Jobs", "EventLog");
        }
    }
}
