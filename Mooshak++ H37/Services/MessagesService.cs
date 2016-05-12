using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mooshak___H37.Services
{
	public class MessagesService
	{
		private readonly IAppDataContext _db;

		public MessagesService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
		}


	}
}
