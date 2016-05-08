using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Teacher")]
	public class TeacherController : Controller
    {
		AssigmentsService _assignService = new AssigmentsService();
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
			var allCourses = _courseService.getAllCourses();

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a course - " });

			result.AddRange(allCourses.Select(x=>new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

			return result;
		}

		private List<SelectListItem> GetMilestones(int id)
		{
			List<SelectListItem> result = new List<SelectListItem>();
			var milestones = _milestoneService.GetMilestonesForAssignment(id);

			result.Add(new SelectListItem() { Value = "", Text = " - Choose a course - " });

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
	}
}