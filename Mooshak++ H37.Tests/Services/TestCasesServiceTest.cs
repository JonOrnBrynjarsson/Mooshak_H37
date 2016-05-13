using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using Mooshak___H37.Services;

namespace Mooshak___H37.Tests.Services
{
	[TestClass]
	public class TestCasesServiceTest
	{
		private TestCaseService _testCaseService;

		[TestInitialize]
		public void Initialize()
		{
			// Set up our mock database. In this case,
			// we only have to worry about one table
			// with 3 records:
			var mockDb = new MockDataContext();

			#region Mock Testcases


			var t1 = new TestCase()
			{
				ID = 1,
				Inputstring = "1 3",
				MilestoneID = 10,
				IsOnlyForTeacher = false,
				IsRemoved = false,
				Outputstring = "4"
			};
			mockDb.TestCases.Add(t1);
			var t2 = new TestCase()
			{
				ID = 2,
				Inputstring = "7 9",
				MilestoneID = 10,
				IsOnlyForTeacher = false,
				IsRemoved = false,
				Outputstring = "27"
			};
			mockDb.TestCases.Add(t2);
			var t3 = new TestCase()
			{
				ID = 3,
				Inputstring = "0 0 0",
				MilestoneID = 11,
				IsOnlyForTeacher = false,
				IsRemoved = true,
				Outputstring = "7"
			};
			mockDb.TestCases.Add(t3);
			var t4 = new TestCase()
			{
				ID = 4,
				Inputstring = "0 0 0",
				MilestoneID = 11,
				IsOnlyForTeacher = false,
				IsRemoved = false,
				Outputstring = "70"
			};
			mockDb.TestCases.Add(t4);
			var t5 = new TestCase()
			{
				ID = 5,
				Inputstring = "",
				MilestoneID = 12,
				IsOnlyForTeacher = false,
				IsRemoved = false,
				Outputstring = "Hello World!"
			};
			mockDb.TestCases.Add(t5);

			#endregion

			// Note: you only have to add data necessary for this
			// particular service (FriendService) to run properly.
			// There will be more tables in your DB, but you only
			// need to provide the data for the methods you are
			// actually testing here.

			_testCaseService = new TestCaseService(mockDb);
		}

		[TestMethod]
		public void getASingleTestCase()
		{
			// Arrange:
			const int testcaseId = 1;
		
			// Act:
			var tc = _testCaseService.getSingleTestCase(testcaseId);
			// Assert: 
			Assert.AreEqual(tc.ID, testcaseId);
		}
	
		[TestMethod]
		public void getNumberOfTestCases()
		{
			// Arrange:
			const int num = 4;
			// Act:
			int result = _testCaseService.numberOfTestCases();
			// Assert: 
			Assert.AreEqual(result, num);
		}



		[TestMethod]
		public void insertAIlllegalTestCase()
		{
			// Arrange:
			const int num = 4;
			TestCaseViewModel tcVm = new TestCaseViewModel()
			{
				ID = 0 ,
				Inputstring = "hey",
				Outputstring = "Jú"
			};
			// Act:
			_testCaseService.createTestCase(tcVm, 13);
			var tc = _testCaseService.numberOfTestCases();
			// Assert: 
			Assert.AreEqual(tc, num);
		}

		[TestMethod]
		public void insertALegalTestCase()
		{
			// Arrange:
			const int num = 5;
			TestCaseViewModel tcVm = new TestCaseViewModel()
			{
				ID = 6,
				Inputstring = "hana nú",
				MilestoneID = 13,
				Outputstring = "hérna hér"
			};
			// Act:
			_testCaseService.createTestCase(tcVm, 13);
			var tc = _testCaseService.numberOfTestCases();
			// Assert: 
			Assert.AreEqual(tc, num);

		}


		[TestMethod]
		public void getTestCaseForMilestone()
		{
			// Arrange:
			const int milestone1 = 10;
			const int milestone2 = 12;
			const int num1 = 2;
			const int num2 = 1;

			// Act:
			var list1 = _testCaseService.getTestCasesVMForMilestone(milestone1);
			var list2 = _testCaseService.getTestCasesVMForMilestone(milestone2);
			
			// Assert: 
			Assert.AreEqual(list1.Count, num1);
			Assert.AreEqual(list2.Count, num2);
		}


		[TestMethod]
		public void getATestCaseOutput()
		{
			// Arrange:
			const int testcase1 = 1;
			const int testcase2 = 2;
			const int testcase3 = 3;
			const string i1 = "4";
			const string i2 = "27";
			const string i3 = "7";

			// Act:
			string result1 = _testCaseService.getATestCaseOutput(testcase1);
			string result2 = _testCaseService.getATestCaseOutput(testcase2);
			string result3 = _testCaseService.getATestCaseOutput(testcase3);

			// Assert: 
			Assert.AreEqual(result1, i1);
			Assert.AreEqual(result2, i2);
			Assert.IsNull(result3);
		}


		[TestMethod]
		public void getATestCaseInput()
		{
			// Arrange:
			const int testcase1 = 1;
			const int testcase2 = 2;
			const int testcase3 = 3;
			const string i1 = "1 3";
			const string i2 = "7 9";
			const string i3 = "0 0 0";

			// Act:
			string result1 = _testCaseService.getATestCaseInput(testcase1);
			string result2 = _testCaseService.getATestCaseInput(testcase2);
			string result3 = _testCaseService.getATestCaseInput(testcase3);

			// Assert: 
			Assert.AreEqual(result1, i1);
			Assert.AreEqual(result2, i2);
			Assert.IsNull(result3);
		}
	}
}
