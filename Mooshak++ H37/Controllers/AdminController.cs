using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using Project_4.Controllers;

namespace Mooshak___H37.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : BasicController
	{
		private ErrorsService _errorsService = new ErrorsService(null);
		private CoursesService _courseService = new CoursesService(null);
		private UsersService _userService = new UsersService(null);

		// GET: Admin
		public ActionResult Index()
		{
			var model = _errorsService.getTopErrormessages();
			return View(model);
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

			return RedirectToAction("EditCourse", new { id = Courseid });
		}

		public ActionResult EditCourse(int? id)
		{
			if (id == null)
			{
				//TODO
				//throw exception
				id = 1;
			}

			CourseViewModel model = _courseService.getCourseViewModelById(id.Value);

			if (model == null)
			{
				//TODO
				//throw exception
			}

			ViewBag.userList = _userService.getAllUsersNameNotInCourse(id.Value);

			return View(model);
		}

		[HttpPost]
		public ActionResult EditCourse(CourseViewModel model)
		{
			if (ModelState.IsValid)
			{
				_courseService.editCourse(model);
			}
			else
			{
				//TODO
				//throw exception?
				return View(model);
			}

			return RedirectToAction("ViewCourses");
		}

		public ActionResult RemoveCourse(int? id)
		{
			if (id == null)
			{
				//TODO
				//Throw exception
				return null;//dont return null here
			}

			_courseService.removeCourseById(id.Value);

			return RedirectToAction("ViewCourses");
		}

		public ActionResult RemoveUser(int? id)
		{
			if(id == null)
			{
				// do something
			}

			_userService.removeUserById(id);

			return RedirectToAction("ViewUsers");
		}

        [HttpPost]
        public ActionResult addUserToCurse(AddUserToCourseViewModel model)
        {

           _courseService.addUserToCourse(model);

            return RedirectToAction("EditCourse", new { model.ID });
        }

        public ActionResult ViewUsers()
		{
			var viewModel = _userService.getAllUsersName();
			foreach (var item in viewModel)
			{
				item.RoleID = _userService.getRoleNamebyId(item.ID);
			}
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult EditUser(int id)
		{
			var viewModel = _userService.getSingleUser(id);
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult EditUser(UserViewModel model)
		{
			if(ModelState.IsValid)
			{
				_userService.EditUser(model);
			}
			else
			{
				return View(model);
			}

			return RedirectToAction("ViewUsers");
		}
	}
}