using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;


namespace Mooshak___H37.Services
{

	public class AssigmentsService
	{ 
		private readonly IAppDataContext _db;
		private readonly MilestoneService _milestoneService;
		private readonly UsersService _usersService;
		private readonly CoursesService _coursesService;

		public AssigmentsService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
			_milestoneService = new MilestoneService(null);
			_coursesService = new CoursesService(null);
		}
		
		/// <summary>
		/// Returns the date to day
		/// </summary>
		/// <returns></returns>
		public DateTime today()
        {
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1);
            return endDateTime;
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
                               select assi).OrderBy(x => x.DueDate).ToList();

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
					Submitted = hasSubmittedAssignment(assignm.ID)
                };
                viewModel.Add(model);
            }

            return viewModel;
        }

		/// <summary>
		/// Checks is all milestones in an assignment have submissions to them
		/// </summary>
		/// <param name="assignmentId">The assignment being checked</param>
		/// <returns>True if all milestones have submissions</returns>
		public bool hasSubmittedAssignment(int assignmentId)
		{
			var milestones = (from m in _db.Milestones
				where m.AssignmentID == assignmentId
				      && m.IsRemoved == false
				select m.ID).ToList();
			if (milestones.Count > 0)
			{

				foreach (int milestone in milestones)
				{
					if (!hasSubmittedMilestone(milestone))
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if the current user has submitted a solution to a particular milestone
		/// </summary>
		/// <param name="milestoneId">The milestone being checked</param>
		/// <returns>True if mileste has submissions</returns>
		public bool hasSubmittedMilestone(int milestoneId)
		{
			int user = _usersService.getUserIdForCurrentApplicationUser();
			var ret = (from s in _db.Submissions
				where s.MilestoneID == milestoneId
				      && s.UserID == user
					  && s.IsRemoved == false
				select s).FirstOrDefault();
			if (ret != null)
			{
				return true;
			}
			return false;
		}

        /// <summary>
        /// Gets an assignment with given assignment ID. and finds all milestones, users
		/// and current users submissions to that assignment
        /// </summary>
        /// <returns>Returns assignmentViewModel with all the information needed</returns>
        public AssignmentViewModel getAssignmentById(int assignmentId)
        {
            var assignment = (from asi in _db.Assignments
                              where asi.ID == assignmentId &&
                              asi.IsRemoved != true
                              select asi).FirstOrDefault();

            if (assignment == null)
            {
                throw new Exception("The Assignment does not exist or has been removed");
            }

            //Current user ID
	        var userId = _usersService.getUserIdForCurrentApplicationUser();

            //List of Milestones with given assignment ID
            var milestones = _db.Milestones.Where(x => x.AssignmentID == assignmentId &&
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
									   select s.UserID).Count(),
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
                          where assignment.CourseID == usr.CourseID
						  && usr.RoleID == 1 //1 is Student
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

			_milestoneService.removeMilestoneByAssignmentId(model.ID);

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
	        int curruser = _usersService.getUserIdForCurrentApplicationUser();
            //Creates list of view models for assignments
            foreach (var assignm in assignments)
            {
				
                var test = getGradeFromSubmissions(assignm.ID, curruser);
				bool submitted = hasSubmittedAssignment(assignm.ID);
                double finalGrade = getGradeFromSubmissions(assignm.ID, curruser);

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
        /// First finds Milestones associate with assignment. Then It goes through the milestones and Finds...
        /// Newest associated Submissions for each milestone. It then goes through the information and 
        /// Calculates Total Grade for Given Assignment.
        /// </summary>
        /// <returns>Total Grade for Assignment</returns>
        private double getGradeFromSubmissions(int AssignmentId, int userId)
        {
            var milestones = (from mil in _db.Milestones
                              where mil.AssignmentID == AssignmentId &&
                              mil.IsRemoved != true
                              select mil).ToList();

            List<GradeViewModel> newestSubmissions = new List<GradeViewModel>();


            foreach (var item in milestones)
            {
                GradeViewModel model = new GradeViewModel
                {
                    SubmissionData = (from newsub in _db.Submissions
                                      where item.ID == newsub.MilestoneID &&
                                      newsub.UserID == userId &&
                                      newsub.IsRemoved == false
                                      orderby newsub.DateSubmitted descending
                                      select newsub).FirstOrDefault(),
                    MilestoneData = item,
                };
                newestSubmissions.Add(model);
            }

            double totalGrade = 0;

            foreach (var it in newestSubmissions)
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

        /// <summary>
		/// Create a new assignment in the Assignment table from the database
		/// with the information of the model param sent in.
		/// </summary>
        internal void createAssignment(AssignmentViewModel model)
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
        internal void editAssignment(AssignmentViewModel model)
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
        public int numberOfAssignments()
        {
            var assignments = (from assi in _db.Assignments
                               where assi.IsRemoved == false
                               select assi).Count();

            return assignments;
        }
    }
}
