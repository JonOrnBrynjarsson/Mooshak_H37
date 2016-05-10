using Mooshak___H37.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak___H37.Controllers
{
	public class HomeController : Controller
	{
        readonly UsersService _userService = new UsersService();
        readonly AssigmentsService _assignService = new AssigmentsService();
        readonly CoursesService _courseService = new CoursesService();
        readonly MilestoneService _milestoneService = new MilestoneService();
        readonly TestCaseService _testcaseService = new TestCaseService();
        readonly SubmissionsService _submissionsService = new SubmissionsService();

        public ActionResult Index()
		{
            ViewBag.NumOfUsers = _userService.UsersInSystem().ElementAt(0);
            ViewBag.NumOfStudents = _userService.UsersInSystem().ElementAt(1);
            ViewBag.NumOfTeachers = _userService.UsersInSystem().ElementAt(2);
            ViewBag.NumOfAdmins = _userService.UsersInSystem().ElementAt(4);

            ViewBag.NumOfAssignments = _assignService.NumberOfAssignments();

            ViewBag.NumOfCourses = _courseService.NumberOfCourses();

            ViewBag.NumOfMilestones = _milestoneService.NumberOfMilestones();

            ViewBag.NumOfSubmissions = _submissionsService.NumberOfSubmissions();

            ViewBag.NumOfTestCases = _testcaseService.NumberOfTestCases();


            return View();
		}
	}
}