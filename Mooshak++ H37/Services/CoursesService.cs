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

		public CoursesService()
		{
			_db = new ApplicationDbContext();
		}

        public List<TeacherCourseViewModel> getAllCourses()
        {

            var Courses = (from x in _db.Courses
                           select x).ToList();

            var viewModel = new List<TeacherCourseViewModel>();

            foreach (var course in Courses)
            {
                TeacherCourseViewModel model = new TeacherCourseViewModel
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

        public List<TeacherCourseViewModel> getAllCoursesByID(string UserID)
        {
            List<Course> Courses = null;

            var coursesID = (from x in _db.UserCourseRelations
                             where x.UserID == UserID
                             select x.CourseID);

            foreach (var id in coursesID)
            {
                Courses.Add(getCourseByID(id));
            }

            var viewModel = new List<TeacherCourseViewModel>();

            foreach (var course in Courses)
            {
                TeacherCourseViewModel model = new TeacherCourseViewModel
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

        public Course getCourseByID(int courseID)
        {

            var course = (from x in _db.Courses
                         where x.ID == courseID
                         select x).SingleOrDefault();

            return course;
        }


    }
}
