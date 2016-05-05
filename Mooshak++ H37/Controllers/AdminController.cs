using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak___H37.Services;

namespace Mooshak___H37.Controllers
{
    public class AdminController : Controller
    {
        ErrorsService _errorsService = new ErrorsService();

        // GET: Admin
        public ActionResult Index()
        {
            var viewModel = _errorsService.getTopErrormessages();
            return View();
        }
    }
}