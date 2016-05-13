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
using Project_4.Controllers;

namespace Mooshak___H37.Controllers
{
	[Authorize(Roles = "Student")]
	public class StudentController : BasicController
	{

		readonly AssigmentsService _assignService = new AssigmentsService(null);
		readonly FilesService _filesService = new FilesService(null);
		readonly CoursesService _courseService = new CoursesService(null);
		readonly UsersService _usersService = new UsersService(null);
		readonly MilestoneService _milestoneService = new MilestoneService(null);
		readonly SubmissionsService _submissionService = new SubmissionsService(null);

		// GET: Assignment
		[HttpGet]
		public ActionResult Index()
		{
			var viewModel = _assignService.getAllAssignmentsForCurrUser();
			return View(viewModel);
		}

		public ActionResult ViewAssignment(int assignmentId)
		{
			try
			{
				//returns selected assignment
				var viewModel = _assignService.getAssignmentById(assignmentId);
				//gives total percentage of all milestones in given assignment
				ViewBag.TotalPercentage = _milestoneService.getTotalMilestonePercentageForAssignment(assignmentId);
				return View(viewModel);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
		}

		public ActionResult Assignments()
		{
			//Returns all assignments in all Courses that user is in.
			var viewModel = _courseService.getCoursesForUser();
            ViewBag.Today = _assignService.today();
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult Submit(int milestoneId)
		{

			//Checks if user has already submitted the maximum number
			//Of allowed submissions

			try
			{
				var canSubmit = _milestoneService.userCanSubmitMilestone(milestoneId);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}

			ViewBag.AssignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);

			MilestoneViewmodel m = new MilestoneViewmodel();

			m = _milestoneService.getSingleMilestone(milestoneId);

			StudentSubmitViewModel submission = new StudentSubmitViewModel();

			submission.Milestone = milestoneId;
			submission.DateSet = m.DateSet;
			submission.Duedate = m.DueDate;

			return View(submission);

			//if (!_milestoneService.UserCanSubmitMilestone(milestoneId))
			//{
			//	//You have already submitted the maximum number of times.
			//	return View("Error");
			//}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Submit(StudentSubmitViewModel submit)
		{
			int submissionId;
		if (submit.File != null && submit.File.ContentLength > 0)
			{
				try
				{
					submissionId = _submissionService.createSubmission(submit.Milestone);
					if (submissionId == 0)
					{
						throw new Exception();
					}
					_filesService.saveSubmissionfile(submit.File, submissionId);
					_filesService.testingSubmission(submissionId);
				}
				catch (Exception)
				{
					return View("Error");
				}
			
			}
			else
			{
				ViewBag.ErrorMessage = "No file submitted, please try again";
				return View(submit);
			}
            //return RedirectToAction("Index");

            //var assignmentID = _assignService.getAssignmentIDFromMilestoneID(submit.Milestone);
			int milestoneId = _milestoneService.getMilestoneIdBySubmitId(submissionId);
            return RedirectToAction("ViewSubmissions", new { milestoneID = milestoneId });
			//return RedirectToAction("ViewAssignment", new { id = assignmentID });

		}

        public ActionResult ViewSubmissions (int milestoneId)
		{
			try
			{
				//Returns submissions that student has submitted in given milestone
				var viewModel = _submissionService.getSubmissionsForMilestoneForStudent(milestoneId);

				//ID of assignment is added to Viewbag to be able to Go Back
				ViewBag.assignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);
				ViewBag.milestoneId = milestoneId;
				ViewBag.allowedSubmissions = _milestoneService.allowedSubmissionsForMilestone(milestoneId);
				return View(viewModel);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
		}

		[HttpGet]
		public ActionResult SubmissionDetail(int submissionId)
		{
			SubmissionsViewModel model = new SubmissionsViewModel();
			model = _submissionService.getSubmissionDetail(submissionId);
			return View(model);
		}

		[HttpGet]
        public ActionResult ViewSubmission (int submissionId)
        {
			try
			{
				var viewModel = _submissionService.getSubmission(submissionId);
				viewModel.code = _filesService.getSubmissionFile(submissionId);
				viewModel.Testruns = _submissionService.getTestrunsOutcomeForSubmission(submissionId);
				return View(viewModel);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
        }

        [HttpGet]
		public ActionResult EditCode( )
		{
			int submissionId = 68;
			EditCodeViewModel editCode = new EditCodeViewModel();
			editCode.Milestone = _milestoneService.getMilestoneIdBySubmitId(submissionId);
			editCode.codefile = _filesService.getSubmissionFile(submissionId);

			return View(editCode);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult SubmitEdit(EditCodeViewModel model)
		public ActionResult SubmitEdit()
		{
			string code = Request.QueryString["codefile"];
			string milstr = Request.QueryString["Milestone"];

			int milestoneId = 0;
			bool mil = int.TryParse(milstr, out milestoneId);

			if (!string.IsNullOrEmpty(code))
			{
				try
				{
					int submissionId = _submissionService.createSubmission(milestoneId);
					if (submissionId == 0)
					{
						throw new Exception();
					}

					_filesService.saveSubmissionfile(code, submissionId);
					_filesService.testingSubmission(submissionId);
				}
				catch (Exception)
				{
					return View("Error");
				}
			}
			return RedirectToAction("Index");
		}

        public ActionResult About()
        {
            return RedirectToAction("About", "Home");
        }
    }
}