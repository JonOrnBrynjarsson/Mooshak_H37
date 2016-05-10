using Microsoft.AspNet.Identity;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Services
{
	public class SubmissionsService
	{
		private ApplicationDbContext _db;

		public SubmissionsService()
		{
			_db = new ApplicationDbContext();
		}

		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}

		public List<SubmissionsViewModel> GetSubmissionsForMilestone (int milestoneID)
		{
			var currUsId = GetCurrentUser();

			var submissions = (from subs in _db.Submissions
							   where subs.MilestoneID == milestoneID
							   && subs.IsRemoved != true
							   select subs).ToList();

			var viewModel = new List<SubmissionsViewModel>();

			foreach (var subs in submissions)
			{
				SubmissionsViewModel model = new SubmissionsViewModel
				{
					Grade = subs.Grade,
					ID = subs.ID,
					IsGraded = subs.IsGraded,
					IsRemoved = subs.IsRemoved,
					MilestoneID = subs.MilestoneID,
					Milestone = subs.Milestone,
					ProgramFileLocation = subs.ProgramFileLocation,
					UserID = subs.UserID,
					UserName = (from name in _db.Users
								where name.ID == subs.UserID
								select name.Name).FirstOrDefault()
			};
				viewModel.Add(model);
			}

			return viewModel;
		}

		public SubmissionsViewModel GetSubmission(int submissionID)
		{
			//var currUsId = GetCurrentUser();

			var subs = (from subm in _db.Submissions
							   where subm.ID == submissionID
							   && subm.IsRemoved != true
							   select subm).FirstOrDefault();

			SubmissionsViewModel model = new SubmissionsViewModel
			{
				Grade = subs.Grade,
				ID = subs.ID,
				IsGraded = subs.IsGraded,
				IsRemoved = subs.IsRemoved,
				MilestoneID = subs.MilestoneID,
				Milestone = subs.Milestone,
				ProgramFileLocation = subs.ProgramFileLocation,
				UserID = subs.UserID,
				UserName = (from name in _db.Users
							where name.ID == subs.UserID
							select name.Name).FirstOrDefault()
			};

			return model;
		}

		public int GetMilestoneIDFromSubmissionID(int submissionID)
		{
			var milestone = (from mil in _db.Submissions
							 where mil.ID == submissionID
							 select mil.MilestoneID).FirstOrDefault();

			return milestone;
		}

		internal void GradeAssignment(SubmissionsViewModel model)
		{
			var currSubmission = (from subs in _db.Submissions
						where subs.ID == model.ID
						select subs).FirstOrDefault();

			if (currSubmission != null)
			{
				currSubmission.Grade = model.Grade;
				currSubmission.IsGraded = true;

				_db.SaveChanges();
			}
			else
			{
				// DO Something!!
			}
		}
	}
}