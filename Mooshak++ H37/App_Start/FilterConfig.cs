﻿using System.Web;
using System.Web.Mvc;
using Project_4.Utilities;

namespace Mooshak___H37
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new HandleErrorAttribute());
			filters.Add(new CustomHandleExceptionAttribute());
		}
	}
}
