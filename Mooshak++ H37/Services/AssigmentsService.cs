using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mooshak___H37.Models;

namespace Mooshak___H37.Services
{
	class AssigmentsService
	{
		private ApplicationDbContext _db;

		public AssigmentsService()
		{
			_db = new ApplicationDbContext();
		}

	}
}
