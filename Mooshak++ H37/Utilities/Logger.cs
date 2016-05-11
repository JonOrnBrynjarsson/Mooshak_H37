using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Web.Mvc;

namespace Project_4.Utilities
{
	class Logger
	{
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
			
			string Logfile = ConfigurationManager.AppSettings["Logfile"];
			string LogPath = ConfigurationManager.AppSettings["LogPath"];
			if (!Directory.Exists(LogPath))
			{
				Directory.CreateDirectory(LogPath);
			}

			string message = string.Format("At {0}, the action {1} in {2}Controller, threw an error \" {3} \" {4}", DateTime.Now, action, controller,  ex.Message, Environment.NewLine);
			
			using (StreamWriter fileToUse = new StreamWriter(LogPath + Logfile, true, Encoding.Default))
			{
				fileToUse.Write(message);
			}

		}



	}
}
