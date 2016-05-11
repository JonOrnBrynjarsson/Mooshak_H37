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

		/// <summary>
		/// Finds LOGGED in user
		/// </summary>
		/// <returns>ID of Current User</returns>
		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}
		/// <summary>
		/// Finds All submissions for Given Milestone ID
		/// </summary>
		/// <param name="milestoneID"></param>
		/// <returns>List of Submissions</returns>
        public List<SubmissionsViewModel> GetSubmissionsForMilestone(int milestoneID)
        {
            var currUsId = GetCurrentUser();

			//Finds alls submissions for given Milestone that have not been
			//Marked as removed.
            var submissions = (from subs in _db.Submissions
                               where subs.MilestoneID == milestoneID
                               && subs.IsRemoved != true
                               select subs).ToList();

			if (submissions == null)
			{
				//no submissions exist
			}

            var viewModel = new List<SubmissionsViewModel>();

			//Creates List of View models for given Submissions
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
                                select name.Name).FirstOrDefault(),
					FinalSolution = subs.FinalSolution,
                };
                viewModel.Add(model);
            }

            return viewModel;
        }

		/// <summary>
		/// Finds Submission for Given Submissions ID
		/// </summary>
		/// <param name="submissionID"></param>
		/// <returns>Submission for Given ID</returns>
        public SubmissionsViewModel GetSubmission(int submissionID)
        {
			//Finds submission that has not been marked as removed
            var subs = (from subm in _db.Submissions
                        where subm.ID == submissionID
                        && subm.IsRemoved != true
                        select subm).FirstOrDefault();

			if (subs == null)
			{
				throw new Exception("The submission has been removed or does not exist.");
			}

			//Creates model for given Submission
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
                            select name.Name).FirstOrDefault(),
				FinalSolution = subs.FinalSolution,
            };

            return model;
        }

		//Grades Assignment with given model.
        internal void GradeAssignment(SubmissionsViewModel model)
        {
			//Finds submission that from given Model
            var currSubmission = (from subs in _db.Submissions
                                  where subs.ID == model.ID
                                  select subs).FirstOrDefault();

			//Submission Exists.
            if (currSubmission != null)
            {
                currSubmission.Grade = model.Grade;
                currSubmission.IsGraded = true;

                _db.SaveChanges();
            }
            else
            {
				throw new Exception("The submission that is being graded does not exist or has been removed.");
			}
		}

		/// <summary>
		/// Finds Submissions for given Milestone Associated
		/// With Current Logged in User.
		/// </summary>
		/// <param name="milestoneID"></param>
		/// <returns>List of submissions</returns>
        public List<SubmissionsViewModel> GetSubmissionsForMilestoneForStudent(int milestoneID)
        {
            var currUser = GetCurrentUser();

			//Finds submissions for current user for given milestone
			//That have not been marked as removed.
            var submissions = (from subs in _db.Submissions
                               where subs.MilestoneID == milestoneID &&
                               subs.UserID == currUser &&
							   subs.IsRemoved != true
                               select subs).ToList();

			if (submissions == null)
			{
				throw new Exception("Given submission does not exist or has been removed.");
			}

			var viewModel = new List<SubmissionsViewModel>();


			//Creates list of submissions for given model. 
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
                                select name.Name).FirstOrDefault(),
					FinalSolution = subs.FinalSolution,
				};
                viewModel.Add(model);

            }

            return viewModel;
        }

		/// <summary>
		/// Returns Submission for given Submission ID
		/// </summary>
		/// <param name="submissionID"></param>
		/// <returns>Submission for given Submission ID</returns>
        public SubmissionsViewModel GetOneSubmission(int submissionID)
        {
			//Finds submission for given Submission ID
			//That has not been removed.
            var submission = (from subs in _db.Submissions
                              where subs.ID == submissionID
							  && subs.IsRemoved != true
                              select subs).FirstOrDefault();

			if (submission == null)
			{
				throw new Exception("Submission does not exist or has been removed.");
			}

			//Creates viewmodel for given submission
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
                            select name.Name).FirstOrDefault(),
				FinalSolution = submission.FinalSolution,
			};

            return model;

        }

		/// <summary>
		/// Finds All submissions in system
		/// </summary>
		/// <returns>Number of all submissions</returns>
        public int NumberOfSubmissions()
        {
            var submissions = (from subs in _db.Submissions
                              select subs).Count();

            return submissions;
        }

		/// <summary>
		/// Finds Submission for given Submission ID,
		/// Along with Username, Code and Testruns
		/// </summary>
		/// <param name="submissionId"></param>
		/// <returns>Submission for given ID</returns>
		public SubmissionsViewModel getSubmissionDetail(int submissionId)
		{
			//Finds submissions for given submission ID
			Submission submission = (from subs in _db.Submissions
							   where subs.ID == submissionId
							   && subs.IsRemoved == false
							   select subs).SingleOrDefault();

			//Creates View model for given Submission
			SubmissionsViewModel model = new SubmissionsViewModel
			{
				ID = submission.ID,
				MilestoneID = submission.MilestoneID,
				UserID = submission.UserID,
				IsGraded = submission.IsGraded,
				Grade = submission.Grade,
				ProgramFileLocation = submission.ProgramFileLocation,
				DateSubmitted = submission.DateSubmitted,
				FinalSolution = submission.FinalSolution,
			};
			//Finds Username associated with Submission
			model.UserName = _usersService.GetSingleUser(model.UserID).Name;
			//Finds Code associated with Submission
			model.code = _filesService.getSubmissionFile(submissionId);
			//Finds Testrun associated with Submission
			model.Testruns = getSubmissionDetail(submissionId).Testruns;

			return model;

		}

		/// <summary>
		/// Finds ID of milestone associated with Submission
		/// </summary>
		/// <param name="submissionID"></param>
		/// <returns>ID of Milestone</returns>
		public int GetMilestoneIDFromSubmissionID(int submissionID)
		{
			var milestone = (from mil in _db.Submissions
							 where mil.ID == submissionID
							 select mil.MilestoneID).FirstOrDefault();

			return milestone;
		}
	}
}