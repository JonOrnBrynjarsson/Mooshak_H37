using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc.Html;
using Microsoft.Ajax.Utilities;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Web;
using Microsoft.AspNet.Identity;
using Mooshak___H37.Controllers;
using System.Web.Mvc;
using System.Web;

namespace Mooshak___H37.Services
{

	public class AssigmentsService
	{ 
		private readonly IAppDataContext _db;
		private readonly MilestoneService _milestoneService;
		private readonly UsersService _usersService;

		public AssigmentsService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
			_milestoneService = new MilestoneService(null);
		}

		public DateTime today()
        {
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1);
            return endDateTime;
        }

        private int getCurrentUser()
        {
            return _usersService.getUserIdForCurrentApplicationUser();
        }

        /// <summary>
        /// Gets list of Assignments associated with Current user
        /// </summary>
        /// <returns>List of assignments</returns>
        public List<AssignmentViewModel> getAllAssignmentsForCurrUser()
        {
            var todayDate = today();

            var currUsId = _usersService.getUserIdForCurrentApplicationUser();

            var userCourses = (from uscr in _db.UserCourseRelations
                               where currUsId == uscr.UserID
							   && uscr.IsRemoved == false
                               select uscr.CourseID).ToList();


            var assignments = (from assi in _db.Assignments
                               where userCourses.Contains(assi.CourseID) &&
                                assi.IsRemoved != true &&
                                assi.DueDate > todayDate
                               orderby assi.DueDate descending
                               select assi).ToList();

            var viewModel = new List<AssignmentViewModel>();

            //Creates list of View models for assignment
            foreach (var assignm in assignments)
            {
                AssignmentViewModel model = new AssignmentViewModel
                {
                    ID = assignm.ID,
                    Name = assignm.Name,
                    SetDate = assignm.SetDate,
                    DueDate = assignm.DueDate,
                    CourseID = assignm.CourseID,
                    IsActive = assignm.IsActive,
                    IsRemoved = assignm.IsRemoved,
                    Description = assignm.Description,
                };
                viewModel.Add(model);
            }

            return viewModel;
        }

        /// <summary>
        /// Gets an assignment with given assignment ID. and finds all milestones, users
		/// and current users submissions to that assignment
        /// </summary>
        /// <returns>Returns assignmentViewModel with all the information needed</returns>
        public AssignmentViewModel getAssignmentById(int assignmId)
        {
            var assignment = (from asi in _db.Assignments
                              where asi.ID == assignmId &&
                              asi.IsRemoved != true
                              select asi).FirstOrDefault();

            if (assignment == null)
            {
                throw new Exception("The Assignment does not exist or has been removed");
            }

            //Current user ID
            var userId = getCurrentUser();

            //List of Milestones with given assignment ID
            var milestones = _db.Milestones.Where(x => x.AssignmentID == assignmId &&
                                                  x.IsRemoved != true)
                .Select(x => new MilestoneViewmodel
                {
                    ID = x.ID,
                    AllowedSubmissions = x.AllowedSubmissions,
                    Name = x.Name,
                    Description = x.Description,
                    AssignmentID = x.AssignmentID,
                    IsRemoved = x.IsRemoved,
                    Percentage = x.Percentage,
                    UserSubmissions = (from s in _db.Submissions
                                       where s.UserID == userId && s.MilestoneID == x.ID
                                       select s.ID).Count(),
                    TotalSubmissions = (from s in _db.Submissions
                                        where s.MilestoneID == x.ID
                                        select s.ID).Count()

                }).ToList();

            var coursename = (from asi in _db.Courses
                              where asi.ID == assignment.CourseID
                              select asi).SingleOrDefault();

            if (coursename == null)
            {
                throw new Exception("The Course for Assignment Does not exist or has been removed");
            }

            //Users associated with Course
            var userid = (from usr in _db.UserCourseRelations
                          where
                          assignment.CourseID == usr.CourseID
                          select usr.UserID).ToList();


            var users = (from user in _db.Users
                         where userid.Contains(user.ID)
                         select user.Name).Select(x => new UserViewModel
                         {
                             Name = x
                         }).ToList();



            //Creates new View Model with given properties
            AssignmentViewModel model = new AssignmentViewModel
            {
                ID = assignment.ID,
                Name = assignment.Name,
                SetDate = assignment.SetDate,
                DueDate = assignment.DueDate,
                CourseID = assignment.CourseID,
                IsActive = assignment.IsActive,
                IsRemoved = assignment.IsRemoved,
                Description = assignment.Description,
                Milestones = milestones,
                CourseName = coursename.Name,
                Users = users
            };

            return model;
        }

        /// <summary>
        /// Gets Courses and populates them with Assignments.
        /// </summary>
        /// <returns>list of Courses</returns>					HAS 0 REFERENCES (ENGINN AÐ NÝTA ÞETTA FALL?)
        //public List<CourseViewModel> GetCourseAssignments()
        //{
        //    var currUsId = _usersService.getUserIdForCurrentApplicationUser();

        //    //Gets List of Course IDs associated with Current User
        //    var userCourses = (from uscr in _db.UserCourseRelations
        //                       where currUsId == uscr.UserID &&
        //                       uscr.IsRemoved != true
        //                       select uscr.CourseID).ToList();


        //    //var assignments = (from assi in _db.Assignments
        //    //				   where userCourses.Contains(assi.CourseID)
        //    //				   && assi.IsRemoved != true
        //    //				   orderby assi.DueDate descending
        //    //				   select assi).ToList();


        //    //Gets List of Courses associated with Current User
        //    var courses = (from crse in _db.Courses
        //                   where userCourses.Contains(crse.ID) &&
        //                   crse.IsRemoved != true
        //                   select crse)

        //    .Select(x => new CourseViewModel
        //    {
        //        ID = x.ID,
        //        Name = x.Name,
        //        Isactive = x.Isactive,
        //        IsRemoved = x.IsRemoved,
        //        StartDate = x.Startdate,
        //        Assignments = getAssignmentsInCourse(x.ID),
        //    }).ToList();

        //    var viewModel = new List<CourseViewModel>();

        //    //Creates list of View Models for Course
        //    foreach (var course in courses)
        //    {
        //        CourseViewModel model = new CourseViewModel
        //        {
        //            Assignments = course.Assignments,
        //            ID = course.ID,
        //            Isactive = course.Isactive,
        //            IsRemoved = course.IsRemoved,
        //            Name = course.Name,
        //            StartDate = course.StartDate,
        //        };
        //        viewModel.Add(model);
        //    }

        //    return viewModel;

        //}

        /// <summary>
        /// Finds ID of Assignment that milestone is associated with
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <returns>Assignment ID</returns>
        public int getAssignmentIDFromMilestoneID(int milestoneId)
        {

            var assignmentId = (from mil in _db.Milestones
                                where mil.ID == milestoneId
                                select mil.AssignmentID).FirstOrDefault();

            return assignmentId;
        }

		/// <summary>
		/// Gets the number of submissions from a user to a specific milestone.
		/// </summary>
		/// <param name="milestoneId">The milestone "ID"</param>
		/// <param name="userId">The user "ID"</param>
		/// <returns>Number of submissions</returns>
		//public int getNumOfSubmissions(int milestoneId, int userId)
		//{
		//	return (from s in _db.Submissions
		//			where s.ID == userId && s.MilestoneID == milestoneId
		//			select s.ID).Count();
		//}


		/// <summary>
		/// Sets the assignment isRemoved to true in the database
		/// </summary>
		/// <param name="model"></param>
		/// <returns>Assignment ID</returns>
		internal void removeAssignment(AssignmentViewModel model)
        {
            var assignment = (from assign in _db.Assignments
                              where assign.ID == model.ID
                              && assign.IsRemoved == false
                              select assign).FirstOrDefault();

            if (assignment == null)
            {
                throw new Exception("The Assignment you want to remove does not exist or has been removed already");
            }

            assignment.IsRemoved = true;
            _db.SaveChanges();
			//TODO
			//REMOVE ALL MILESTONES THAT WERE CONNECTED TO THAT ASSIGNMENT HERE
			//MAKE NEW FUNCTION IN COURSESERVICE AND CALL IT ;D
        }

        /// <summary>
        /// Finds assignments in a Given Course, given they have not been marked Removed.
        /// </summary>
        /// <param name="assignmId"></param>
        /// <returns>List of Assignments</returns>
        public List<AssignmentViewModel> getAssignmentsInCourse(int assignmId)
        {

            //var todayDate = Today();

            //List of assignments with given Course ID.
            var assignments = (from asi in _db.Assignments
                               where asi.CourseID == assignmId &&
                               asi.IsRemoved != true
                               orderby asi.DueDate descending
                               select asi).ToList();

            if (assignments == null)
            {
                throw new Exception("No Assignments for given context");
            }

            List<AssignmentViewModel> viewModel = new List<AssignmentViewModel>();

            //Creates list of view models for assignments
            foreach (var assignm in assignments)
            {
                var test = getGradeFromSubmissions(assignm.ID, getCurrentUser());

                ////Gets list of milestones for associated Assignment
                //var milestones = (from mil in _db.Milestones
                //                  where mil.AssignmentID ==
                //                  assignm.ID
                //                  select mil.ID);

                ////Gets List of Submissions
                //var submissions = (from submit in _db.Submissions
                //                   where milestones.Contains(submit.MilestoneID) &&
                //                   submit.UserID == currUser
                //                   select submit).Count();

                //bool submitted = (submissions >= 1);

                bool submitted = userHasSubmissionForAssignment(assignm, getCurrentUser());

                double finalGrade = getGradeFromSubmissions(assignm.ID, getCurrentUser());

                AssignmentViewModel model = new AssignmentViewModel
                {
                    ID = assignm.ID,
                    Name = assignm.Name,
                    SetDate = assignm.SetDate,
                    DueDate = assignm.DueDate,
                    CourseID = assignm.CourseID,
                    IsActive = assignm.IsActive,
                    IsRemoved = assignm.IsRemoved,
                    Description = assignm.Description,
                    Submitted = submitted,
                    TotalGrade = finalGrade,
                };
                viewModel.Add(model);
            }

            return viewModel;
        }

        /// <summary>
        /// Takes in Assignment Entity Model and UserID, and finds if Given User Has a Submission Associated with Assignment.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userID"></param>
        /// <returns>True or False</returns>
        public bool userHasSubmissionForAssignment(Assignment entity, int userID)
        {
            //Gets list of milestones for associated Assignment
            var milestones = (from mil in _db.Milestones
                              where mil.AssignmentID ==
                              entity.ID
                              select mil.ID);

            //Gets List of Submissions
            var submissions = (from submit in _db.Submissions
                               where milestones.Contains(submit.MilestoneID) &&
                               submit.UserID == userID
                               select submit).Count();

            return (submissions >= 1);
        }

        /// <summary>
        /// First finds Milestones associate with assignment. Then It goes through the milestones and Finds...
        /// Newst associated Submissions for each milestone. It then goes through the information and 
        /// Calculates Total Grade for Given Assignment.
        /// </summary>
        /// <param name="AssignmentID"></param>
        /// <param name="userID"></param>
        /// <returns>Total Grade for Assignment</returns>
        private double getGradeFromSubmissions(int AssignmentID, int userID)
        {
            var milestones = (from mil in _db.Milestones
                              where mil.AssignmentID == AssignmentID &&
                              mil.IsRemoved != true
                              select mil).ToList();

            List<GradeViewModel> NewestSubmissions = new List<GradeViewModel>();


            foreach (var item in milestones)
            {
                GradeViewModel model = new GradeViewModel
                {
                    SubmissionData = (from newsub in _db.Submissions
                                      where item.ID == newsub.MilestoneID &&
                                      newsub.UserID == userID &&
                                      newsub.IsRemoved != true
                                      orderby newsub.DateSubmitted descending
                                      select newsub).FirstOrDefault(),
                    MilestoneData = item,
                };
                NewestSubmissions.Add(model);
            }

            double totalGrade = 0;

            foreach (var it in NewestSubmissions)
            {
                if (it.SubmissionData != null)
                {
                    var perc = it.MilestoneData.Percentage;
                    var grade = it.SubmissionData.Grade;
                    totalGrade += grade * (perc * 0.01);
                }
            }

            return totalGrade;
        }

        //Creates new assignment with given Assignment Model
        internal void CreateAssignment(AssignmentViewModel model)
        {
            _db.Assignments.Add(new Assignment
            {
                Name = model.Name,
                CourseID = model.CourseID,
                Description = model.Description,
                DueDate = model.DueDate,
                ID = model.ID,
                IsActive = model.IsActive,
                IsRemoved = model.IsRemoved,
                SetDate = model.SetDate,
            });
            _db.SaveChanges();
        }

        //Edits assignment with given Assignment Model
        internal void EditAssignment(AssignmentViewModel model)
        {
            var edit = (from assign in _db.Assignments
                        where assign.ID == model.ID
                        select assign).FirstOrDefault();

            //assignment that should be editied excists.
            if (edit != null)
            {
                edit.Name = model.Name;
                edit.SetDate = model.SetDate;
                edit.DueDate = model.DueDate;
                edit.Description = model.Description;
                edit.ID = model.ID;
                edit.IsActive = model.IsActive;
                edit.IsRemoved = model.IsRemoved;

                _db.SaveChanges();
            }
            else
            {
                throw new Exception("No assignment found to edit");
            }
        }

        //returns number of assignments in the system
        /// <summary>
        /// Finds all assignments in System
        /// </summary>
        /// <returns>Number of assignments</returns>
        public int NumberOfAssignments()
        {
            var assignments = (from assi in _db.Assignments
                               where assi.IsRemoved != true
                               select assi).Count();

            return assignments;
        }
    }
}
