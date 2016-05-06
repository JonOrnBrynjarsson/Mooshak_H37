using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;

namespace Mooshak___H37.Controllers
{
    public class AdminController : Controller
    {
        ErrorsService _errorsService = new ErrorsService();
        CoursesService _courseService = new CoursesService();

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
            var viewModel = _courseService.getAllCourses();
            return View(viewModel);
        }

        public ActionResult CreateCourse()
        {
            return View();
        }
    }
}