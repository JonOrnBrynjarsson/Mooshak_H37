using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	[TestClass]
	public class CoursesServicesTest
	{
		private CoursesService _coursesService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();

			#region Mock Courses

			var c1 = new Course()
			{
				ID = 1,
				Name = "Forritun",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c1);

			var c2 = new Course()
			{
				ID = 2,
				Name = "Gagnaskipan",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c2);
			var c3 = new Course()
			{
				ID = 3,
				Name = "Vefforitun",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c3);
			var c4 = new Course()
			{
				ID = 4,
				Name = "Reiknirit",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c4);

			var c5 = new Course()
			{
				ID = 5,
				Name = "Strjál stærðfræði 1",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c5);
			var c6 = new Course()
			{
				ID = 6,
				Name = "Tölvuhögun",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = false
			};
			mockDb.Courses.Add(c6);
			var c7 = new Course()
			{
				ID = 7,
				Name = "Eðlisfræði",
				Startdate = DateTime.Today,
				Isactive = true,
				IsRemoved = true
			};
			mockDb.Courses.Add(c7);
			var c8 = new Course()
			{
				ID = 8,
				Name = "Kerfishönnun",
				Startdate = DateTime.Today,
				Isactive = false,
				IsRemoved = false
			};
			mockDb.Courses.Add(c8);

			var cc1 = new UserCourseRelation()
			{
				ID = 1,
				CourseID = 1,
				UserID = 1,
				RoleID = 1,
				IsRemoved = false
			};
			mockDb.UserCourseRelations.Add(cc1);
			var cc2 = new UserCourseRelation()
			{
				ID = 2,
				CourseID = 8,
				UserID = 1,
				RoleID = 1,
				IsRemoved = false
			};
			mockDb.UserCourseRelations.Add(cc2);
		
			#endregion

			// Note: you only have to add data necessary for this
			// particular service (FriendService) to run properly.
			// There will be more tables in your DB, but you only
			// need to provide the data for the methods you are
			// actually testing here.

			_coursesService = new CoursesService(mockDb);
		}

		[TestMethod]
		public void numberOfCourses()
		{
			// Arrange:
			const int num = 7;
			// Act:
			int result = _coursesService.numberOfCourses();

			// Assert: 
			Assert.AreEqual(num, result);
		}

		[TestMethod]
		public void removeCourseById()
		{
			// Arrange:
			const int num = 6;
			// Act:
			_coursesService.removeCourseById(8);
			int result = _coursesService.numberOfCourses();

			// Assert: 
			Assert.AreEqual(num, result);
		}

	}
}
