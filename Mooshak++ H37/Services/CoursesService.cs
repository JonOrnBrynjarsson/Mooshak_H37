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
		private ApplicationDbContext _db;
		private AssigmentsService _assignmentsService = new AssigmentsService();
		private UsersService _userService = new UsersService();

		public CoursesService()
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

		public List<CourseViewModel> getAllCourses()
        {

            var Courses = (from x in _db.Courses
						   where x.IsRemoved != true
						   orderby x.Name ascending
                           select x).ToList();

            var viewModel = new List<CourseViewModel>();

            foreach (var course in Courses)
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

        public List<CourseViewModel> getAllCoursesByUserID(int UserID)
        {
            List<Course> Courses = null;

            var coursesID = (from x in _db.UserCourseRelations
                             where x.UserID == UserID
                             select x.CourseID);

            foreach (var id in coursesID)
            {
                Courses.Add(getCourseByID(id));
            }

            var viewModel = new List<CourseViewModel>();

	        if (Courses != null)
		        foreach (var course in Courses)
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

		public List<CourseViewModel> GetCoursesForUser()
		{
			var currUsId = GetCurrentUser();

			var userCourses = (from uscr in _db.UserCourseRelations
							   where currUsId == uscr.UserID
							   select uscr.CourseID).ToList();


			var Courses = (from courses in _db.Courses
							   where userCourses.Contains(courses.ID) && courses.IsRemoved != true
							   orderby courses.ID descending
							   select courses).ToList();

			//var assignments = (from assi in _db.Assignments
			//				   where userCourses.Contains(assi.CourseID)
			//				   orderby assi.CourseID descending
			//				   select assi).ToList();

			var viewModel = new List<CourseViewModel>();

			//foreach (var asi in assignments)
			//{
			//	AssignmentViewModel model = new AssignmentViewModel
			//	{
			//		CourseID = asi.CourseID,
			//		Description = asi.Description,
			//		DueDate = asi.DueDate,
			//		ID = asi.ID,
			//		IsActive = asi.IsActive,
			//		Name = asi.Name,
			//		SetDate = asi.SetDate,
			//	};
			//}


			foreach (var course in Courses)
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

			return viewModel;
		}


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

        public int getCourseIdByName(string name)
		{
			var course = (from x in _db.Courses
						  where name == x.Name
						  select x.ID).FirstOrDefault();

			return course;
		}

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

        public Course getCourseByID(int courseID)
        {

            var course = (from x in _db.Courses
                         where x.ID == courseID
                         select x).SingleOrDefault();

            return course;
        }
		//in progress
		public CourseViewModel getCourseViewModelByID(int courseID)
		{

			var course = (from x in _db.Courses
						  where x.ID == courseID && x.IsRemoved == false
						  select x).SingleOrDefault();

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
			var course = (from c in _db.Courses
							  where c.ID == id
							  select c).FirstOrDefault();

			if (course != null)
			{
				course.IsRemoved = true;
				_db.SaveChanges();
			}
		}

	}
}
