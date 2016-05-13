using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	[TestClass]
	public class UsersServiceTest
	{
		private UsersService _usersService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();
			
			#region Mock Users

			var f1 = new User()
			{
				ID = 1,
				Name = "John Doe",
				AspNetUserId = "23olrkj9-sdfsi",
				IsRemoved = true
			};
			mockDb.Users.Add(f1);

			var f2 = new User
			{
				ID = 2,
				Name = "Jón Gunnarsson",
				AspNetUserId = "23odsdflrkj9-sdfsi",
				IsRemoved = false
			};
			mockDb.Users.Add(f2);
			var f3 = new User
			{
				ID = 3,
				Name = "Gunna Jónsdóttir",
				AspNetUserId = "d3olrkj9-sdfsi",
				IsRemoved = false
			};
			mockDb.Users.Add(f3);
			var f4 = new User
			{
				ID = 4,
				Name = "Niemand",
				AspNetUserId = "a-sdfsi",
				IsRemoved = true
			};
			mockDb.Users.Add(f4);
			var f5 = new User
			{
				ID = 5,
				Name = "Stafróf málsgrein",
				AspNetUserId = "d3olrs-sdfsi",
				IsRemoved = false
			};
			mockDb.Users.Add(f5);

			#endregion
			
			_usersService = new UsersService(mockDb);
		}

		[TestMethod]
		public void getAllActiveUsersInSystem()
		{
			// Arrange:
			int numOfActiveUsers = 3;
			// Act:
			var result = _usersService.UsersInSystem();
			// Assert:
			Assert.AreEqual(numOfActiveUsers, result.Count);
		}
	



	}
}
