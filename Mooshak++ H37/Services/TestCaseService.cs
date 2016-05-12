using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Services
{
	public class TestCaseService
	{
		private readonly IAppDataContext _db;

		public TestCaseService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
		}
		//Creates a new Testcase with Given Model and Milestone ID
		internal void createTestCase(TestCaseViewModel model, int milestoneId)
		{
			_db.TestCases.Add(new TestCase
			{
				ID = model.ID,
				Inputstring = model.Inputstring,
				MilestoneID = milestoneId,
			});
			_db.SaveChanges();
		}

		//Removes Testcase with associated with given model
		internal void removeTestCase(int testcaseId)
		{
			//Finds test Case Associated with Given model
			var testcase = (from test in _db.TestCases
							 where test.ID == testcaseId
							 && test.IsRemoved == false
							 select test).SingleOrDefault();

			if (testcase == null)
			{
				throw new Exception("Testcase cannot be removed because it does not exist.");
			}

			testcase.IsRemoved = true;
			_db.SaveChanges();
		}

		/// <summary>
		/// Finds Testcases associated with given Milestone
		/// </summary>
		/// <param name="milestoneId"></param>
		/// <returns>List of Testcasesviewmodel</returns>
		public List<TestCaseViewModel> getTestCasesVMForMilestone(int milestoneId)
		{
			List < TestCase >  testcases = GetTestCasesForMilstone(milestoneId);
			var viewModel = new List<TestCaseViewModel>();
			if (testcases != null)
			{
				foreach (var test in testcases)
				{
					TestCaseViewModel model = new TestCaseViewModel
					{
						ID = test.ID,
						Inputstring = test.Inputstring,
						Outputstring = test.Outputstring,
						MilestoneID = test.MilestoneID,
					};
					viewModel.Add(model);
				}
			}

			return viewModel;
		}

		/// <summary>
		/// Get a list of testcases for a particular milestone
		/// </summary>
		/// <param name="milestoneId">The ID of the milestone</param>
		/// <returns>A list of testcases</returns>
		public List<TestCase> GetTestCasesForMilstone(int milestoneId)
		{
			return (from test in _db.TestCases
					orderby test.ID
					where test.MilestoneID == milestoneId
					&& test.IsRemoved != true
					select test).ToList();
		}



		/// <summary>
		/// Finds Test Case associated with given Test Case ID
		/// </summary>
		/// <param name="testCaseId"></param>
		/// <returns>Test Case for given ID</returns>
		public TestCaseViewModel getSingleTestCase(int testCaseId)
		{
			//Finds Test Case for given Test Case ID
			var testcase = (from test in _db.TestCases
							where test.ID == testCaseId &&
							test.IsRemoved != true
							select test).FirstOrDefault();

			if (testcase == null)
			{
				throw new Exception("The testcase does not exist or has been removed.");
			}

			//Creates model for given Test Case
			TestCaseViewModel model = new TestCaseViewModel
			{
				ID = testcase.ID,
				Inputstring = testcase.Inputstring,
				MilestoneID = testcase.MilestoneID,
			};

			return model;
		}

		/// <summary>
		/// Finds All Test cases in system
		/// </summary>
		/// <returns>Number of Test Cases</returns>
        public int numberOfTestCases()
        {
            var testCases = (from tc in _db.TestCases
							 where tc.IsRemoved == false
							 select tc).Count();

            return testCases;
        }
    }
}