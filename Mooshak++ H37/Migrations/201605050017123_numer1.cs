namespace Mooshak___H37.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class numer1 : DbMigration
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
                        UserID = c.String(nullable: false, maxLength: 128),
                        CourseID = c.Int(),
                        AssignmentID = c.Int(),
                        MilestoneID = c.Int(),
                        SubmissionID = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        Message = c.String(),
                        AspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID)
                .ForeignKey("dbo.Courses", t => t.CourseID)
                .ForeignKey("dbo.Submissions", t => t.SubmissionID)
                .Index(t => t.CourseID)
                .Index(t => t.AssignmentID)
                .Index(t => t.SubmissionID)
                .Index(t => t.AspNetUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers1",
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles1",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserCourseRelation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                        RoleID = c.String(nullable: false, maxLength: 128),
                        IsRemoved = c.Boolean(nullable: false),
                        AspNetRole_Id = c.String(maxLength: 128),
                        AspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetRoles1", t => t.AspNetRole_Id)
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.AspNetRole_Id)
                .Index(t => t.AspNetUser_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims1",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        AspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id)
                .Index(t => t.AspNetUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins1",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        AspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id)
                .Index(t => t.AspNetUser_Id);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AssignmentID = c.Int(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                        IsRemoved = c.Boolean(nullable: false),
                        AspNetUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID, cascadeDelete: true)
                .Index(t => t.AssignmentID)
                .Index(t => t.AspNetUser_Id);
            
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
                "dbo.Milestones",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, unicode: false, storeType: "text"),
                        AllowedSubmissions = c.Int(nullable: false),
                        Grade = c.Double(),
                        AssignmentID = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID, cascadeDelete: true)
                .Index(t => t.AssignmentID);
            
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                "dbo.AspNetRoleAspNetUsers",
                c => new
                    {
                        AspNetRole_Id = c.String(nullable: false, maxLength: 128),
                        AspNetUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AspNetRole_Id, t.AspNetUser_Id })
                .ForeignKey("dbo.AspNetRoles1", t => t.AspNetRole_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers1", t => t.AspNetUser_Id, cascadeDelete: true)
                .Index(t => t.AspNetRole_Id)
                .Index(t => t.AspNetUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Testruns", "TestCase_ID", "dbo.TestCases");
            DropForeignKey("dbo.Testruns", "SubmissionID", "dbo.Submissions");
            DropForeignKey("dbo.TestCases", "MilestoneID", "dbo.Milestones");
            DropForeignKey("dbo.Submissions", "MilestoneID", "dbo.Milestones");
            DropForeignKey("dbo.Milestones", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.ErrorReports", "SubmissionID", "dbo.Submissions");
            DropForeignKey("dbo.ErrorReports", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.ErrorReports", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.GroupMembers", "AssignmentID", "dbo.Assignments");
            DropForeignKey("dbo.GroupMembers", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.ErrorReports", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.AspNetUserLogins1", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.AspNetUserClaims1", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.UserCourseRelation", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.UserCourseRelation", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.UserCourseRelation", "AspNetRole_Id", "dbo.AspNetRoles1");
            DropForeignKey("dbo.AspNetRoleAspNetUsers", "AspNetUser_Id", "dbo.AspNetUsers1");
            DropForeignKey("dbo.AspNetRoleAspNetUsers", "AspNetRole_Id", "dbo.AspNetRoles1");
            DropForeignKey("dbo.Assignments", "CourseID", "dbo.Courses");
            DropIndex("dbo.AspNetRoleAspNetUsers", new[] { "AspNetUser_Id" });
            DropIndex("dbo.AspNetRoleAspNetUsers", new[] { "AspNetRole_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Testruns", new[] { "TestCase_ID" });
            DropIndex("dbo.Testruns", new[] { "SubmissionID" });
            DropIndex("dbo.TestCases", new[] { "MilestoneID" });
            DropIndex("dbo.Milestones", new[] { "AssignmentID" });
            DropIndex("dbo.Submissions", new[] { "MilestoneID" });
            DropIndex("dbo.GroupMembers", new[] { "AspNetUser_Id" });
            DropIndex("dbo.GroupMembers", new[] { "AssignmentID" });
            DropIndex("dbo.AspNetUserLogins1", new[] { "AspNetUser_Id" });
            DropIndex("dbo.AspNetUserClaims1", new[] { "AspNetUser_Id" });
            DropIndex("dbo.UserCourseRelation", new[] { "AspNetUser_Id" });
            DropIndex("dbo.UserCourseRelation", new[] { "AspNetRole_Id" });
            DropIndex("dbo.UserCourseRelation", new[] { "CourseID" });
            DropIndex("dbo.ErrorReports", new[] { "AspNetUser_Id" });
            DropIndex("dbo.ErrorReports", new[] { "SubmissionID" });
            DropIndex("dbo.ErrorReports", new[] { "AssignmentID" });
            DropIndex("dbo.ErrorReports", new[] { "CourseID" });
            DropIndex("dbo.Assignments", new[] { "CourseID" });
            DropTable("dbo.AspNetRoleAspNetUsers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.Testruns");
            DropTable("dbo.TestCases");
            DropTable("dbo.Milestones");
            DropTable("dbo.Submissions");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.AspNetUserLogins1");
            DropTable("dbo.AspNetUserClaims1");
            DropTable("dbo.UserCourseRelation");
            DropTable("dbo.AspNetRoles1");
            DropTable("dbo.AspNetUsers1");
            DropTable("dbo.ErrorReports");
            DropTable("dbo.Courses");
            DropTable("dbo.Assignments");
        }
    }
}
