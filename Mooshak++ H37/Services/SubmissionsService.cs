using Microsoft.AspNet.Identity;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mooshak___H37.Models.Entities;

namespace Mooshak___H37.Services
{

	public class SubmissionsService
	{
		private readonly ApplicationDbContext _db;
		private readonly UsersService _usersService;
		private readonly FilesService _filesService;
		
		public SubmissionsService()
		{
			_db = new ApplicationDbContext();
			_usersService = new UsersService();
			_filesService = new FilesService();
		}

		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}
        public List<SubmissionsViewModel> GetSubmissionsForMilestone(int milestoneID)
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
                    DateSubmitted = subs.DateSubmitted,
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
                //ProgramFileLocation = subs.ProgramFileLocation,
                DateSubmitted = subs.DateSubmitted,
                UserID = subs.UserID,
                UserName = (from name in _db.Users
                            where name.ID == subs.UserID
                            select name.Name).FirstOrDefault()
            };

            return model;
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

        public List<SubmissionsViewModel> GetSubmissionsForMilestoneForStudent(int milestoneID)
        {
            var currUser = GetCurrentUser();
            var submissions = (from subs in _db.Submissions
                               where subs.MilestoneID == milestoneID &&
                               subs.UserID == currUser
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
                    DateSubmitted = subs.DateSubmitted,
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


        public SubmissionsViewModel GetOneSubmission(int submissionID)
        {
            var submission = (from subs in _db.Submissions
                              where subs.ID == submissionID
                              select subs).FirstOrDefault();

            SubmissionsViewModel model = new SubmissionsViewModel
            {
                Grade = submission.Grade,
                ID = submission.ID,
                IsGraded = submission.IsGraded,
                IsRemoved = submission.IsRemoved,
                MilestoneID = submission.MilestoneID,
                Milestone = submission.Milestone,
                DateSubmitted = submission.DateSubmitted,
                ProgramFileLocation = submission.ProgramFileLocation,
                UserID = submission.UserID,
                UserName = (from name in _db.Users
                            where name.ID == submission.UserID
                            select name.Name).FirstOrDefault()
            };

            return model;

        }

        public int NumberOfSubmissions()
        {
            var submissions = (from subs in _db.Submissions
                              select subs).Count();

            return submissions;
        }     

		public SubmissionsViewModel getSubmissionDetail(int submissionId)
		{
			Submission submission = (from subs in _db.Submissions
							   where subs.ID == submissionId
							   && subs.IsRemoved == false
							   select subs).SingleOrDefault();

			SubmissionsViewModel model = new SubmissionsViewModel
			{
				ID = submission.ID,
				MilestoneID = submission.MilestoneID,
				UserID = submission.UserID,
				IsGraded = submission.IsGraded,
				Grade = submission.Grade,
				ProgramFileLocation = submission.ProgramFileLocation,
				DateSubmitted = submission.DateSubmitted
			};
			model.UserName = _usersService.GetSingleUser(model.UserID).Name;
			model.code = _filesService.getSubmissionFile(submissionId);
			model.Testruns = getSubmissionDetail(submissionId).Testruns;

			return model;

		}

		public int GetMilestoneIDFromSubmissionID(int submissionID)
		{
			var milestone = (from mil in _db.Submissions
							 where mil.ID == submissionID
							 select mil.MilestoneID).FirstOrDefault();

			return milestone;
		}
	}
}