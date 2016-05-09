using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Admin")]
	public class AdminController : Controller
    {
		private ErrorsService _errorsService = new ErrorsService();
		private CoursesService _courseService = new CoursesService();
		private UsersService _userService = new UsersService();

        // GET: Admin
        public ActionResult Index()
        {

            return View();
        }
		// GET: Admin
		public ActionResult IndexErrors()
		{
			var viewModel = _errorsService.getTopErrormessages();
			return View(viewModel);
		}
		public ActionResult CreateUser()
        {
            RegisterViewModel viewModel = new RegisterViewModel();
            viewModel.Course = _courseService.getAllCourses();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult CreateUser(RegisterViewModel model)
        {
            RegisterViewModel viewModel = new RegisterViewModel();
            viewModel.Course = _courseService.getAllCourses();

            model.Password = null;

            return View(model);
        }

        public ActionResult ViewCourses()
		{
			var viewModel = _courseService.getAllCourses();
			return View(viewModel);
		}

		public ActionResult CreateCourse()
        {
            CourseViewModel viewModel = new CourseViewModel();
            viewModel.User = _userService.getAllUsersName();

            return View(viewModel);
        }
     

        [HttpPost]
        public ActionResult CreateCourse(CourseViewModel model)
        {
            _courseService.setCourse(model);
            return View();
        }

		public ActionResult EditCourse(int? id)
		{
			if(id==null)
			{
				//TODO
				//throw exception
			}

			CourseViewModel model = _courseService.getCourseViewModelByID(id.Value);

			if(model == null)
			{
				//TODO
				//throw exception
			}

			return View(model);
		}

		[HttpPost]
		public ActionResult EditCourse(CourseViewModel model)
		{
			if (ModelState.IsValid)
			{
				_courseService.EditCourse(model);
			}
			else
			{
				//TODO
				//throw exception?
				return View(model);
			}

			return RedirectToAction("ViewCourses");
		}
	}
}