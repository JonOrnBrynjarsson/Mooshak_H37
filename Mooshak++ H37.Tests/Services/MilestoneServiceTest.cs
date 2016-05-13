using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	[TestClass]
	public class MilestoneServiceTest
	{
		private MilestoneService _milestoneService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();
			
			#region Mock Milestones
			
			var m1 = new Milestone()
			{
				ID = 1,
				Name = "Skrifa Hello World!",
				AllowedSubmissions = 3,
				AssignmentID = 1,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = false,
				Percentage = 45.0
			};
			mockDb.Milestones.Add(m1);

			
			var m2 = new Milestone()
			{
				ID = 2,
				Name = "Leggja saman tvær tölur",
				AllowedSubmissions = 2,
				AssignmentID = 1,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = false,
				Percentage = 55.0
			};
			mockDb.Milestones.Add(m2);

			var m3 = new Milestone()
			{
				ID = 3,
				Name = "Skrifa Hello Test!",
				AllowedSubmissions = 1,
				AssignmentID = 2,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = false,
				Percentage = 33.3
			};
			mockDb.Milestones.Add(m3);

			var m4 = new Milestone()
			{
				ID = 4,
				Name = "Skrifa út fabinici talnarunu",
				AllowedSubmissions = 3,
				AssignmentID = 2,
				Description = "Skrifa út fyrstu 50 fabinici talnarununa",
				IsRemoved = false,
				Percentage = 14.0
			};
			mockDb.Milestones.Add(m4);

			var m5 = new Milestone()
			{
				ID = 5,
				Name = "margfalda aðeins með + virkjanum",
				AllowedSubmissions = 3,
				AssignmentID = 2,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = true,
				Percentage = 53.0
			};
			mockDb.Milestones.Add(m5);

			var m6 = new Milestone()
			{
				ID = 6,
				Name = "Skrifa út veldi prímtalna",
				AllowedSubmissions = 3,
				AssignmentID = 3,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = false,
				Percentage = 50.0
			};
			mockDb.Milestones.Add(m6);

			var m7 = new Milestone()
			{
				ID = 7,
				Name = "Skrifa Hello World!",
				AllowedSubmissions = 3,
				AssignmentID = 3,
				Description = "Hér á að skila útgáfu af... ",
				IsRemoved = true,
				Percentage = 50.0
			};
			mockDb.Milestones.Add(m7);

			#endregion

			_milestoneService = new MilestoneService(mockDb);
		}

		[TestMethod]
		public void getMilestonesForAssignment()
		{
			// Arrange:
			const int num1 = 2;
			const int num2 = 2;
			const int num3 = 1;

			// Act:
			var result1 = _milestoneService.getMilestonesForAssignment(1);
			var result2 = _milestoneService.getMilestonesForAssignment(2);
			var result3 = _milestoneService.getMilestonesForAssignment(3);

			// Assert: 
			Assert.AreEqual(num1, result1.Count);
			Assert.AreEqual(num2, result2.Count);
			Assert.AreEqual(num3, result3.Count);
		}

		[TestMethod]
		public void getSingleMilestone()
		{
			// Arrange:
			const int num1 = 2;
			
			// Act:
			var result1 = _milestoneService.getSingleMilestone(num1);
			
			// Assert: 
			Assert.AreEqual(result1.ID, num1);
	
		}

		[TestMethod]
		public void numberOfMilestones()
		{
			// Arrange:
			const int num = 5;
			// Act:
			var result = _milestoneService.numberOfMilestones();
			
			// Assert: 
			Assert.AreEqual(num, result);
		}



	}
}
