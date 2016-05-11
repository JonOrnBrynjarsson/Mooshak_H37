using Project_4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_4.Controllers
{
    public class BasicController : Controller
    {
		protected override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			Exception ex = filterContext.Exception;
			//Logger.Instance.LogException(filterContext);
		}
	}
}