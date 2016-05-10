using System;
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
		CoursesService _courseService = new CoursesService();
		MilestoneService _milestoneService = new MilestoneService();

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
			ViewBag.TotalPercentage = _milestoneService.GetTotalMilestonePercentageForAssignment(id);
			return View(viewModel);
		}

		public ActionResult Assignments()
		{
			var viewModel = _courseService.GetCoursesForUser();
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult Submit(int milestoneID)
		{
			StudentSubmit s = new StudentSubmit();

// Hér þarf að laga milestone til að taka við því sem verið er að vinna með
			s.Milestone = milestoneID;

			return View(s);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Submit(StudentSubmit submit)
		{
			if (submit.File != null && submit.File.ContentLength > 0)
			{
				int submissionId = _filesService.createSubmission(submit.Milestone);
				if (submissionId == 0)
				{
					return View("Error");
				}
				_filesService.saveSubmissionfile(submit.File, submissionId);
				_filesService.testingSubmission(submissionId);
			}
			else
			{
				if (ViewBag.ErrorMessage != "")
				{
					return View("Error");
				}
				ViewBag.ErrorMessage = "No file submitted, try again";
				return View(submit);
			}
			return RedirectToAction("Index");
		}


		[HttpGet]
		public ActionResult EditCode(int submissionId )
		{
			StudentSubmit studentSubmit = new StudentSubmit();
			studentSubmit.Milestone = _filesService.getMilestoneIdBySubmitId(submissionId);
			//studentSubmit.File = _filesService

			return View();
		}
	}
}