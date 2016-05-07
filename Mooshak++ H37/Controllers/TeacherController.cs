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
			return View(viewModel);
		}


		[HttpPost]
		public ActionResult CreateAssignment(AssignmentViewModel model)
		{
			_assignService.CreateAssignment(model);
			return View();
		}
	}
}