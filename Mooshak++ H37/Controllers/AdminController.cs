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
			ViewBag.userList = _userService.getAllUsersName();

            return View(viewModel);
        }
     

        [HttpPost]
        public ActionResult CreateCourse(CourseViewModel model)
        {
			//Save the new course that was just created to the DB
            _courseService.setCourse(model);

			//find the id number of the course just created from the DB
			int Courseid = _courseService.getCourseIdByName(model.Name);

			return RedirectToAction("EditCourse", new { id = Courseid});
        }

		public ActionResult EditCourse(int? id)
		{
			if(id==null)
			{
				//TODO
				//throw exception
				id = 1;
			}

			CourseViewModel model = _courseService.getCourseViewModelByID(id.Value);

			if(model == null)
			{
				//TODO
				//throw exception
			}

			ViewBag.userList = _userService.getAllUsersName();

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

		public ActionResult DeleteCourse(int? id)
		{
			if(id == null)
			{
				//TODO
				//Throw exception
				return null;//dont return null here
			}

			return View();
		}

		public ActionResult ViewUsers()
		{
			var viewModel = _userService.getAllUsersName();
			foreach (var item in viewModel)
			{
				item.RoleID = _userService.getRoleNamebyID(item.ID);
			}
			return View(viewModel);
		}
	}
}