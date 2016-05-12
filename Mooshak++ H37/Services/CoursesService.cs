using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Services
{
	public class CoursesService
	{

		private readonly IAppDataContext _db;
		private readonly AssigmentsService _assignmentsService;
		private readonly UsersService _userService;

		public CoursesService(IAppDataContext dbContext)
		{

			_db = dbContext ?? new ApplicationDbContext();
			_assignmentsService = new AssigmentsService(null);
			_userService = new UsersService(null);
		}


		/// <summary>
		/// Gets a list of all Courses from the database that are not
		/// marked as Removed.
		/// </summary>
		/// <returns>list of all Courses</returns>
		public List<CourseViewModel> getAllCourses()
        {
            var courses = (from x in _db.Courses
						   where x.IsRemoved == false
						   orderby x.Name ascending
                           select x).ToList();

            var viewModel = new List<CourseViewModel>();

            foreach (var course in courses)
            {
                CourseViewModel model = new CourseViewModel
                {
                    Name = course.Name,
                    StartDate = course.Startdate,
                    ID = course.ID,
                    Isactive = course.Isactive,
					User = _userService.getUsersInCourse(course.ID)
				};
                viewModel.Add(model);
            }
			

            return viewModel;
        }

		/// <summary>
		/// Finds all Courses Associated with given User
		/// </summary>
		/// <returns>List of Courses</returns>
        public List<CourseViewModel> getAllCoursesByUserID(int UserId)
        {
            List<Course> courses = null;

            var coursesID = (from x in _db.UserCourseRelations
                             where x.UserID == UserId &&
							 x.IsRemoved == false
                             select x.CourseID);

            foreach (var id in coursesID)
            {
                courses.Add(getCourseById(id));
            }

            var viewModel = new List<CourseViewModel>();

	        if (courses != null)
		        foreach (var course in courses)
		        {
			        CourseViewModel model = new CourseViewModel
			        {
				        Name = course.Name,
				        StartDate = course.Startdate,
				        ID = course.ID,
				        Isactive = course.Isactive,
			        };
			        viewModel.Add(model);
		        }

	        return viewModel;

        }

		/// <summary>
		/// Finds all Courses that LOGGED IN User is Associated with.
		/// </summary>
		/// <returns>List of Courses</returns>
		public List<CourseViewModel> getCoursesForUser()
		{
			var courses = getCurrentCourseList();

			var viewModel = new List<CourseViewModel>();

			//Creates a View Model for Given Courses.
			foreach (var course in courses)
			{
                if (_assignmentsService.getAssignmentsInCourse(course.ID).Count != 0)
                {
                    CourseViewModel model = new CourseViewModel
                    {

                        ID = course.ID,
                        Assignments = _assignmentsService.getAssignmentsInCourse(course.ID),
                        Isactive = course.Isactive,
                        IsRemoved = course.IsRemoved,
                        Name = course.Name,
                        StartDate = course.Startdate,
                    };
                    viewModel.Add(model);
                }
			}

			return viewModel;
		}

		/// <summary>
		/// Finds all Courses that LOGGED IN User is Associated with.
		/// </summary>
		/// <returns>List of Courses</returns>
		public List<CourseViewModel> getCoursesForUserWithEmptyCourses()
		{
			var courses = getCurrentCourseList();

			var viewModel = new List<CourseViewModel>();

			//Creates a View Model for Given Courses.
			foreach (var course in courses)
			{
				{
					CourseViewModel model = new CourseViewModel
					{

						ID = course.ID,
						Assignments = _assignmentsService.getAssignmentsInCourse(course.ID),
						Isactive = course.Isactive,
						IsRemoved = course.IsRemoved,
						Name = course.Name,
						StartDate = course.Startdate,
					};
					viewModel.Add(model);
				}
			}

			return viewModel;
		}

		/// <summary>
		/// Finds list of Courses associated with logged in User
		/// </summary>
		/// <returns>List of Courses</returns>
		public List<Course> getCurrentCourseList()
		{
			//Finds current User
			var currUsId = _userService.getUserIdForCurrentApplicationUser();

			//Creates a List of Course IDs that User is associated with
			var userCourses = (from uscr in _db.UserCourseRelations
				where currUsId == uscr.UserID
				      && uscr.IsRemoved == false
				select uscr.CourseID).ToList();

			//Selectes Courses that are in the previous userCourses List.
			var courses = (from crs in _db.Courses
				where userCourses.Contains(crs.ID)
				      && crs.IsRemoved == false
				orderby crs.ID descending
				select crs).ToList();

			return courses;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="model"></param>
		internal void addUserToCourse(AddUserToCourseViewModel model)
        {
            int roleId = 1;
            if(model.Teacher == true)
            {
                roleId = 2;
            }

            _db.UserCourseRelations.Add(new UserCourseRelation {
                        CourseID = model.ID,
                        UserID = model.Name,
                        RoleID = roleId
            });
            _db.SaveChanges();
        }

		/// <summary>
		/// Adds a course into the database table Courses.
		/// </summary>
		/// <param name="model">this is the course that will be added.</param>
		internal int setCourse(CourseViewModel model)
        {
            Course course = new Course
            {
                Name = model.Name,
                Startdate = model.StartDate.Value,
                Isactive = model.Isactive,
                IsRemoved = model.IsRemoved
            };
			_db.Courses.Add(course);
            _db.SaveChanges();
			return course.ID;
        }

		/// <summary>
		/// Searches the database for a course with matching id as the parameter given
		/// and returns an entity model Course if it is found
		/// </summary>
        public Course getCourseById(int courseId)
        {

            var course = (from x in _db.Courses
                         where x.ID == courseId
						 && x.IsRemoved != true
                         select x).SingleOrDefault();

			if (course == null)
			{
				throw new Exception("Course does not exist for given ID");
			}
			return course;
        }

		/// <summary>
		/// Fills in all the needed info for a CourseViewModel from the courseID param
		/// </summary>
		/// <returns>CourseViewmodel with name, active status, id, removed bool, start date,
		/// list of assignments and list of users in course</returns>
		public CourseViewModel getCourseViewModelById(int courseId)
		{

			var course = (from x in _db.Courses
						  where x.ID == courseId 
						  && x.IsRemoved == false
						  select x).SingleOrDefault();

			if (course == null)
			{
				throw new Exception("Course does not exist");
			}

			var ass = _assignmentsService.getAssignmentsInCourse(courseId);
			var users = _userService.getUsersInCourse(courseId);

			CourseViewModel model = new CourseViewModel
			{
				Name = course.Name,
				Isactive = course.Isactive,
				ID = course.ID,
				IsRemoved = course.IsRemoved,
				StartDate = course.Startdate,
				Assignments = ass,
				User = users
			};
			return model;
		}

		/// <summary>
		/// Edit a course already in the database.
		/// </summary>
		/// <param name="model">This is the course with the new information</param>
		internal void editCourse(CourseViewModel model)
		{
			var edit = (from course in _db.Courses
						where model.ID == course.ID
						&& course.IsRemoved == false
						select course).FirstOrDefault();

			if(edit !=null)
			{
				edit.Name = model.Name;
				edit.Startdate = model.StartDate.Value;
				edit.IsRemoved = model.IsRemoved;
				edit.Isactive = model.Isactive;

				_db.SaveChanges();
			}
		}

		/// <summary>
		/// Removes a course from the course database and then calls removeCourseConnections to 
		/// remove all the connections students have to the course
		/// </summary>
		/// <param name="courseId">id of course to be removed</param>
		public void removeCourseById(int courseId)
		{
			var course = (from c in _db.Courses
							  where c.ID == courseId
							  && c.IsRemoved == false
							  select c).FirstOrDefault();

			if (course != null)
			{
				course.IsRemoved = true;
				_db.SaveChanges();

				removeCourseConnections(courseId);
			}
			else
			{
				throw new Exception("The course you want to remove does not exist.");
			}
		}

		/// <summary>
		/// Removes all conections a course has to users from the database UserCourseRelation
		/// </summary>
		/// <param name="courseId">course id to be removed</param>
		private void removeCourseConnections(int courseId)
		{
			var conn = (from item in _db.UserCourseRelations
						where item.CourseID == courseId
						&& item.IsRemoved == false
						select item).ToList();

			if(conn.Count > 0)
			{
				foreach (var item in conn)
				{
					item.IsRemoved = true;
				}
				_db.SaveChanges();
			}
			else
			{
				throw new Exception("This connection does not exist.");
			}
		}

		/// <summary>
		/// Finds the count all Courses in the system
		/// </summary>
		/// <returns>Total Number Of Courses</returns>
        public int numberOfCourses()
        {
            var courses = (from course in _db.Courses
						   where course.IsRemoved == false
                           select course).Count();

            return courses;
        }

		public List<User> getUsersInCourse(int courseId)
		{
			return (from u in _db.Users
					join ucr in _db.UserCourseRelations on u.ID equals ucr.UserID
					where ucr.CourseID == courseId
					select u).ToList();
		}

		public int getCourseIdFromMilestoneId(int milestoneId)
		{
			return (from c in _db.Courses
					join a in _db.Assignments on c.ID equals a.CourseID
					join m in _db.Milestones on a.ID equals m.AssignmentID
					where m.ID == milestoneId
					select c.ID).SingleOrDefault();
		}
	}
}
