namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datesubmitted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submissions", "DateSubmitted", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Submissions", "DateSubmitted");
        }
    }
}
