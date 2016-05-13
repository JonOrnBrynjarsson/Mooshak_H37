using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mooshak___H37.Services;
using Mooshak___H37.Models.Viewmodels;
using Project_4.Controllers;

namespace Mooshak___H37.Controllers
{
	[Authorize(Roles = "Teacher")]
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

        public ActionResult ViewAssignment(int assignmentId)
        {
            //Returns selected assignment
            try
            {
                var viewModel = _assignService.getAssignmentById(assignmentId);
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
            var allCourses = _courseService.getCoursesForUserWithEmptyCourses();

            result.Add(new SelectListItem() { Value = "", Text = " - Choose a course - " });

            result.AddRange(allCourses.Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }));

            return result;
        }

        private List<SelectListItem> GetMilestones(int assignmentId)
        {
            //Creates a list of Milestones that Assignment is associated with.

            List<SelectListItem> result = new List<SelectListItem>();
            var milestones = _milestoneService.getMilestonesForAssignment(assignmentId);

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
                return RedirectToAction("Assignments");
            }
            else
            {
                ViewBag.CourseList = GetCourses();
                return View(model);
            }
        }



        [HttpGet]
        public ActionResult Milestones(int assignmentId)
        {
            try
            {
                AssignmentViewModel model = _assignService.getAssignmentById(assignmentId);
                ViewBag.MilestoneList = GetMilestones(assignmentId);
                ViewBag.TotalPercentage = _milestoneService.getTotalMilestonePercentageForAssignment(assignmentId);
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
        public ActionResult CreateMilestone(MilestoneViewmodel model, int assignmentId)
        {
            try
            {
                _milestoneService.teacherCanCreateMilestone(model, assignmentId);

			}
			catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

            if (ModelState.IsValid)
            {
                _milestoneService.createMilestone(model, assignmentId);
                return RedirectToAction("Milestones", new { assignmentId = assignmentId });
            }
            else
            {
                ViewBag.CourseList = GetCourses();
                return View(model);
            }

        }

        

        public ActionResult ViewSubmissions(int milestoneId)
        {
            try
            {
                var viewmodel = _submissionsService.getSubmissionsForMilestone(milestoneId);
				ViewBag.assignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);
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
                var viewModel = _submissionsService.getSubmission(submissionId);
                viewModel.code = _filesService.getSubmissionFile(submissionId);
                viewModel.Testruns = _submissionsService.getTestrunsOutcomeForSubmission(submissionId);
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
                    _submissionsService.gradeAssignment(model);
                    return RedirectToAction("ViewSubmissions", new
                    {
                        milestoneId =
                        _milestoneService.getMilestoneIdBySubmitId(model.ID)
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
            ViewBag.today = _assignService.today();

            return View(viewModel);

        }

		[HttpGet]
		public ActionResult EditMilestone(int milestoneId)
		{
			try
			{
				var viewModel = _milestoneService.getSingleMilestone(milestoneId);
				ViewBag.assignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);
				return View(viewModel);
			}
			catch (Exception e)
			{
				return View("~/Views/Shared/Cerror.cshtml", e);
			}
		}

		[HttpPost]
        public ActionResult EditMilestone(MilestoneViewmodel model, int milestoneId)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _milestoneService.editMilestone(model, milestoneId);
                    var assignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);
                    return RedirectToAction("Milestones", new { assignmentId = assignmentId });
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
        public ActionResult RemoveMilestone(int milestoneId)
        {
            try
            {
                var viewModel = _milestoneService.getSingleMilestone(milestoneId);
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
                _milestoneService.removeMilestone(model);
                var assignmentId = _assignService.getAssignmentIDFromMilestoneID(model.ID);
                return RedirectToAction("Milestones", new { assignmentId = assignmentId });
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }

        [HttpGet]
        public ActionResult EditAssignment(int assignmentId)
        {
            try
            {
                var viewModel = _assignService.getAssignmentById(assignmentId);
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
                    return RedirectToAction("ViewAssignment", new { assignmentId = model.ID });
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
        public ActionResult RemoveAssignment(int assignmentId)
        {
            try
            {
                var viewModel = _assignService.getAssignmentById(assignmentId);
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
			ViewBag.MilestoneId = model.MilestoneID;
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult CreateTestCase(TestCaseViewModel model, int milestoneId)
        {
            if (ModelState.IsValid)
            {
                _testcaseService.createTestCase(model, milestoneId);
                return RedirectToAction("TestCases", new { milestoneId = milestoneId });

            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult TestCases(int milestoneId)
        {
            var viewModel = _testcaseService.getTestCasesVMForMilestone(milestoneId);

            ViewBag.milestoneId = milestoneId;

            ViewBag.assignmentId = _assignService.getAssignmentIDFromMilestoneID(milestoneId);

            if (viewModel.Count == 0)
            {
                viewModel = new List<TestCaseViewModel>();
            }

            return View(viewModel);
        }



        [HttpGet]
        public ActionResult RemoveTestCase(int testCaseId)
        {
            try
            {
				TestCaseViewModel viewModel = _testcaseService.getSingleTestCase(testCaseId);
                return View(viewModel);
			}
			catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }
        }
        [HttpPost]
        public ActionResult RemoveTestCase(int testcaseId, int milestoneId)
        {
            try
            {
                _testcaseService.removeTestCase(testcaseId);
	            return RedirectToAction("TestCases", new { milestoneId = milestoneId });

            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Cerror.cshtml", e);
            }

        }

        public ActionResult About()
        {
            return RedirectToAction("About", "Home");
        }
    }
}

