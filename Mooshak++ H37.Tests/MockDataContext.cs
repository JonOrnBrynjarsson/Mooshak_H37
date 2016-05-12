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
			Assignment = new InMemoryDbSet<Assignment>();
			Course = new InMemoryDbSet<Course>();
			ErrorReport = new InMemoryDbSet<ErrorReport>();
			GroupMember = new InMemoryDbSet<GroupMember>();
			Message = new InMemoryDbSet<Message>();
			Milestone = new InMemoryDbSet<Milestone>();
			Role = new InMemoryDbSet<Role>();
			Submission = new InMemoryDbSet<Submission>();
			TestCase = new InMemoryDbSet<TestCase>();
			Testrun = new InMemoryDbSet<Testrun>();
			User = new InMemoryDbSet<User>();
			UserCourseRelation = new InMemoryDbSet<UserCourseRelation>();
		}

		public IDbSet<Assignment> Assignment { get; set; }
		public IDbSet<Course> Course { get; set; }
		public IDbSet<ErrorReport> ErrorReport { get; set; }
		public IDbSet<GroupMember> GroupMember { get; set; }
		public IDbSet<Message> Message { get; set; }
		public IDbSet<Milestone> Milestone { get; set; }
		public IDbSet<Role> Role { get; set; }
		public IDbSet<Submission> Submission { get; set; }
		public IDbSet<TestCase> TestCase { get; set; }
		public IDbSet<Testrun> Testrun { get; set; }
		public IDbSet<User> User { get; set; }
		public IDbSet<UserCourseRelation> UserCourseRelation { get; set; }
		
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
