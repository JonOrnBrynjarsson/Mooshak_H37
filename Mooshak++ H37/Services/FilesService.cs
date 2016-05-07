using Mooshak___H37.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Mooshak___H37.Models.Entities;
using System.IO.Compression;
using System.Net;

namespace Mooshak___H37.Services
{
	public static class ZipFile { }

	class FilesService
	{
		private ApplicationDbContext _db;

		public FilesService()
		{
			_db = new ApplicationDbContext();
		}


		public bool SaveSubmissionfile(string file)
		{
			return true;
		}
		/// <summary>
		/// Returns the userName responsible for a submission.
		/// </summary>
		/// <param name="submissionID">The "ID" of  submission being tested</param>
		/// <returns></returns>
		public string getUserNameBySubmissionID(int submissionID)
		{
			string userName = (from x in _db.Submissions
				where x.ID == submissionID
				select x.User.AspNetUser.UserName).SingleOrDefault().ToString();

			return userName.SubstringUpToFirst('@');
		}
		/// <summary>
		/// Returns the folder that holds the zipfile for a submission.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <returns></returns>
		public string getStudentSubmissionFolder(int submissionId)
		{
			return ConfigurationManager.AppSettings["StudentFileLocation"] + 
			"/" + submissionId.ToString();
		}

		/// <summary>
		/// Returns the folder where the submission will be tested.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <returns></returns>
		public string getStudentRunFolder(int submissionId)
		{
			string userName = getUserNameBySubmissionID(submissionId);

			return ConfigurationManager.AppSettings["RunLocation"] +
			"/" + userName;
		}

		/// <summary>
		/// Returns the zipfile name an submission.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <returns></returns>
		public string getZipfileName(int submissionId)
		{
			return getUserNameBySubmissionID(submissionId) + 
				submissionId.ToString() + ".zip";
		}	

		/// <summary>
		/// Copies the zipfile to be tested to the run folder.  If this is the first time
		/// this user has submitted a solution to a Milestone a rundirectory is created for
		/// him.  Throws an exception if there is no file in the submission directory.
		/// </summary>
		/// <param name="submissionId" name>The "ID" of the submission being copied</param>
		/// /// <param name="userName" name>The "userName" of the user submitting</param>
		public void copyFileToRunFolder(int submissionId, string userName)
		{
			string origFile = getStudentSubmissionFolder(submissionId);

			if (Directory.Exists(origFile))
			{
				string runFolder = getStudentRunFolder(submissionId);
				string zipfile = origFile + getZipfileName(submissionId);

				if (!Directory.Exists(runFolder))
				{
					Directory.CreateDirectory(runFolder);
				}
				File.Copy(runFolder, origFile, true);
			}
			else
			{
				throw new FileNotFoundException();
			}
		}


		public void unCompressZipFile(int submissionId)
		{
			string runFolder = getStudentRunFolder(submissionId);
			string zipfile = getZipfileName(submissionId);
			DirectoryInfo di = new DirectoryInfo(runFolder);
			FileInfo fi = di.GetFiles(zipfile).FirstOrDefault();

			//ZipFile.ExtractToDirectory(runFolder, origFolder);
		}
		/// <summary>
		/// Compiles the C++ main.cpp file and all included files in the 
		/// runfolder for the submission.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being comiled</param>
		public void compileStudentProgram(int submissionId)
		{
			string runfolder = getStudentRunFolder(submissionId);
			string fileName = getUserNameBySubmissionID(submissionId);
			fileName += ".exe";

			DirectoryInfo di = new DirectoryInfo(runfolder);
			FileInfo fi = di.GetFiles("main.cpp").FirstOrDefault();
			if (fi != null)
			{
				compileProgram(runfolder, fileName);
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		/// <summary>
		/// A function to compile the teachers code into an executable file
		/// that can be used for comparison to the students submissions.
		/// </summary>
		/// <param name="milestoneId">The "ID" of the milestone to be compiled</param>
		public void compileTeacherProgram(int milestoneId)
		{
			string fileLocation = @ConfigurationManager.AppSettings["TeacherFileLocation"] 
					+ @"\" + milestoneId.ToString();
			string runfolder = @ConfigurationManager.AppSettings["RunLocation"] + @"\";
			runfolder += milestoneId.ToString();

			string fileName = runfolder + milestoneId.ToString() + ".exe";

			if (!Directory.Exists(runfolder))
			{
				Directory.CreateDirectory(runfolder);
			}

			//File.Copy(runfolder, origFile, true);
			DirectoryInfo dir = new DirectoryInfo(fileLocation);
			FileInfo fi = dir.GetFiles("main.cpp").FirstOrDefault();
			if (fi != null)
			{
				DirectoryInfo directory = new DirectoryInfo(runfolder);
				FileInfo[] files = directory.GetFiles("*.*");
				
				foreach (var file in files)
				{
					file.Delete();
				}

				compileProgram(fileLocation, fileName);
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
		/// <param name="FolderWithCodeFile">The folder of the file to be compiled</param>
		/// <param name="fullFileNameforCompiledFile">The filename of the compiled file with directory</param>
		public void compileProgram(string FolderWithCodeFile, string fullFileNameforCompiledFile)
		{
			string fileToCompile = @FolderWithCodeFile + @"\main.cpp";
			string Compiler = "mingw32-g++.exe";
			string all = fileToCompile + " -o " + fullFileNameforCompiledFile;
			//Console.WriteLine(all);

			Process process = new Process();
			process.StartInfo.FileName = Compiler;
			process.StartInfo.Arguments = all;
			process.StartInfo.UseShellExecute = false;
			//process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.Start();
			process.WaitForExit();

		}

		/// <summary>
		/// Returns the number of test cases related to this submission.
		/// </summary>
		/// <param name="submissionId"></param>
		/// <returns></returns>
		public List<int> getTestCases(int submissionId)
		{
			var testCases = (from t in _db.TestCases
				join m in _db.Milestones
					on t.MilestoneID equals m.ID
				join s in _db.Submissions
					on m.ID equals s.MilestoneID
				where s.ID == submissionId
				select t.ID);
				
			return testCases.ToList();
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
		/// Deletes all files from the test directory
		/// </summary>
		/// <param name="submissionId">The submission being tested</param>
		public void clearRunfolder(int submissionId)
		{
			string runfolder = getStudentRunFolder(submissionId);
			DirectoryInfo dir = new DirectoryInfo(runfolder);
			FileInfo[] files = dir.GetFiles("*.*");

			foreach (var file in files)
			{
				System.IO.File.Delete(file.ToString());
			}
		}

		/// <summary>
		/// Runs the students submission against a testCase.  Compares the result
		/// against what the theachers code produces.  Returns true if the results
		/// are the same. Otherwise returns false.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission being tested</param>
		/// <param name="testCase">The "ID" of the testcase being used to test the submission</param>
		/// <returns></returns>
		public bool runTest(int submissionId, string testCase)
		{
//Hvað með Errorhandling ef forrit keyrir ekki?
			Process process = new Process();
			string runfolder = getStudentRunFolder(submissionId);
			string filename = runfolder +
			                  getUserNameBySubmissionID(submissionId) +
			                  ".exe";
			process.StartInfo.FileName = @filename;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.Start();
//Hvað með Minnisvillur???
			StreamWriter myStreamWriter = process.StandardInput;
			
			myStreamWriter.WriteLine(testCase);

			StreamReader reader = process.StandardOutput;
			string output = reader.ReadLine();
			process.Close();


//Keyra saman við teacher...
			string comp = compareToTeacher(submissionId, testCase);
			if (output == comp)
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}

		public string compareToTeacher(int submissionId, string testCase)
		{
			int milestoneId = (from s in _db.Submissions
				where s.ID == submissionId
				select s.MilestoneID).SingleOrDefault();
			string programLocation = @ConfigurationManager.AppSettings["TeacherFileLocation"]
					+ @"\" + milestoneId;
			string filename = ConfigurationManager.AppSettings["RunLocation"] 
					+ milestoneId + ".exe";
			Process process = new Process();
			process.StartInfo.FileName = @filename;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.Start();
//Hvað með Minnisvillur???
			StreamWriter myStreamWriter = process.StandardInput;
			
			myStreamWriter.WriteLine(testCase);

			StreamReader reader = process.StandardOutput;
			string output = reader.ReadLine();
			process.Close();

			return output;
		}
		public void doingTestsOnSubmission(int submissionId)
		{
			List<int> testCases = getTestCases(submissionId);
			copyFileToRunFolder(submissionId, getUserNameBySubmissionID(submissionId));
			//þá afþjappa file / s í runfolder -bíður aðeins skil ekki zip dótið
			compileStudentProgram(submissionId);

			/* keyra forritið skv. fjölda testcase með nýju í hvert sinn
			 * Vista niðurstöður / fallið komið vantar upplýsingar
			 */
			 updateTestrun(submissionId,);
			 clearRunfolder(submissionId);

			return;
		}


		public void FileCompiler(int userId)
		{

			string fileToCompile = @"c:\temp\jontest\jon\main.cpp";
			string fileName = @"c:\temp\jontest\jon\jonob06.exe";
			string Compiler = "mingw32-g++.exe";
			string all = fileToCompile + " -o " + fileName;
			Console.WriteLine(all);

			Process process = new Process();
			process.StartInfo.FileName = Compiler;
			process.StartInfo.Arguments = all;
			process.StartInfo.UseShellExecute = false;
			//process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.Start();
			process.WaitForExit();
		}




	}
}
