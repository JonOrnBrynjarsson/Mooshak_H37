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
	}
}