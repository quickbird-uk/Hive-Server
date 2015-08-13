namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class overrodeusername : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "PhoneNumber" });
            CreateIndex("dbo.AspNetUsers", "PhoneNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "PhoneNumber" });
            CreateIndex("dbo.AspNetUsers", "PhoneNumber");
        }
    }
}
