namespace HiveServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedDatatime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bindings", "UpdatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Organisations", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Organisations", "UpdatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Fields", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Fields", "UpdatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Tasks", "DueDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Tasks", "DateFinished", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Tasks", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Tasks", "UpdatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Contacts", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Contacts", "UpdatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contacts", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Contacts", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Tasks", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Tasks", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Tasks", "DateFinished", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Tasks", "DueDate", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Fields", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Fields", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Organisations", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Organisations", "CreatedOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Bindings", "UpdatedOn", c => c.DateTimeOffset(precision: 7));
        }
    }
}
