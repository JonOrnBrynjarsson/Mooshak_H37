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
using Project_4.Controllers;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Student")]
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
			var viewModel = _assignService.getAllAssignments();
			return View(viewModel);
		}

		public ActionResult ViewAssignment(int id)
		{
			try
			{
				//returns selected assignment
				var viewModel = _assignService.Assignment(id);
				//gives total percentage of all milestones in given assignment
				ViewBag.TotalPercentage = _milestoneService.GetTotalMilestonePercentageForAssignment(id);
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
			var viewModel = _courseService.GetCoursesForUser();
            ViewBag.Today = _assignService.Today();
			return View(viewModel);
		}

		[HttpGet]
		public ActionResult Submit(int milestoneId)
		{

			//Checks if user has already submitted the maximum number
			//Of allowed submissions

			try
			{
				var canSubmit = _milestoneService.UserCanSubmitMilestone(milestoneId);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}

			MilestoneViewmodel m = new MilestoneViewmodel();

			m = _milestoneService.GetSingleMilestone(milestoneId);

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
		if (submit.File != null && submit.File.ContentLength > 0)
			{
				try
				{
					int submissionId = _filesService.createSubmission(submit.Milestone);
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
			return RedirectToAction("Index");
		}

		public ActionResult ViewSubmissions (int milestoneID)
		{
			try
			{
				//Returns submissions that student has submitted in given milestone
				var viewModel = _submissionService.GetSubmissionsForMilestoneForStudent(milestoneID);

				//ID of assignment is added to Viewbag to be able to Go Back
				ViewBag.AssignmentID = _assignService.GetAssignmentIDFromMilestoneID(milestoneID);
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
        public ActionResult ViewSubmission (int submissionID)
        {
			try
			{
				var viewModel = _submissionService.GetSubmission(submissionID);
				viewModel.code = _filesService.getSubmissionFile(submissionID);
				viewModel.Testruns = _milestoneService.getTestrunsOutcomeForSubmission(submissionID);
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
			editCode.Milestone = _filesService.getMilestoneIdBySubmitId(submissionId);
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
					int submissionId = _filesService.createSubmission(milestoneId);
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
	}
}