namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testcase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestCases", "Outputstring", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestCases", "Outputstring");
        }
    }
}
