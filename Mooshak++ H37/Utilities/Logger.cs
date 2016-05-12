using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Web.Mvc;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Project_4.Utilities
{
	class Logger
	{
		private readonly ApplicationDbContext _db;
		private readonly UsersService _usersService;

		public Logger()
		{
			_db = new ApplicationDbContext();
			_usersService = new UsersService();
		}

		public static Logger theInstance = null;
		public static Logger Instance
		{
			get
			{
				if (theInstance == null)
				{
					theInstance = new Logger();
				}
				return theInstance;
			}
		}

		public void LogException(Exception ex, string controller, string action )
		{
			string comment = string.Format("The action {0} in {1}Controller, threw an error \" {2}", action, controller,  ex.Message);
			ErrorReport report = new ErrorReport();
			report.UserID = _usersService.getUserIdForCurrentApplicationUser();
			report._message = comment;
			report.DateOccurred = DateTime.Now;

			_db.ErrorReports.Add(report);
			_db.SaveChanges();
		}
	}
}
