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
	public class FilesService
	{
		private readonly IAppDataContext _db;
		private readonly UsersService _usersService;
		private readonly TestCaseService _testCaseService;

		public FilesService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
			_testCaseService = new TestCaseService(null);
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
			string runfolder = @ConfigurationManager.AppSettings["RunLocation"] +
			                   userName;

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
				string errorMsg = compileProgram(filefolder +  fi, fileName);
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
		public string compileProgram(string folderWithCodeFile, string fullFileNameforCompiledFile)
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
			return output;
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
		/// The main functions in testing a submission.  This one calles different functions
		/// as needed.  
		/// </summary>
		/// <param name="submissionId">The "Id" of the submission being tested</param>
		public void testingSubmission(int submissionId)
		{
			List<int> testCases = _testCaseService.getTestCases(submissionId);
			compileStudentProgram(submissionId);

			foreach (var test in testCases)
			{
				string input = _testCaseService.getATestCaseInput(test);
				string output = _testCaseService.getATestCaseOutput(test);
		
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
