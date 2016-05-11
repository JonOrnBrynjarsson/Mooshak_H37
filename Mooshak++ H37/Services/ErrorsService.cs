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

		void convertUserIdToUserName(List<AdminErrorViewmModel> model)
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
