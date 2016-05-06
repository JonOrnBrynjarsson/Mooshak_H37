using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models;

namespace Mooshak___H37.Controllers
{
    public class AdminController : Controller
    {
        ErrorsService _errorsService = new ErrorsService();
        CoursesService _courseService = new CoursesService();
        UsersService _userService = new UsersService();

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

		public ActionResult ViewCourses()
		{
			var viewModel = _courseService.getAllCourses();
			return View(viewModel);
		}

		public ActionResult CreateCourse()
        {
            return View();
        }
    }
}