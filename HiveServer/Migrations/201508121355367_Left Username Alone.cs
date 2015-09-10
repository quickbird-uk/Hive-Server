namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeftUsernameAlone : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "UserName" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.AspNetUsers", "UserName");
        }
    }
}
