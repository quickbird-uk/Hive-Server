namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeftEmailAlone : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "Email" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.AspNetUsers", "Email");
        }
    }
}
