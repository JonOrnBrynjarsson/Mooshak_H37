using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooshak___H37.Models.Entities;
using System.Threading.Tasks;
using Mooshak___H37.Models.Viewmodels;
using Microsoft.AspNet.Identity;

namespace Mooshak___H37.Services
{
	class CoursesService
	{
		private readonly ApplicationDbContext _db;
		private readonly AssigmentsService _assignmentsService;
		private readonly UsersService _userService;

		public CoursesService()
		{
			_db = new ApplicationDbContext();
			_assignmentsService = new AssigmentsService();
			_userService = new UsersService();
		}


		/// <summary>
		/// Finds the ID of logged in user
		/// </summary>
		/// <returns>User ID</returns>
		public int getCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}

		/// <summary>
		/// Gets a list of all Courses from the database that are not
		/// marked as Removed.
		/// </summary>
		/// <returns>list of all Courses</returns>
		public List<CourseViewModel> getAllCourses()
        {
            var courses = (from x in _db.Courses
						   where x.IsRemoved != true
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
        public List<CourseViewModel> getAllCoursesByUserID(int UserID)
        {
            List<Course> courses = null;

            var coursesID = (from x in _db.UserCourseRelations
                             where x.UserID == UserID &&
							 x.IsRemoved != true
                             select x.CourseID);

            foreach (var id in coursesID)
            {
                courses.Add(getCourseByID(id));
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
			//Finds current User
			var currUsId = getCurrentUser();

			//Creates a List of Course IDs that User is associated with
			var userCourses = (from uscr in _db.UserCourseRelations
							   where currUsId == uscr.UserID
							   && uscr.IsRemoved != true
							   select uscr.CourseID).ToList();

			//Selectes Courses that are in the previous userCourses List.
			var courses = (from crs in _db.Courses
							   where userCourses.Contains(crs.ID) && crs.IsRemoved != true
							   orderby crs.ID descending
							   select crs).ToList();
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
		/// TODO
		/// </summary>
		/// <param name="model"></param>
        internal void addUserToCourse(AddUserToCourseViewModel model)
        {
            int roleID = 1;
            if(model.Teacher == true)
            {
                roleID = 2;
            }

            _db.UserCourseRelations.Add(new UserCourseRelation {
                        CourseID = model.ID,
                        UserID = model.Name,
                        RoleID = roleID
            });
            _db.SaveChanges();
        }

		/// <summary>
		/// Finds Course where given Name is Associated
		/// </summary>
		/// <param name="name"></param>
		/// <returns>ID of Course</returns>
        public int getCourseIdByName(string name)
		{
			var course = (from x in _db.Courses
						  where name == x.Name
						  && x.IsRemoved != true
						  select x.ID).FirstOrDefault();

			if (course == 0)
			{
				throw new Exception("Course does not exist in this context");
			}

			return course;
		}

		/// <summary>
		/// Adds a course into the database table Courses.
		/// </summary>
		/// <param name="model">this is the course that will be added.</param>
		internal void setCourse(CourseViewModel model)
        {
            _db.Courses.Add(new Course
            {
                Name = model.Name,
                Startdate = model.StartDate.Value,
                Isactive = model.Isactive,
                IsRemoved = model.IsRemoved
            });
            _db.SaveChanges();
        }

		/// <summary>
		/// Searches the database for a course with matching id as the parameter given
		/// and returns an entity model Course if it is found
		/// </summary>
        public Course getCourseByID(int courseID)
        {

            var course = (from x in _db.Courses
                         where x.ID == courseID
						 && x.IsRemoved != true
                         select x).SingleOrDefault();

			if (course == null)
			{
				throw new Exception("Course does not exist for given ID");
			}

			return course;
        }

		//in progress
		public CourseViewModel getCourseViewModelByID(int courseID)
		{

			var course = (from x in _db.Courses
						  where x.ID == courseID && x.IsRemoved == false
						  select x).SingleOrDefault();

			if (course == null)
			{
				throw new Exception("Course does not exist");
			}

			var ass = _assignmentsService.getAssignmentsInCourse(courseID);
			var users = _userService.getUsersInCourse(courseID);

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

		internal void EditCourse(CourseViewModel model)
		{
			var edit = (from course in _db.Courses
						where model.ID == course.ID
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

		public void removeCourseByID(int id)
		{

			//find the correct course in the Courses table
			var course = (from c in _db.Courses
							  where c.ID == id
							  select c).FirstOrDefault();

			if (course != null)
			{
				course.IsRemoved = true;
				_db.SaveChanges();

				removeCourseConnections(id);
			}
			else
			{
				throw new Exception("The course you want to remove does not exist.");
			}
		}

		private void removeCourseConnections(int courseId)
		{
			var conn = (from item in _db.UserCourseRelations
						where item.CourseID == courseId
						select item).ToList();

			if(conn != null)
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
		/// Finds all Courses in the system
		/// </summary>
		/// <returns>Total Number Of Courses</returns>
        public int NumberOfCourses()
        {
            var courses = (from course in _db.Courses
                               select course).Count();

            return courses;
        }

    }
}
