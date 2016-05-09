﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Mooshak___H37.Services;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using System.Web.UI.HtmlControls;
using Mooshak___H37.Models.Entities;
using System.IO;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Student")]
	public class StudentController : Controller
	{
		AssigmentsService _assignService = new AssigmentsService();
		FilesService _filesService = new FilesService();

		// GET: Assignment
		[HttpGet]
		public ActionResult Index()
		{
			var viewModel = _assignService.getAllAssignments();
			return View(viewModel);
		}

		public ActionResult ViewAssignment(int id)
		{
			var viewModel = _assignService.Assignment(id);
			return View(viewModel);
		}

/*		public ActionResult Assignments()
		{
			var viewModel = _assignService.GetCourseAssignments();
			return View(viewModel);
		}*/

		[HttpGet]
		public ActionResult Submit(int? milestone)
		{
			StudentSubmit s = new StudentSubmit();
			s.Milestone = 10;
			return View(s);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Submit(StudentSubmit s)
		{
			if (s.File != null && s.File.ContentLength > 0)
			{
				_filesService.SaveSubmissionfile(s.File);
			}
			else
			{
				if (ViewBag.ErrorMessage != "")
				{
					return View("Error");
				}
				ViewBag.ErrorMessage = "No file submitted, try again";
				return View(s);
			}
			return RedirectToAction("Index");
		}
	}
}