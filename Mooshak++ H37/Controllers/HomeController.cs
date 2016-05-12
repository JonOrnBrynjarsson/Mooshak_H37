using Microsoft.Ajax.Utilities;
using Mooshak___H37.Services;
using Project_4.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Mooshak___H37.Controllers
{
    public class HomeController : BasicController
	{
		readonly UsersService _userService = new UsersService();
		readonly AssignmentsService _assignService = new AssignmentsService();
		readonly CoursesService _courseService = new CoursesService();
		readonly MilestoneService _milestoneService = new MilestoneService();
		readonly TestCaseService _testcaseService = new TestCaseService();
		readonly SubmissionsService _submissionsService = new SubmissionsService();

		public ActionResult Index()
		{
			ViewBag.NumOfUsers = _userService.UsersInSystem().ElementAt(0);
			ViewBag.NumOfStudents = _userService.UsersInSystem().ElementAt(1);
			ViewBag.NumOfTeachers = _userService.UsersInSystem().ElementAt(2);
			//ViewBag.NumOfAdmins = _userService.UsersInSystem().ElementAt(4);

			ViewBag.NumOfAssignments = _assignService.NumberOfAssignments();

			ViewBag.NumOfCourses = _courseService.NumberOfCourses();

			ViewBag.NumOfMilestones = _milestoneService.NumberOfMilestones();

			ViewBag.NumOfSubmissions = _submissionsService.NumberOfSubmissions();

			ViewBag.NumOfTestCases = _testcaseService.NumberOfTestCases();

            if (Request.IsAuthenticated)
            {
                var ID = _userService.getUserIdForCurrentApplicationUser();
                return RedirectToAction("Index", _userService.getAspUserRole(ID));
            }

			return RedirectToAction("Login", "Account");
		}
	}
}