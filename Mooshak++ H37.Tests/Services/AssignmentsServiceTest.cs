using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	class AssignmentsServiceTest
	{
		private AssigmentsService _assigmentsService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();

			// Note: you only have to add data necessary for this
			// particular service (FriendService) to run properly.
			// There will be more tables in your DB, but you only
			// need to provide the data for the methods you are
			// actually testing here.

			_assigmentsService = new AssigmentsService(mockDb);
		}
	}
}
