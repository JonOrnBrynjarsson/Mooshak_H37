﻿using Mooshak___H37.Models;
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
		private readonly ApplicationDbContext _db;

		public TestCaseService()
		{
			_db = new ApplicationDbContext();
		}
		//Creates a new Testcase with Given Model and Milestone ID
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

		//Removes Testcase with associated with given model
		internal void RemoveTestCase(TestCaseViewModel model)
		{
			//Finds test Case Associated with Given model
			var testcase = (from test in _db.TestCases
							 where test.ID == model.ID
							 select test).FirstOrDefault();

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
		/// <param name="milID"></param>
		/// <returns>List of Test cases</returns>
		public List<TestCaseViewModel> GetTestCasesForMilestone(int milID)
		{
			//Finds test cases for given Milestone, that have not been removed
			var testcases = (from test in _db.TestCases
							 orderby test.ID
							 where test.MilestoneID == milID
							 && test.IsRemoved != true
							 select test).ToList();

			var viewModel = new List<TestCaseViewModel>();
			//Milestone has Test Cases
			if (testcases != null)
			{
				//Creates List of Test Case Models for Given Milestone
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

		/// <summary>
		/// Finds Test Case associated with given Test Case ID
		/// </summary>
		/// <param name="testCaseID"></param>
		/// <returns>Test Case for given ID</returns>
		public TestCaseViewModel GetSingleTestCase(int testCaseID)
		{
			//Finds Test Case for given Test Case ID
			var testcase = (from test in _db.TestCases
							  where test.ID == testCaseID &&
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
        public int NumberOfTestCases()
        {
            var testCases = (from tc in _db.TestCases
                              select tc).Count();

            return testCases;
        }
    }
}