using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;

namespace Mooshak___H37.Controllers
{
    public class TeacherController : Controller
    {
        CoursesService _courseService = new CoursesService();

        // GET: Teacher
        public ActionResult Index()
        {
            var viewModel = _courseService.getAllCourses();
            return View(viewModel);
        }
    }
}