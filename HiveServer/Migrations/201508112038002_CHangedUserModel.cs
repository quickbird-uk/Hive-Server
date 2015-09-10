namespace WebWIthIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CHangedUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RealName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.Long(nullable: false));
            CreateIndex("dbo.AspNetUsers", "PhoneNumber", unique: true, name: "PhoneNumberIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", "PhoneNumberIndex");
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String());
            DropColumn("dbo.AspNetUsers", "RealName");
        }
    }
}
