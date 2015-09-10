namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTwitter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Twitter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Twitter");
        }
    }
}
