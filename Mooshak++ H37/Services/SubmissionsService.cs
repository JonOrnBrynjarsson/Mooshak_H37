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
		private readonly IAppDataContext _db;
		private readonly UsersService _usersService;
		private readonly FilesService _filesService;
		
		public SubmissionsService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
			_filesService = new FilesService(null);
		}

		/// <summary>
		/// Finds LOGGED in user
		/// </summary>
		/// <returns>ID of Current User</returns>
		public int getCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserId = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserId;
		}
		/// <summary>
		/// Finds All submissions for Given Milestone ID
		/// </summary>
		/// <param name="milestoneId"></param>
		/// <returns>List of Submissions</returns>
        public List<SubmissionsViewModel> getSubmissionsForMilestone(int milestoneId)
        {
            var currUsId = getCurrentUser();

			//Finds alls submissions for given Milestone that have not been
			//Marked as removed.
            var submissions = (from subs in _db.Submissions
                               where subs.MilestoneID == milestoneId
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
								&& name.IsRemoved == false
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
		/// <param name="submissionId"></param>
		/// <returns>Submission for Given ID</returns>
        public SubmissionsViewModel getSubmission(int submissionId)
        {
			//Finds submission that has not been marked as removed
            var subs = (from subm in _db.Submissions
                        where subm.ID == submissionId
                        && subm.IsRemoved == false
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
							&& name.IsRemoved == false
                            select name.Name).FirstOrDefault(),
				FinalSolution = subs.FinalSolution,
            };

            return model;
        }

		//Grades Assignment with given model.
        internal void gradeAssignment(SubmissionsViewModel model)
        {
			//Finds submission that from given Model
            var currSubmission = (from subs in _db.Submissions
                                  where subs.ID == model.ID
								  && subs.IsRemoved == false
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
		/// <param name="milestoneId"></param>
		/// <returns>List of submissions</returns>
        public List<SubmissionsViewModel> getSubmissionsForMilestoneForStudent(int milestoneId)
        {
            var currUser = getCurrentUser();

			//Finds submissions for current user for given milestone
			//That have not been marked as removed.
			var submissions = (from subs in _db.Submissions
				where subs.MilestoneID == milestoneId &&
				      subs.UserID == currUser &&
				      subs.IsRemoved == false
				select subs).OrderByDescending(x => x.ID).ToList();

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
								&& name.IsRemoved == false
                                select name.Name).FirstOrDefault(),
					FinalSolution = subs.FinalSolution,
					NumOfTestruns = getNumOfTestruns(subs.ID),
					NumSuccessfulTestruns = getNumOfSuccessfulTestruns(subs.ID),
					Testruns = getListofTestruns(subs.ID)
				};
                viewModel.Add(model);

            }

            return viewModel;
        }
		/// <summary>
		/// Gets the number of testruns done on a particular submission.
		/// </summary>
		/// <param name="submissionId">The ID of the submission</param>
		/// <returns>Int representing the number of testruns</returns>
		public int getNumOfTestruns(int submissionId)
		{
			return (from t in _db.Testruns
					where t.SubmissionID == submissionId
					&& t.IsRemoved == false
					select t.ID).Count();
		}

		/// <summary>
		/// Gets the number of successful testruns done on a particular submission.
		/// </summary>
		/// <param name="submissionId">The ID of the submission</param>
		/// <returns>Int representing the number of successful testruns</returns>
		public int getNumOfSuccessfulTestruns(int submissionId)
		{
			return (from t in _db.Testruns
					where t.SubmissionID == submissionId
				    && t.IsSuccess == true 
					&& t.IsRemoved == false
					select t.ID).Count();
		}

		/// <summary>
		/// Gets a list of testruns done on a particular submission.
		/// </summary>
		/// <param name="submissionId">The ID of the submission</param>
		/// <returns>List of testruns</returns>
		public List<Testrun> getListofTestruns(int submissionId)
		{
			return (from t in _db.Testruns
					where t.SubmissionID == submissionId
					&& t.IsRemoved == false
					select t).ToList();
		}


		///// <summary>
		///// Returns Submission for given Submission ID
		///// </summary>
		///// <param name="submissionID"></param>
		///// <returns>Submission for given Submission ID</returns>
		//      public SubmissionsViewModel GetOneSubmission(int submissionID)
		//      {
		//	//Finds submission for given Submission ID
		//	//That has not been removed.
		//          var submission = (from subs in _db.Submissions
		//                            where subs.ID == submissionID
		//					  && subs.IsRemoved != true
		//                            select subs).FirstOrDefault();

		//	if (submission == null)
		//	{
		//		throw new Exception("Submission does not exist or has been removed.");
		//	}

		//	//Creates viewmodel for given submission
		//	SubmissionsViewModel model = new SubmissionsViewModel
		//          {
		//              Grade = submission.Grade,
		//              ID = submission.ID,
		//              IsGraded = submission.IsGraded,
		//              IsRemoved = submission.IsRemoved,
		//              MilestoneID = submission.MilestoneID,
		//              Milestone = submission.Milestone,
		//              DateSubmitted = submission.DateSubmitted,
		//              ProgramFileLocation = submission.ProgramFileLocation,
		//              UserID = submission.UserID,
		//              UserName = (from name in _db.Users
		//                          where name.ID == submission.UserID
		//                          select name.Name).FirstOrDefault(),
		//		FinalSolution = submission.FinalSolution,
		//	};

		//          return model;

		//      }




		/// <summary>
		/// Finds All submissions in system
		/// </summary>
		/// <returns>Number of all submissions</returns>
		public int numberOfSubmissions()
        {
            var submissions = (from subs in _db.Submissions
							   where subs.IsRemoved == false
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
			model.UserName = _usersService.getSingleUser(model.UserID).Name;
			//Finds Code associated with Submission
			model.code = _filesService.getSubmissionFile(submissionId);
			//Finds Testrun associated with Submission
			model.Testruns = getSubmissionDetail(submissionId).Testruns;

			return model;

		}

		/// <summary>
		/// Finds ID of milestone associated with Submission
		/// </summary>
		/// <param name="submissionId"></param>
		/// <returns>ID of Milestone</returns>
		public int getMilestoneIDFromSubmissionID(int submissionId)
		{
			var milestone = (from mil in _db.Submissions
							 where mil.ID == submissionId
							 && mil.IsRemoved == false
							 select mil.MilestoneID).FirstOrDefault();

			return milestone;
		}
	}
}