using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooshak___H37.Models.Entities;
using System.Threading.Tasks;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Services
{
	class CoursesService
	{
		private ApplicationDbContext _db;
		private AssigmentsService _assignmentsService;
		private UsersService _userService;

		public CoursesService()
		{
			_db = new ApplicationDbContext();
		}

        public List<CourseViewModel> getAllCourses()
        {

            var Courses = (from x in _db.Courses
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
						  where x.ID == courseID
						  select x).SingleOrDefault();

			var ass = _assignmentsService.getAssignmentsInCourse(courseID);
			var users = _userService.getAllUsersName();
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


	}
}
