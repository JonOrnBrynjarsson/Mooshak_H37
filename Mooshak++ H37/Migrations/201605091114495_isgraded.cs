namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isgraded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submissions", "IsGraded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Submissions", "IsGraded");
        }
    }
}
