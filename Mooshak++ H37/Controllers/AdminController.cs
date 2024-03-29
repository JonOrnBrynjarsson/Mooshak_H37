﻿using System;
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
            return RedirectToAction("Register", "Account");
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
			if (ModelState.IsValid)
			{
				int courseId = _courseService.setCourse(model);

				if (courseId == 0)
				{
					throw new Exception("Course does not exist in this context");
				}
				return RedirectToAction("EditCourse", new { id = courseId });
			}
			return View(model);
		}

		public ActionResult EditCourse(int? id)
		{
			try
			{
				CourseViewModel model = _courseService.getCourseViewModelById(id.Value);

				ViewBag.userList = _userService.getAllUsersNameNotInCourse(id.Value);

				return View(model);
			}
			catch(Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
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
			try
			{
				_courseService.removeCourseById(id.Value);

				return RedirectToAction("ViewCourses");
			}
			catch(Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
		}

		public ActionResult RemoveUser(int? id)
		{
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
				item.RoleName = _userService.getAspUserRole(item.ID);
			}
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult EditUser(int id)
		{
			try
			{
				var viewModel = _userService.getSingleUserInfo(id);
				return View(viewModel);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
		}

		[HttpPost]
		public ActionResult EditUser(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				_userService.editUser(model);
			}
			else
			{
				return View(model);
			}

			return RedirectToAction("ViewUsers");
		}

        public ActionResult About()
        {
            return RedirectToAction("About", "Home");
        }
    }
}