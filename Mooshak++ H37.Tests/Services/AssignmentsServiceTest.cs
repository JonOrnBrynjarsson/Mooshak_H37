using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	[TestClass]
	public class AssignmentsServiceTest
	{
		private AssigmentsService _assigmentsService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();

			var a1 = new Assignment()
			{
				ID = 1,
				Name = "Verkefni 1",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 1,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a1);

			var a2 = new Assignment()
			{
				ID = 2,
				Name = "Verkefni 2",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 1,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax2"
			};
			mockDb.Assignments.Add(a2);

			var a3 = new Assignment()
			{
				ID = 3,
				Name = "Verkefni 3",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 2,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a3);

			var a4 = new Assignment()
			{
				ID = 4,
				Name = "Verkefni 4",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 1,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a4);

			var a5 = new Assignment()
			{
				ID = 5,
				Name = "Verkefni 5",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 2,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a5);

			var a6 = new Assignment()
			{
				ID = 6,
				Name = "Verkefni 6",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 2,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a6);

			var a7 = new Assignment()
			{
				ID = 7,
				Name = "Verkefni 7",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 2,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a7);

			var a8 = new Assignment()
			{
				ID = 8,
				Name = "Verkefni 8",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 2,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a8);

			var a9 = new Assignment()
			{
				ID = 9,
				Name = "Verkefni 9",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 3,
				IsActive = false,
				IsRemoved = true,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a9);

			var a10 = new Assignment()
			{
				ID = 10,
				Name = "Verkefni 10",
				SetDate = DateTime.Today,
				DueDate = DateTime.Today,
				CourseID = 3,
				IsActive = true,
				IsRemoved = false,
				Description = "Vinna strax"
			};
			mockDb.Assignments.Add(a10);

			// Note: you only have to add data necessary for this
			// particular service (FriendService) to run properly.
			// There will be more tables in your DB, but you only
			// need to provide the data for the methods you are
			// actually testing here.

			_assigmentsService = new AssigmentsService(mockDb);
		}

		[TestMethod]
		public void removeCourseById()
		{
			// Arrange:
			//const int a1 = 6;
			const int a2 = 10;

			// Act:
			//var result1 = _assigmentsService.getAssignmentById(6);
			var result2 = _assigmentsService.getAssignmentById(10);
			
			// Assert: 
			//Assert.AreEqual(a1, result1);
			Assert.AreEqual(a2, result2);
		}

		[TestMethod]
		public void numberOfAssignments()
		{
			// Arrange:
			const int num = 9;

			// Act:
			var result = _assigmentsService.numberOfAssignments();
			
			// Assert: 
			Assert.AreEqual(num, result);
		}


	}
}
