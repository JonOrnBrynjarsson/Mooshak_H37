using System.Data.Entity;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models;

namespace Mooshak___H37.Tests
{
	class MockDataContext : IAppDataContext
	{
		/// <summary>
		/// Sets up the fake database.
		/// </summary>
		public MockDataContext()
		{
			// We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
			Assignments = new InMemoryDbSet<Assignment>();
			Courses = new InMemoryDbSet<Course>();
			ErrorReports = new InMemoryDbSet<ErrorReport>();
			GroupMembers = new InMemoryDbSet<GroupMember>();
			Messages = new InMemoryDbSet<Message>();
			Milestones = new InMemoryDbSet<Milestone>();
			Roles = new InMemoryDbSet<Role>();
			Submissions = new InMemoryDbSet<Submission>();
			TestCases = new InMemoryDbSet<TestCase>();
			Testruns = new InMemoryDbSet<Testrun>();
			Users = new InMemoryDbSet<User>();
			UserCourseRelations = new InMemoryDbSet<UserCourseRelation>();
		}

		public IDbSet<Assignment> Assignments { get; set; }
		public IDbSet<Course> Courses { get; set; }
		public IDbSet<ErrorReport> ErrorReports { get; set; }
		public IDbSet<GroupMember> GroupMembers { get; set; }
		public IDbSet<Message> Messages { get; set; }
		public IDbSet<Milestone> Milestones { get; set; }
		public IDbSet<Role> Roles { get; set; }
		public IDbSet<Submission> Submissions { get; set; }
		public IDbSet<TestCase> TestCases { get; set; }
		public IDbSet<Testrun> Testruns { get; set; }
		public IDbSet<User> Users { get; set; }
		public IDbSet<UserCourseRelation> UserCourseRelations { get; set; }
		
		// TODO: bætið við fleiri færslum hér
		// eftir því sem þeim fjölgar í AppDataContext klasanum ykkar!

		public int SaveChanges()
		{
			// Pretend that each entity gets a database id when we hit save.
			int changes = 0;

			return changes;
		}

		public void Dispose()
		{
			// Do nothing!
		}
	}
}
