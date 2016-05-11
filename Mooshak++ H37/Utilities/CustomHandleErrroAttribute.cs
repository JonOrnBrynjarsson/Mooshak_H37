using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Project_4;

namespace Project_4.Utilities
{
	class CustomApplicationExceptionAttribute : HandleErrorAttribute
	{

		public override void OnException(ExceptionContext filterContext)
		{
			Exception ex = filterContext.Exception;
			
			string currentController = (string)filterContext.RouteData.Values["controller"];
			string currentActionName = (string)filterContext.RouteData.Values["action"];			string viewName = "Error";

			Logger.Instance.LogException(ex, currentController, currentActionName);

			if (currentController == "Movie" || currentController == "Book")
			{
			}
			else if (ex is CustomApplicationException)
			{
				viewName = "ErrorCustom";
			}
			else if (ex is ArgumentException)
			{
				viewName = "ErrorArgument";
			}

			HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, currentController, currentActionName);
			ViewResult result = new ViewResult
			{
				ViewName = viewName,
				ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
				TempData = filterContext.Controller.TempData
			};

			filterContext.Result = result;
			filterContext.ExceptionHandled = true;

			base.OnException(filterContext);
		}

	}
}
