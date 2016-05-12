using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Mooshak___H37.Models.Entities;
using System.Web;

namespace Mooshak___H37.Services
{
	class FilesService
	{
		private readonly ApplicationDbContext _db;
		private readonly UsersService _usersService;

		public FilesService()
		{
			_db = new ApplicationDbContext();
			_usersService = new UsersService();
		}

		/// <summary>
		/// Saves the file submitted to a submission folder for that submission.
		/// </summary>
		/// <param name="file">The code submitted</param>
		/// <param name="submissionId">The "ID" of the submission.</param>
		public void saveSubmissionfile(HttpPostedFileBase file, int submissionId)
		{
			string filePath = getStudentSubmissionFolder(submissionId);
			
			filePath += file.FileName;
			file.SaveAs(filePath);	
		}

		/// <summary>
		/// Saves the string codefile to a submission folder
		/// </summary>
		/// <param name="codefile">The string to be saved</param>
		/// <param name="submissionId">The "ID" of the submission</param>
		public void saveSubmissionfile(string codefile, int submissionId)
		{
			string filePath = getStudentSubmissionFolder(submissionId) +
			                  @"\main.cpp";				  
			File.Create(filePath);
		}

		/// <summary>
		/// Saves a submission to the database.
		/// </summary>
		/// <param name="milestonedId">The "ID" of the milestone that is being worked on</param>
		/// <returns>The "ID" of the submission</returns>
		public int createSubmission(int milestonedId)
		{
			if (milestonedId > 0)
			{
				try
				{
					Submission submission = new Submission
					{
						//ID = 1,
						MilestoneID = milestonedId,
						UserID = _usersService.getUserIdForCurrentApplicationUser(),
						ProgramFileLocation = "a",
						//Grade = 0,
						IsGraded = false,
						FinalSolution = false,
						DateSubmitted = DateTime.Now

					};
					_db.Submissions.Add(submission);
					_db.SaveChanges();

					submission.ProgramFileLocation = getStudentSubmissionFolder(submission.ID);
					_db.SaveChanges();
					return submission.ID;
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex);
				}
			}
			return 0;
		}

		/// <summary>
		/// Gets the userName responsible for a submission.
		/// </summary>
		/// <param name="submissionId">The "ID" of  submission being tested</param>
		/// <returns>The username for a specific submission</returns>
		public string getUserNameBySubmissionId(int submissionId)
		{
			string userName = (from x in _db.Submissions
				where x.ID == submissionId
				select x.User.AspNetUser.UserName).SingleOrDefault();
			if (!String.IsNullOrEmpty(userName))
			{
				return userName.SubstringUpToFirst('@');
			}
			return null;
		}

		/// <summary>
		/// Gets the folder that holds the code for a submission.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <returns>A string reprsenting the folder for the student submission</returns>
		public string getStudentSubmissionFolder(int submissionId)
		{
			string folder = @ConfigurationManager.AppSettings["StudentFileLocation"] + 
				submissionId.ToString() + @"\";

			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return folder;
		}

		/// <summary>
		/// Returns the folder where the submission will be tested.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <returns></returns>
		public string getStudentRunFolder(int submissionId)
		{
			string userName = getUserNameBySubmissionId(submissionId);

			string runfolder = @ConfigurationManager.AppSettings["RunLocation"].ToString() +
			                   userName;// + @"\";

			if (!Directory.Exists(runfolder))
			{
				Directory.CreateDirectory(runfolder);
			}
			return runfolder;
		}
		
		/// <summary>
		/// Compiles a submission from student to a running folder.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission to be compiled</param>
		public void compileStudentProgram(int submissionId)
		{
			String runfolder = getStudentRunFolder(submissionId);
			string fileName = runfolder + @"\" +
				getUserNameBySubmissionId(submissionId) +
				".exe";
			string filefolder = getStudentSubmissionFolder(submissionId);
			DirectoryInfo di = new DirectoryInfo(filefolder);
			FileInfo fi = di.GetFiles("*.cpp").FirstOrDefault();
			if (fi != null)
			{
				compileProgram(filefolder +  fi, fileName);
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		/// <summary>
		/// Compiles the code to an executable file.  If the file is to be in a different directory
		/// from the one that holds the code, that directory has to be included in the name
		/// sent into the function.
		/// </summary>
		/// <param name="folderWithCodeFile">The folder of the file to be compiled</param>
		/// <param name="fullFileNameforCompiledFile">The filename of the compiled file with directory</param>
		public void compileProgram(string folderWithCodeFile, string fullFileNameforCompiledFile)
		{
			string fileToCompile = @folderWithCodeFile;
			string Compiler = "mingw32-g++.exe";
			string all = fileToCompile + " -o " + fullFileNameforCompiledFile;
			
			Process process = new Process();
			process.StartInfo.FileName = Compiler;
			process.StartInfo.Arguments = all;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			StreamReader errorReader = process.StandardError;
			string output = errorReader.ReadToEnd();
			process.WaitForExit();
			process.Close();
		}

		/// <summary>
		/// Returns the "ID" of the test cases related to the submission ID sent in.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested.</param>
		/// <returns>A list of testcase IDs</returns>
		public List<int> getTestCases(int submissionId)
		{
			var testCases = (from t in _db.TestCases
				join m in _db.Milestones
					on t.MilestoneID equals m.ID
				join s in _db.Submissions
					on m.ID equals s.MilestoneID
				where s.ID == submissionId
				      && t.IsRemoved == false 
				select t.ID);
				
			return testCases.ToList();
		}

		/// <summary>
		/// Gets the string with the input for a specific testrun.
		/// </summary>
		/// <param name="testCaseId">The "ID" of the testcase</param>
		/// <returns>The input string for a testrun</returns>
		public string getATestCaseInput(int testCaseId)
		{
			return (from t in _db.TestCases
				where t.ID == testCaseId
					&& t.IsRemoved == false
				select t.Inputstring).SingleOrDefault();
		}

		/// <summary>
		/// Gets the string with the expected output for a specific testrun.
		/// </summary>
		/// <param name="testCaseId">The "ID" of the testcase</param>
		/// <returns>A string with the expected output of the testcase</returns>
		public string getATestCaseOutput(int testCaseId)
		{
			return (from t in _db.TestCases
					where t.ID == testCaseId
						&& t.IsRemoved == false
					select t.Outputstring).SingleOrDefault();
		}

		/// <summary>
		/// Saves the result from the current testrun to the database
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <param name="testCase">The "ID" of the testcase used in this testrun</param>
		/// <param name="hasPassed">True if testrun is successful</param>
		/// <param name="comments">Comments about the current testrun</param>
		public void updateTestrun(int submissionId, int testCase, bool hasPassed, string comments)
		{
			Testrun testrun = new Testrun();
			testrun.SubmissionID = submissionId;
			testrun.IsSuccess = hasPassed;
			testrun.ResultComments = comments;
			testrun.TestCase = testCase;

			_db.Testruns.Add(testrun);
			_db.SaveChanges();
		}

		/// <summary>
		/// Deletes all files from the test directory.
		/// </summary>
		/// <param name="submissionId">The submission being tested</param>
		public void clearRunfolder(int submissionId)
		{
			string runfolder = getStudentRunFolder(submissionId);
			DirectoryInfo dir = new DirectoryInfo(runfolder);
			FileInfo[] files = dir.GetFiles("*.*");

			foreach (var file in files)
			{
				try
				{
					string deleteFile = runfolder + @"\" + file.ToString();
					System.IO.File.Delete(deleteFile);
				}
				catch (Exception ex)
				{
					System.Console.WriteLine(ex);
				}
			}
		}

		/// <summary>
		/// Runs the students submission against a testCase.  Compares the result
		/// against expected output.  Returns true if the results
		/// are the same. Otherwise returns false.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <param name="testCase">The input for the test run</param>
		/// <param name="compareTo">The expected output from the test run </param>
		/// <returns>bool = true if testrun is successful</returns>
		public bool runTest(int submissionId, string testCase, string compareTo)
		{
			Process process = new Process();
			string runfolder = getStudentRunFolder(submissionId);
			string filename = runfolder + @"\" +
			                  getUserNameBySubmissionId(submissionId) +
			                  ".exe";
			process.StartInfo.FileName = @filename;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();

			StreamWriter myStreamWriter = process.StandardInput;
			if (!string.IsNullOrEmpty(testCase))
			{
				myStreamWriter.WriteLine(testCase);
			}

			StreamReader reader = process.StandardOutput;
			StreamReader errorReader = process.StandardError;
			string output = reader.ReadLine();
			string error = errorReader.ReadToEnd();
			process.Close();

			if (output == compareTo)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the Milestone ID from the database based on a specific submission ID.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission for the milestone</param>
		/// <returns>The "ID" of the milestone for the submission sent in</returns>
		public int getMilestoneIdBySubmitId(int submissionId)
		{
			int milestoneId = (from s in _db.Submissions
							   where s.ID == submissionId
							   select s.MilestoneID).SingleOrDefault();
			return milestoneId;
		}

		/// <summary>
		/// The main functions in testing a submission.  This one calles different functions
		/// as needed.  
		/// </summary>
		/// <param name="submissionId">The "Id" of the submission being tested</param>
		public void testingSubmission(int submissionId)
		{
			List<int> testCases = getTestCases(submissionId);
			compileStudentProgram(submissionId);

			foreach (var test in testCases)
			{
				string input = getATestCaseInput(test);
				string output = getATestCaseOutput(test);
		
				if (runTest(submissionId, input, output))
				{
					updateTestrun(submissionId, test, true, "LATER");
				}
				else
				{
					updateTestrun(submissionId, test, false, "Villa");
				}
			}
			clearRunfolder(submissionId);
		}

		/// <summary>
		/// Gets the program code for a submission
		/// </summary>
		/// <param name="submissionId">The submission ID for the submission</param>
		/// <returns>The program code as a string</returns>
		public string getSubmissionFile(int submissionId)
		{
			string folder = getStudentSubmissionFolder(submissionId);
			DirectoryInfo dir = new DirectoryInfo(folder);
			FileInfo[] files = dir.GetFiles("*.cpp");
			if (files[0] == null)
			{
				throw new FileNotFoundException();
			}
			string codefile = folder + @"\" + files[0];
			string code = File.ReadAllText(codefile);

			return code;
		}
	}
}
