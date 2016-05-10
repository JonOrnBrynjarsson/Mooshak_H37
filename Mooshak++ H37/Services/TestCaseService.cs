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
		private ApplicationDbContext _db;

		public TestCaseService()
		{
			_db = new ApplicationDbContext();
		}
		internal void CreateTestCase(TestCaseViewModel model, int milestoneID)
		{
			_db.TestCases.Add(new TestCase
			{
				ID = model.ID,
				Inputstring = model.Inputstring,
				MilestoneID = milestoneID,
			});
			_db.SaveChanges();
		}

		internal void RemoveTestCase(TestCaseViewModel model)
		{
			var testcase = (from test in _db.TestCases
							 where test.ID == model.ID
							 select test).FirstOrDefault();

			if (testcase == null)
			{
				//DO SOMEHTING
			}

			testcase.IsRemoved = true;
			_db.SaveChanges();
		}

		public List<TestCaseViewModel> GetTestCasesForMilestone(int milID)
		{
			var testcases = (from test in _db.TestCases
							 orderby test.ID
							 where test.MilestoneID == milID
							 && test.IsRemoved != true
							 select test).ToList();

			var viewModel = new List<TestCaseViewModel>();

			if (testcases.Count == 0)
			{
				//foreach (var test in testcases)
				//{
				//	TestCaseViewModel model = new TestCaseViewModel
				//	{
				//		ID = test.ID,
				//		Inputstring = test.Inputstring,
				//		MilestoneID = test.MilestoneID,
				//	};
				//	viewModel.Add(model);
				//}

				return viewModel;
			}
			else
			{
				foreach (var test in testcases)
				{
					TestCaseViewModel model = new TestCaseViewModel
					{
						ID = test.ID,
						Inputstring = test.Inputstring,
						MilestoneID = test.MilestoneID,
					};
					viewModel.Add(model);
				}
			}

			return viewModel;
		}

		public TestCaseViewModel GetSingleTestCase(int testCaseID)
		{
			var testcase = (from test in _db.TestCases
							  where test.ID == testCaseID
							select test).FirstOrDefault();

			if (testcase == null)
			{
				//DO SOMETHING
				//throw new exception / skila NULL(ekki skila null hér)
			}

			TestCaseViewModel model = new TestCaseViewModel
			{
				ID = testcase.ID,
				Inputstring = testcase.Inputstring,
				MilestoneID = testcase.MilestoneID,
			};

			return model;
		}

        public int NumberOfTestCases()
        {
            var testCases = (from tc in _db.TestCases
                              select tc).Count();

            return testCases;
        }
    }
}