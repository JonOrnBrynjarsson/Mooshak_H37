using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak___H37.Models.Entities;


namespace Mooshak___H37.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

	public interface IAppDataContext
	{
		IDbSet<Assignment> Assignments { get; set; }
		IDbSet<Course> Courses { get; set; }
		IDbSet<ErrorReport> ErrorReports { get; set; }
		IDbSet<GroupMember> GroupMembers { get; set; }
		IDbSet<Message> Messages { get; set; }
		IDbSet<Milestone> Milestones { get; set; }
		IDbSet<Submission> Submissions { get; set; }
		IDbSet<TestCase> TestCases { get; set; }
		IDbSet<Testrun> Testruns { get; set; }
		IDbSet<UserCourseRelation> UserCourseRelations { get; set; }
		IDbSet<User> Users { get; set; }
		IDbSet<Role> Roles { get; set; }

		int SaveChanges();
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser> , IAppDataContext
    {

		public IDbSet<Assignment> Assignments { get; set; }
		public IDbSet<Course> Courses { get; set; }
		public IDbSet<ErrorReport> ErrorReports { get; set; }
		public IDbSet<GroupMember> GroupMembers { get; set; }
		public IDbSet<Message> Messages { get; set; }
		public IDbSet<Milestone> Milestones { get; set; }
		public IDbSet<Submission> Submissions { get; set; }
		public IDbSet<TestCase> TestCases { get; set; }
		public IDbSet<Testrun> Testruns { get; set; }
		public IDbSet<UserCourseRelation> UserCourseRelations { get; set; }
		public  IDbSet<User> Users { get; set; }
        public  IDbSet<Role> Roles { get; set; }
		

		public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext create()
        {
            return new ApplicationDbContext();
        }
    }
}