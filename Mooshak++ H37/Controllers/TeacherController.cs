using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models.Viewmodels;
using Microsoft.AspNet.Identity;
using Project_4.Controllers;
using System.Diagnostics;

namespace Mooshak___H37.Controllers
{
	//[Authorize(Roles = "Teacher")]
	public class TeacherController : BasicController
	{
		readonly AssigmentsService _assignService = new AssigmentsService(null);
		readonly CoursesService _courseService = new CoursesService(null);
		readonly MilestoneService _milestoneService = new MilestoneService(null);
		readonly TestCaseService _testcaseService = new TestCaseService(null);
		readonly SubmissionsService _submissionsService = new SubmissionsService(null);
		readonly FilesService _filesService = new FilesService(null);

		// GET: Assignment
		[HttpGet]
		public ActionResult Index()
		{
			//Returns alls assignments that the teacher is associated with

            var viewModel = _assignService.getAllAssignmentsForCurrUser();

            return View(viewModel);
        }

        public ActionResult ViewAssignment(int id)
        {
            //Returns selected assignment
            try
            {
                var viewModel = _assignService.getAssignmentById(id);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }



        private List<SelectListItem> GetCourses()
        {
            //Creates a list of Courses that teacher is associated with.
            List<SelectListItem> result = new List<SelectListItem>();
            var allCourses = _courseService.getCoursesForUser();

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
                _assignService.createAssignment(model);
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
            try
            {
                AssignmentViewModel model = _assignService.getAssignmentById(id);
                ViewBag.MilestoneList = GetMilestones(id);
                ViewBag.TotalPercentage = _milestoneService.GetTotalMilestonePercentageForAssignment(id);
                return View(model);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

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
            try
            {
                _milestoneService.TeacherCanCreateMilestone(model, assigID);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

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

            //if (_milestoneService.TeacherCanCreateMilestone(model, assigID))
            //{
            //	if (ModelState.IsValid)
            //	{
            //		_milestoneService.CreateMilestone(model, assigID);
            //		return RedirectToAction("Milestones", new { id = assigID });
            //	}
            //	else
            //	{
            //		ViewBag.CourseList = GetCourses();
            //		return View(model);
            //	}
            //}
            //else
            //{
            //	return View("Error");
            //}


        }

        [HttpGet]
        public ActionResult EditMilestone(int milestoneID)
        {
            try
            {
                var viewModel = _milestoneService.GetSingleMilestone(milestoneID);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }

        public ActionResult ViewSubmissions(int milestoneID)
        {
            try
            {
                var viewmodel = _submissionsService.GetSubmissionsForMilestone(milestoneID);
                return View(viewmodel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }

        [HttpGet]
        public ActionResult GradeSubmission(int submissionId)
        {
            try
            {
                var viewModel = _submissionsService.GetSubmission(submissionId);
                viewModel.code = _filesService.getSubmissionFile(submissionId);
                viewModel.Testruns = _milestoneService.getTestrunsOutcomeForSubmission(submissionId);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }

        [HttpPost]
        public ActionResult GradeSubmission(SubmissionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _submissionsService.GradeAssignment(model);
                    //return RedirectToAction("ViewSubmissions", new { id = model.ID });
                    return RedirectToAction("ViewSubmissions", new
                    {
                        milestoneID =
                        _submissionsService.GetMilestoneIDFromSubmissionID(model.ID)
                    });
                }
                catch (Exception e)
                {
                    return View("~/Views/Shared/Cerror.cshtml", e);
                }
            }

            else
            {
                return View(model);
            }
        }

        public ActionResult Assignments()
        {

            var viewModel = _courseService.getCoursesForUser();
            ViewBag.Today = _assignService.today();

            return View(viewModel);

        }

        [HttpPost]
        public ActionResult EditMilestone(MilestoneViewmodel model, int milestoneID)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _milestoneService.EditMilestone(model, milestoneID);
                    var assignmentID = _assignService.getAssignmentIDFromMilestoneID(milestoneID);
                    return RedirectToAction("Milestones", new { id = assignmentID });
                }
                else
                {
                    throw new Exception("Input Data is Invalid");

                }
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }

        [HttpGet]
        public ActionResult RemoveMilestone(int id)
        {
            try
            {
                var viewModel = _milestoneService.GetSingleMilestone(id);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }
        [HttpPost]
        public ActionResult RemoveMilestone(MilestoneViewmodel model)
        {
            try
            {
                _milestoneService.RemoveMilestone(model);
                var assignmentID = _assignService.getAssignmentIDFromMilestoneID(model.ID);
                return RedirectToAction("Milestones", new { id = assignmentID });
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }

        [HttpGet]
        public ActionResult EditAssignment(int id)
        {
            try
            {
                var viewModel = _assignService.getAssignmentById(id);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }

        [HttpPost]
        public ActionResult EditAssignment(AssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _assignService.editAssignment(model);
                    return RedirectToAction("ViewAssignment", new { id = model.ID });
                }
                catch (Exception e)
                {
                    return View("~/Views/Shared/Cerror.cshtml", e);
                }

            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult RemoveAssignment(int id)
        {
            try
            {
                var viewModel = _assignService.getAssignmentById(id);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }
        [HttpPost]
        public ActionResult RemoveAssignment(AssignmentViewModel model)
        {
            try
            {
                _assignService.removeAssignment(model);
                return RedirectToAction("Assignments");
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
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

            ViewBag.AssignID = _assignService.getAssignmentIDFromMilestoneID(milID);

            if (viewModel.Count == 0)
            {
                viewModel = new List<TestCaseViewModel>();
            }

            return View(viewModel);
        }



        [HttpGet]
        public ActionResult RemoveTestCase(int id)
        {
            try
            {
                var viewModel = _testcaseService.GetSingleTestCase(id);
                return View(viewModel);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }
        [HttpPost]
        public ActionResult RemoveTestCase(TestCaseViewModel model)
        {
            try
            {
                _testcaseService.RemoveTestCase(model);
                //LAGA::
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }
    }
}

