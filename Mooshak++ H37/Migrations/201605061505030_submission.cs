namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class submission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submissions", "Grade", c => c.Double(nullable: false));
            AddColumn("dbo.Submissions", "ProgramFileLocation", c => c.String(nullable: false));
            AddColumn("dbo.Submissions", "User_ID", c => c.Int());
            CreateIndex("dbo.Submissions", "User_ID");
            AddForeignKey("dbo.Submissions", "User_ID", "dbo.Users", "ID");
            DropColumn("dbo.Submissions", "Code");
            DropColumn("dbo.Submissions", "TestCasePassed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Submissions", "TestCasePassed", c => c.Int(nullable: false));
            AddColumn("dbo.Submissions", "Code", c => c.Binary(nullable: false));
            DropForeignKey("dbo.Submissions", "User_ID", "dbo.Users");
            DropIndex("dbo.Submissions", new[] { "User_ID" });
            DropColumn("dbo.Submissions", "User_ID");
            DropColumn("dbo.Submissions", "ProgramFileLocation");
            DropColumn("dbo.Submissions", "Grade");
        }
    }
}
