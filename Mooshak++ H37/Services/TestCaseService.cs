using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;

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
		public void createTestCase(TestCaseViewModel model, int milestoneId)
		{
			_db.TestCases.Add(new TestCase
			{
				ID = model.ID,
				Inputstring = model.Inputstring,
				Outputstring = model.Outputstring,
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

		/// <summary>
		/// Returns the "ID" of the test cases related to the submission ID sent in.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested.</param>
		/// <returns>A list of testcase IDs</returns>
		public List<int> getTestCases(int submissionId)
		{
			var testCases = (from t in _db.TestCases
							 join m in _db.Milestones
								 on t.MilestoneID equals m.ID
							 join s in _db.Submissions
								 on m.ID equals s.MilestoneID
							 where s.ID == submissionId
								   && t.IsRemoved == false
							 select t.ID);

			return testCases.ToList();
		}


		/// <summary>
		/// Gets the string with the input for a specific testrun.
		/// </summary>
		/// <param name="testCaseId">The "ID" of the testcase</param>
		/// <returns>The input string for a testrun</returns>
		public string getATestCaseInput(int testCaseId)
		{
			return (from t in _db.TestCases
					where t.ID == testCaseId
						&& t.IsRemoved == false
					select t.Inputstring).SingleOrDefault();
		}

		/// <summary>
		/// Gets the string with the expected output for a specific testrun.
		/// </summary>
		/// <param name="testCaseId">The "ID" of the testcase</param>
		/// <returns>A string with the expected output of the testcase</returns>
		public string getATestCaseOutput(int testCaseId)
		{
			return (from t in _db.TestCases
					where t.ID == testCaseId
						&& t.IsRemoved == false
					select t.Outputstring).SingleOrDefault();
		}

	}
}