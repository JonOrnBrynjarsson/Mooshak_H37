namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Testruns", "TestCase", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Testruns", "TestCase");
        }
    }
}
