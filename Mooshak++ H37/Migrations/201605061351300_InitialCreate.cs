namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SetDate = c.DateTime(),
                        DueDate = c.DateTime(),
                        CourseID = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Startdate = c.DateTime(nullable: false),
                        Isactive = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ErrorReports",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateOccurred = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                        CourseID = c.Int(),
                        AssignmentID = c.Int(),
                        MilestoneID = c.Int(),
                        SubmissionID = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID)
                .ForeignKey("dbo.Submissions", t => t.SubmissionID)
                .ForeignKey("dbo.Milestones", t => t.MilestoneID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID)
                .Index(t => t.UserID)
                .Index(t => t.CourseID)
                .Index(t => t.AssignmentID)
                .Index(t => t.MilestoneID)
                .Index(t => t.SubmissionID);
            
            CreateTable(
                "dbo.Milestones",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, unicode: false, storeType: "text"),
                        AllowedSubmissions = c.Int(nullable: false),
                        FileWithLocation = c.String(),
                        AssignmentID = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID, cascadeDelete: true)
                .Index(t => t.AssignmentID);
            
            CreateTable(
                "dbo.Submissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MilestoneID = c.Int(nullable: false),
                        Code = c.Binary(nullable: false),
                        TestCasePassed = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Milestones", t => t.MilestoneID, cascadeDelete: true)
                .Index(t => t.MilestoneID);
            
            CreateTable(
                "dbo.Testruns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubmissionID = c.Int(nullable: false),
                        IsSuccess = c.Boolean(nullable: false),
                        ResultComments = c.String(),
                        IsRemoved = c.Boolean(nullable: false),
                        TestCase_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Submissions", t => t.SubmissionID, cascadeDelete: true)
                .ForeignKey("dbo.TestCases", t => t.TestCase_ID)
                .Index(t => t.SubmissionID)
                .Index(t => t.TestCase_ID);
            
            CreateTable(
                "dbo.TestCases",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Inputstring = c.String(),
                        MilestoneID = c.Int(nullable: false),
                        IsOnlyForTeacher = c.Boolean(),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Milestones", t => t.MilestoneID, cascadeDelete: true)
                .Index(t => t.MilestoneID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256),
                        AspNetUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUserId, cascadeDelete: true)
                .Index(t => t.AspNetUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserCourseRelation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AssignmentID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.AssignmentID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        from = c.String(nullable: false, maxLength: 50),
                        To = c.String(nullable: false, maxLength: 50),
                        DateSent = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.GroupMembers", "UserID", "dbo.Users");
            DropForeignKey("dbo.GroupMembers", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.ErrorReports", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.UserCourseRelation", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserCourseRelation", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.UserCourseRelation", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.ErrorReports", "UserID", "dbo.Users");
            DropForeignKey("dbo.Users", "AspNetUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ErrorReports", "MilestoneID", "dbo.Milestones");
            DropForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases");
            DropForeignKey("dbo.TestCases", "MilestoneID", "dbo.Milestones");
            DropForeignKey("dbo.Testruns", "SubmissionID", "dbo.Submissions");
            DropForeignKey("dbo.Submissions", "MilestoneID", "dbo.Milestones");
            DropForeignKey("dbo.ErrorReports", "SubmissionID", "dbo.Submissions");
            DropForeignKey("dbo.Milestones", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.ErrorReports", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.Assignments", "CourseID", "dbo.Courses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.GroupMembers", new[] { "UserID" });
            DropIndex("dbo.GroupMembers", new[] { "AssignmentID" });
            DropIndex("dbo.UserCourseRelation", new[] { "RoleID" });
            DropIndex("dbo.UserCourseRelation", new[] { "UserID" });
            DropIndex("dbo.UserCourseRelation", new[] { "CourseID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Users", new[] { "AspNetUserId" });
            DropIndex("dbo.TestCases", new[] { "MilestoneID" });
            DropIndex("dbo.Testruns", new[] { "TestCase_ID" });
            DropIndex("dbo.Testruns", new[] { "SubmissionID" });
            DropIndex("dbo.Submissions", new[] { "MilestoneID" });
            DropIndex("dbo.Milestones", new[] { "AssignmentID" });
            DropIndex("dbo.ErrorReports", new[] { "SubmissionID" });
            DropIndex("dbo.ErrorReports", new[] { "MilestoneID" });
            DropIndex("dbo.ErrorReports", new[] { "AssignmentID" });
            DropIndex("dbo.ErrorReports", new[] { "CourseID" });
            DropIndex("dbo.ErrorReports", new[] { "UserID" });
            DropIndex("dbo.Assignments", new[] { "CourseID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.Roles");
            DropTable("dbo.UserCourseRelation");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Users");
            DropTable("dbo.TestCases");
            DropTable("dbo.Testruns");
            DropTable("dbo.Submissions");
            DropTable("dbo.Milestones");
            DropTable("dbo.ErrorReports");
            DropTable("dbo.Courses");
            DropTable("dbo.Assignments");
        }
    }
}
