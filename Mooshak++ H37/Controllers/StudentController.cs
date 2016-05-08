using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Controllers
{
	[Authorize(Roles = "Student")]
	public class StudentController : Controller
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

/*		public ActionResult Assignments()
		{
			var viewModel = _assignService.GetCourseAssignments();
			return View(viewModel);
		}*/
    }


}