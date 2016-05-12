using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;
using Mooshak___H37.Models.Viewmodels;
using Mooshak___H37.Services.Viewmodels;

namespace Mooshak___H37.Services
{
	public class ErrorsService
	{
		private readonly ApplicationDbContext _db;
		private readonly UsersService _usersService;

		public ErrorsService()
		{
			_db = new ApplicationDbContext();
			_usersService = new UsersService();
		}

		/// <summary>
		/// Gets the 20 most recent error messages ot of the ErrorReports table
		/// fills it in a list of AdminErrorViewModel wich has a date, message, and username 
		/// </summary>
		public List<AdminErrorViewmModel> getTopErrormessages()
		{
			
			var errorMessage = (from error in _db.ErrorReports
				orderby error.DateOccurred descending
				select error).Take(20);

			var viewModel = new List<AdminErrorViewmModel>();

			foreach (var message in errorMessage)
			{
				AdminErrorViewmModel model = new AdminErrorViewmModel
				{
					Date = message.DateOccurred,
					Message = message._message,
					User = message.UserID.ToString()
				};
				viewModel.Add(model);
			}

			convertUserIdToUserName(viewModel);

			return viewModel;
		}

		/// <summary>
		/// converts the User prop in AdminErrorViewmodel from being just the id of the user
		/// to giving it the username 
		/// </summary>
		/// <param name="model">This List will be updated once the function is done. (will have names instead of id's)</param>
		private void convertUserIdToUserName(List<AdminErrorViewmModel> model)
		{
			int val = 0;
			bool suc;
			foreach(var item in model)
			{
				suc = Int32.TryParse(item.User, out val);
				item.User = _usersService.getUserNameByID(val);
			}
		}

	}
}
