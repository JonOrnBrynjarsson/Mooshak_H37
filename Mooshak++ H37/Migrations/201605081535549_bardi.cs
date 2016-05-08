namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bardi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Milestones", "Percentage", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Milestones", "Percentage");
        }
    }
}
