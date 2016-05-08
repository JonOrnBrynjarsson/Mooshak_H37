using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Mooshak___H37.Services;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using System.Web.UI.HtmlControls;

namespace Mooshak___H37.Controllers
{
	[Authorize(Roles = "Student")]
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
			s.milestone = 10;
			return View(s);
		}

		[HttpPost]
		public ActionResult Submit(StudentSubmit submit)
		{
			
			int milestone;
			bool result = Int32.TryParse(submit.milestone.ToString(), out milestone);
			
			//string file = collection["file"];
			//HttpPostedFile file = collection["file"];
			/*
			if (file.ContentLength > 0)
			{
				_filesService.SaveSubmissionfile(file)
			}
*/
			return RedirectToAction("Index");
		}
	}
}