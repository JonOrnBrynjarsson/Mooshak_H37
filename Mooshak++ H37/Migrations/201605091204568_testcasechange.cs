namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testcasechange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases");
            DropPrimaryKey("dbo.TestCases");
            AlterColumn("dbo.TestCases", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TestCases", "ID");
            AddForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases");
            DropPrimaryKey("dbo.TestCases");
            AlterColumn("dbo.TestCases", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TestCases", "ID");
            AddForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases", "ID");
        }
    }
}
