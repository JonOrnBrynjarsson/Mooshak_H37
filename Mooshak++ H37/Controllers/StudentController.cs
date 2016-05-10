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
		readonly AssigmentsService _assignService = new AssigmentsService();
		readonly FilesService _filesService = new FilesService();
		readonly CoursesService _courseService = new CoursesService();
		readonly UsersService _usersService = new UsersService();
		readonly MilestoneService _milestoneService = new MilestoneService();
		readonly SubmissionsService _submissionService = new SubmissionsService();

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
			//int userId = 
			//var viewModel = _courseService.getAllCoursesByUserID(userId);
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult Submit(int milestoneId)
		{
			if (!_milestoneService.UserCanSubmitMilestone(milestoneId))
			{
				return View("Error");
			}
			MilestoneViewmodel m = new MilestoneViewmodel();
			m = _milestoneService.GetSingleMilestone(milestoneId);
			StudentSubmitViewModel submission = new StudentSubmitViewModel();
			submission.Milestone = milestoneId;
			submission.DateSet = m.DateSet;
			submission.Duedate = m.DueDate;

			return View(submission);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Submit(StudentSubmitViewModel submit)
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

		public ActionResult ViewSubmissions (int milestoneID)
		{
			var viewModel = _submissionService.GetSubmissionsForMilestoneForStudent(milestoneID);
            ViewBag.AssignmentID = _assignService.GetAssignmentIDFromMilestoneID(milestoneID);
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult SubmissionDetail(int submissionId)
		{
			SubmissionsViewModel model = new SubmissionsViewModel();
			model = _submissionService.getSubmissionDetail(submissionId);
			return View(model);
		}

		[HttpGet]
        public ActionResult ViewSubmission (int submissionID)
        {
            var viewModel = _submissionService.GetSubmission(submissionID);
            viewModel.code = _filesService.getSubmissionFile(submissionID);
            viewModel.Testruns = _milestoneService.getTestrunsOutcomeForSubmission(submissionID);
            return View(viewModel);
        }

        [HttpGet]
		public ActionResult EditCode( )
		{
			int submissionId = 68;
			EditCodeViewModel editCode = new EditCodeViewModel();
			editCode.Milestone = _filesService.getMilestoneIdBySubmitId(submissionId);
			editCode.codefile = _filesService.getSubmissionFile(submissionId);

			return View(editCode);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditCode(EditCodeViewModel edit)
		{
			if (!string.IsNullOrEmpty(edit.codefile))
			{
				int submissionId = _filesService.createSubmission(edit.Milestone);
				if (submissionId == 0)
				{
					return View("Error");
				}
				_filesService.saveSubmissionfile(edit.codefile, submissionId);
				_filesService.testingSubmission(submissionId);
			}
			else
			{
				if (ViewBag.ErrorMessage != "")
				{
					return View("Error");
				}
				ViewBag.ErrorMessage = "No file submitted, try again";
				return View(edit);
			}
			return RedirectToAction("Index");
		}
		
	}
}