namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeIndexesNotUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", "PhoneNumberIndex");
            CreateIndex("dbo.AspNetUsers", "PhoneNumber");
            CreateIndex("dbo.AspNetUsers", "Email");
            CreateIndex("dbo.AspNetUsers", "UserName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "UserName" });
            DropIndex("dbo.AspNetUsers", new[] { "Email" });
            DropIndex("dbo.AspNetUsers", new[] { "PhoneNumber" });
            CreateIndex("dbo.AspNetUsers", "PhoneNumber", unique: true, name: "PhoneNumberIndex");
        }
    }
}
