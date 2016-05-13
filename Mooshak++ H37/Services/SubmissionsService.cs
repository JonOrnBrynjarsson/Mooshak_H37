using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models.Entities;

namespace Mooshak___H37.Services
{

	public class SubmissionsService
	{
		private readonly IAppDataContext _db;
		private readonly UsersService _usersService;
		private readonly FilesService _filesService;
		private readonly CoursesService _coursesService;
		
		public SubmissionsService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
			_filesService = new FilesService(null);
			_coursesService = new CoursesService(null);
		}
		

		/// <summary>
		/// Finds All submissions for Given Milestone ID that have not been marke as
		/// removed.
		/// </summary>
		/// <param name="milestoneId"></param>
		/// <returns>List of Submissions</returns>
        public List<SubmissionsViewModel> getSubmissionsForMilestone(int milestoneId)
		{
			var users = _coursesService.getUsersInCourse(_coursesService.getCourseIdFromMilestoneId(milestoneId));

			List<Submission> submissions = new List<Submission>();
			foreach (var user in users)
			{
				var submission = (from sub in _db.Submissions
					where sub.MilestoneID == milestoneId
					      && sub.IsRemoved != true
						  && sub.UserID == user.ID
					      && sub.FinalSolution == true
					select sub).SingleOrDefault();
				if (submission == null)
				{
					submission = (from sub in _db.Submissions
							   where sub.MilestoneID == milestoneId
									 && sub.IsRemoved != true
									 && sub.UserID == user.ID
							   select sub)
							   .OrderByDescending(x => x.DateSubmitted)
							   .FirstOrDefault();
				}
				if (submission != null)
				{
					submissions.Add(submission);
				}

			}

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
						orderby subm.DateSubmitted descending
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


		/// <summary>
		/// Grades Assignment with given model.
		/// </summary>
		/// <param name="model"></param>
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
            var currUser = _usersService.getUserIdForCurrentApplicationUser();

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
			model.UserName = _usersService.getSingleUserInfo(model.UserID).Name;
			//Finds Code associated with Submission
			model.code = _filesService.getSubmissionFile(submissionId);
			//Finds Testrun associated with Submission
			model.Testruns = getSubmissionDetail(submissionId).Testruns;

			return model;

		}


		/// <summary>
		/// Gets the test run outcome for a submission
		/// with the specific ID
		/// </summary>
		/// <param name="submissionId"></param>
		/// <returns></returns>
		public List<Testrun> getTestrunsOutcomeForSubmission(int submissionId)
		{
			List<Testrun> tRuns = (from t in _db.Testruns
								   where t.SubmissionID == submissionId
								   select t).ToList();
			return tRuns;

		}

		/// <summary>
		/// Saves a submission to the database.
		/// </summary>
		/// <param name="milestonedId">The "ID" of the milestone that is being worked on</param>
		/// <returns>The "ID" of the submission</returns>
		public int createSubmission(int milestonedId)
		{
			if (milestonedId > 0)
			{
					Submission submission = new Submission
					{
						MilestoneID = milestonedId,
						UserID = _usersService.getUserIdForCurrentApplicationUser(),
						ProgramFileLocation = "a", //Required parameter -> set when db is updated.
						IsGraded = false,
						FinalSolution = false,
						DateSubmitted = DateTime.Now

					};
					_db.Submissions.Add(submission);
					_db.SaveChanges();

					submission.ProgramFileLocation = _filesService.getStudentSubmissionFolder(submission.ID);
					_db.SaveChanges();
					return submission.ID;
				}
				else
				{
					throw new Exception("The Milestone you are trying to submit to does not exist or has been removed.");
				}

		}
			
	}
}