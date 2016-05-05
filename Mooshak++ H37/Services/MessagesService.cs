using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mooshak___H37.Services
{
	class MessagesService
	{
		private ApplicationDbContext _db;

		public MessagesService()
		{
			_db = new ApplicationDbContext();
		}


	}
}
