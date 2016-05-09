using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models.Viewmodels;
using Microsoft.AspNet.Identity;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Teacher")]
	public class TeacherController : Controller
	{
		AssigmentsService _assignService = new AssigmentsService();
		CoursesService _courseService = new CoursesService();
		MilestoneService _milestoneService = new MilestoneService();
		TestCaseService _testcaseService = new TestCaseService();

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

		public ActionResult CreateAssignment()
		{
			AssignmentViewModel viewModel = new AssignmentViewModel();

			ViewBag.CourseList = GetCourses();

			return View(viewModel);
		}

		private List<SelectListItem> GetCourses()
		{
			List<SelectListItem> result = new List<SelectListItem>();
			var allCourses = _courseService.GetCoursesForUser();

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a course - " });

			result.AddRange(allCourses.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

			return result;
		}

		private List<SelectListItem> GetMilestones(int id)
		{
			List<SelectListItem> result = new List<SelectListItem>();
			var milestones = _milestoneService.GetMilestonesForAssignment(id);

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a milestone - " });

			result.AddRange(milestones.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

			return result;
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
			if (ModelState.IsValid)
			{
				_milestoneService.CreateMilestone(model, assigID);
				return RedirectToAction("Index");
			}
			else
			{
				ViewBag.CourseList = GetCourses();
				return View(model);
			}
		}

		[HttpGet]
		public ActionResult EditMilestone(int milestoneID)
		{
			var viewModel = _milestoneService.GetSingleMilestone(milestoneID);
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
				return RedirectToAction("Index");
			}
			else
			{
				return View(model);
			}
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
				return RedirectToAction("Index");
			}
			else
			{
				return View(model);
			}
		}
	}
}

