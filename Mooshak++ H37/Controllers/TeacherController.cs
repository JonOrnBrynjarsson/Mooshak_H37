using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models.Viewmodels;
using Microsoft.AspNet.Identity;
using Project_4.Controllers;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Teacher")]
	public class TeacherController : BasicController
	{
		readonly AssigmentsService _assignService = new AssigmentsService();
		readonly CoursesService _courseService = new CoursesService();
		readonly MilestoneService _milestoneService = new MilestoneService();
		readonly TestCaseService _testcaseService = new TestCaseService();
		readonly SubmissionsService _submissionsService = new SubmissionsService();
		readonly FilesService _filesService = new FilesService();

		// GET: Assignment
		[HttpGet]
		public ActionResult Index()
		{
			//Returns alls assignments that the teacher is associated with
			var viewModel = _assignService.getAllAssignments();
			return View(viewModel);
		}

		public ActionResult ViewAssignment(int id)
		{
			//Returns selected assignment
			var viewModel = _assignService.Assignment(id);
			return View(viewModel);
		}



		private List<SelectListItem> GetCourses()
		{
			//Creates a list of Courses that teacher is associated with.
			List<SelectListItem> result = new List<SelectListItem>();
			var allCourses = _courseService.GetCoursesForUser();

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a course - " });

			result.AddRange(allCourses.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

			return result;
		}

		private List<SelectListItem> GetMilestones(int assignmentId)
		{
			//Creates a list of Milestones that Assignment is associated with.

			List<SelectListItem> result = new List<SelectListItem>();
			var milestones = _milestoneService.GetMilestonesForAssignment(assignmentId);

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a milestone - " });

			result.AddRange(milestones.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

			return result;
		}

		public ActionResult CreateAssignment()
		{
			AssignmentViewModel viewModel = new AssignmentViewModel();

			//Returns list of All Courses that teacher is associated with
			ViewBag.CourseList = GetCourses();

			return View(viewModel);
		}

		[HttpPost]
		public ActionResult CreateAssignment(AssignmentViewModel model)
		{
			if (ModelState.IsValid)
			{
				_assignService.CreateAssignment(model);
				return RedirectToAction("Index");
			}
			else
			{
				ViewBag.CourseList = GetCourses();
				return View(model);
			}
		}



		[HttpGet]
		public ActionResult Milestones(int id)
		{
			AssignmentViewModel model = _assignService.Assignment(id);
			ViewBag.MilestoneList = GetMilestones(id);
			ViewBag.TotalPercentage = _milestoneService.GetTotalMilestonePercentageForAssignment(id);
			return View(model);
		}


		[HttpGet]
		public ActionResult CreateMilestone(MilestoneViewmodel model)
		{

			MilestoneViewmodel viewModel = new MilestoneViewmodel();
			return View(viewModel);
		}


		[HttpPost]
		public ActionResult CreateMilestone(MilestoneViewmodel model, int assigID)
		{
			if (_milestoneService.TeacherCanCreateMilestone(model, assigID))
			{
				if (ModelState.IsValid)
				{
					_milestoneService.CreateMilestone(model, assigID);
					return RedirectToAction("Milestones", new { id = assigID });
				}
				else
				{
					ViewBag.CourseList = GetCourses();
					return View(model);
				}
			}
			else
			{
				return View("Error");
			}


		}

		[HttpGet]
		public ActionResult EditMilestone(int milestoneID)
		{
			var viewModel = _milestoneService.GetSingleMilestone(milestoneID);
			return View(viewModel);
		}

		public ActionResult ViewSubmissions (int milestoneID)
		{
			var viewmodel = _submissionsService.GetSubmissionsForMilestone(milestoneID);
			return View(viewmodel);
		}

		[HttpGet]
		public ActionResult GradeSubmission(int submissionId)
		{

			var viewModel = _submissionsService.GetSubmission(submissionId);
			viewModel.code = _filesService.getSubmissionFile(submissionId);
			viewModel.Testruns = _milestoneService.getTestrunsOutcomeForSubmission(submissionId);
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult GradeSubmission(SubmissionsViewModel model)
		{
			if (ModelState.IsValid)
			{
				_submissionsService.GradeAssignment(model);
				//return RedirectToAction("ViewSubmissions", new { id = model.ID });
				return RedirectToAction("ViewSubmissions", new { milestoneID = 
					_submissionsService.GetMilestoneIDFromSubmissionID(model.ID)});			}
			else
			{
				return View(model);
			}
		}

		public ActionResult Assignments()
		{
			var viewModel = _courseService.GetCoursesForUser();
			return View(viewModel);
		}








		[HttpPost]
		public ActionResult EditMilestone(MilestoneViewmodel model, int milestoneID)
		{
			if (ModelState.IsValid)
			{
				_milestoneService.EditMilestone(model, milestoneID);
				return RedirectToAction("Index");
			}
			else
			{
				//ViewBag.CourseList = GetCourses();
				return View(model);
			}
		}

		[HttpGet]
		public ActionResult RemoveMilestone(int id)
		{
			var viewModel = _milestoneService.GetSingleMilestone(id);
			return View(viewModel);
		}
		[HttpPost]
		public ActionResult RemoveMilestone(MilestoneViewmodel model)
		{
			_milestoneService.RemoveMilestone(model);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult EditAssignment(int id)
		{
			var viewModel = _assignService.Assignment(id);
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult EditAssignment(AssignmentViewModel model)
		{
			if (ModelState.IsValid)
			{
				_assignService.EditAssignment(model);
				return RedirectToAction("ViewAssignment", new { id = model.ID });

			}
			else
			{
				return View(model);
			}
		}

		[HttpGet]
		public ActionResult RemoveAssignment(int id)
		{
			var viewModel = _assignService.Assignment(id);
			return View(viewModel);
		}
		[HttpPost]
		public ActionResult RemoveAssignment(AssignmentViewModel model)
		{
			_assignService.RemoveAssignment(model);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult CreateTestCase(TestCaseViewModel model)
		{
			TestCaseViewModel viewModel = new TestCaseViewModel();
			return View(viewModel);
		}


		[HttpPost]
		public ActionResult CreateTestCase(TestCaseViewModel model, int milestoneID)
		{
			if (ModelState.IsValid)
			{
				_testcaseService.CreateTestCase(model, milestoneID);
				return RedirectToAction("TestCases", new { milID = milestoneID });

			}
			else
			{
				return View(model);
			}
		}

		[HttpGet]
		public ActionResult TestCases(int milID)
		{
			var viewModel = _testcaseService.GetTestCasesForMilestone(milID);

			ViewBag.MilestID = milID;

			ViewBag.AssignID = _assignService.GetAssignmentIDFromMilestoneID(milID);

			if (viewModel.Count == 0)
			{
				viewModel = new List<TestCaseViewModel>();
			}

			return View(viewModel);
		}



		[HttpGet]
		public ActionResult RemoveTestCase(int id)
		{
			var viewModel = _testcaseService.GetSingleTestCase(id);
			return View(viewModel);
		}
		[HttpPost]
		public ActionResult RemoveTestCase(TestCaseViewModel model)
		{
			_testcaseService.RemoveTestCase(model);
			return RedirectToAction("Index");
		}
	}
}

