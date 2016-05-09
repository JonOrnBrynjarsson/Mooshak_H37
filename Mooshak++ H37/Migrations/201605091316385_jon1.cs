namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jon1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Submissions", "User_ID", "dbo.Users");
            DropIndex("dbo.Submissions", new[] { "User_ID" });
            RenameColumn(table: "dbo.Submissions", name: "User_ID", newName: "UserID");
            AlterColumn("dbo.Submissions", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Submissions", "UserID");
            AddForeignKey("dbo.Submissions", "UserID", "dbo.Users", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Submissions", "UserID", "dbo.Users");
            DropIndex("dbo.Submissions", new[] { "UserID" });
            AlterColumn("dbo.Submissions", "UserID", c => c.Int());
            RenameColumn(table: "dbo.Submissions", name: "UserID", newName: "User_ID");
            CreateIndex("dbo.Submissions", "User_ID");
            AddForeignKey("dbo.Submissions", "User_ID", "dbo.Users", "ID");
        }
    }
}
