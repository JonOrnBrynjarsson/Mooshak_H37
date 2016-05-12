﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			var f1 = new User()
			{
				ID = 1,
				Name = "John Doe",
				AspNetUserId = "23olrkj9-sdfsi",
				IsRemoved = true
			};
			mockDb.User.Add(f1);

			var f2 = new User
			{
				ID = 2,
				Name = "Jón Gunnarsson",
				AspNetUserId = "23odsdflrkj9-sdfsi",
				IsRemoved = false
			};
			mockDb.User.Add(f2);
			var f3 = new User
			{
				ID = 3,
				Name = "Gunna Jónsdóttir",
				AspNetUserId = "d3olrkj9-sdfsi",
				IsRemoved = false
			};
			mockDb.User.Add(f3);

			// Note: you only have to add data necessary for this
			// particular service (FriendService) to run properly.
			// There will be more tables in your DB, but you only
			// need to provide the data for the methods you are
			// actually testing here.

			_usersService = new UsersService(mockDb);
		}

		[TestMethod]
		public void TestGetAllFriendsForDabs()
		{
			// Arrange:
			const string userName = "dabs";

			// Act:
			var friends = _userService.GetFriendsFor(userName);

			// Assert:
			Assert.AreEqual(2, friends.Count);
			foreach (var item in friends)
			{
				Assert.AreNotEqual(item, "dabs");
			}
		}

		[TestMethod]
		public void TestGetForUserWithNoFriends()
		{
			// Arrange:
			const string userWithNoFriends = "loner";
			// Note: no user with this username has an entry
			// in our test data.

			// Act:
			var friends = _userService.GetFriendsFor(userWithNoFriends);

			// Assert:
			Assert.AreEqual(0, friends.Count);
		}
	}
}
