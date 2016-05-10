using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc.Html;
using Microsoft.Ajax.Utilities;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Web;
using Microsoft.AspNet.Identity;

namespace Mooshak___H37.Services
{
	class AssigmentsService
	{
		private readonly ApplicationDbContext _db;

		public AssigmentsService()
		{
			_db = new ApplicationDbContext();
		}

		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}

		public List<AssignmentViewModel> getAllAssignments()
		{
			var currUsId = GetCurrentUser();

			var userCourses = (from uscr in _db.UserCourseRelations
							   where currUsId == uscr.UserID
							   select uscr.CourseID).ToList();


			var assignments = (from assi in _db.Assignments
							   where userCourses.Contains(assi.CourseID) &&
								assi.IsRemoved != true
							   orderby assi.DueDate descending
							   select assi).ToList();

			var viewModel = new List<AssignmentViewModel>();

			foreach (var assignm in assignments)
			{
				AssignmentViewModel model = new AssignmentViewModel
				{
					ID = assignm.ID,
					Name = assignm.Name,
					SetDate = assignm.SetDate,
					DueDate = assignm.DueDate,
					CourseID = assignm.CourseID,
					IsActive = assignm.IsActive,
					IsRemoved = assignm.IsRemoved,
					Description = assignm.Description,
				};
				viewModel.Add(model);
			}

			return viewModel;
		}


		public AssignmentViewModel Assignment(int id)
		{
			var assignment = (from asi in _db.Assignments
							where asi.ID == id
							select asi).FirstOrDefault();

			if (assignment == null)
			{
				//DO SOMETHING
				//throw new exception / skila NULL(ekki skila null hér)
			}

			var userID = GetCurrentUser();

			var milestones = _db.Milestones.Where(x => x.AssignmentID == id &&
												  x.IsRemoved != true)
				.Select(x => new MilestoneViewmodel
				{
					ID = x.ID,
					AllowedSubmissions = x.AllowedSubmissions,
					Name = x.Name,
					Description = x.Description,
					AssignmentID = x.AssignmentID,
					IsRemoved = x.IsRemoved,
					Percentage = x.Percentage,
					UserSubmissions = (from s in _db.Submissions
									   where s.UserID == userID && s.MilestoneID == x.ID
									   select s.ID).Count()
				}).ToList();

			var coursename = (from asi in _db.Courses
								where asi.ID == assignment.CourseID
								select asi).FirstOrDefault();

			//
			var userid = (from usr in _db.UserCourseRelations
								where  
								assignment.CourseID == usr.CourseID
								select usr.UserID).ToList();


			var users = (from user in _db.Users
					  where userid.Contains(user.ID)
					  select user.Name).Select(x => new UserViewModel
					  {
						  Name = x
					  }).ToList();

			if (coursename == null)
			{
				throw new Exception("ITS NULL BITCH");
			}

			AssignmentViewModel model = new AssignmentViewModel
			{
				ID = assignment.ID,
				Name = assignment.Name,
				SetDate = assignment.SetDate,
				DueDate = assignment.DueDate,
				CourseID = assignment.CourseID,
				IsActive = assignment.IsActive,
				IsRemoved = assignment.IsRemoved,
				Description = assignment.Description,
				Milestones = milestones,
				CourseName = coursename.Name,
				Users = users
			}; 

			return model;
		}


		public List<CourseViewModel> GetCourseAssignments()
		{
			var currUsId = GetCurrentUser();

			var userCourses = (from uscr in _db.UserCourseRelations
							   where currUsId == uscr.UserID
							   select uscr.CourseID).ToList();


			var assignments = (from assi in _db.Assignments
							   where userCourses.Contains(assi.CourseID)
							   orderby assi.DueDate descending
							   select assi).ToList();


			var coursese = (from crse in _db.Courses
							where userCourses.Contains(crse.ID)
							select crse)
			.Select(x => new CourseViewModel
			{
				ID = x.ID,
				Name = x.Name,
				Isactive = x.Isactive,
				IsRemoved = x.IsRemoved,
				StartDate = x.Startdate,
				Assignments = getAssignmentsInCourse(x.ID),
			}).ToList();

			var viewModel = new List<CourseViewModel>();

			foreach (var course in coursese)
			{
				CourseViewModel model = new CourseViewModel
				{
					Assignments = course.Assignments,
					ID = course.ID,
					Isactive = course.Isactive,
					IsRemoved = course.IsRemoved,
					Name = course.Name,
					StartDate = course.StartDate,
				};
				viewModel.Add(model);
			}

			return viewModel;

		}


		public int GetAssignmentIDFromMilestoneID(int milestoneID)
		{

			var assignmentID = (from mil in _db.Milestones
							  where mil.ID == milestoneID
							  select mil.AssignmentID).FirstOrDefault();

			return assignmentID;
		}

		/// <summary>
		/// Gets the number of submissions from a user to a specific milestone.
		/// </summary>
		/// <param name="milestoneId">The milestone "ID"</param>
		/// <param name="userId">The user "ID"</param>
		/// <returns>Number of submissions</returns>
		//public int getNumOfSubmissions(int milestoneId, int userId)
		//{
		//	return (from s in _db.Submissions
		//		where s.ID == userId && s.MilestoneID == milestoneId
		//		select s.ID).Count();
		//}



		internal void RemoveAssignment(AssignmentViewModel model)
		{
			var assignment = (from assign in _db.Assignments
							 where assign.ID == model.ID
							 select assign).FirstOrDefault();

			if (assignment == null)
			{
				//DO SOMEHTING
			}

			assignment.IsRemoved = true;
			_db.SaveChanges();
		}

		public List<AssignmentViewModel> getAssignmentsInCourse(int id)
		{
			var assignments = (from asi in _db.Assignments
							  where asi.CourseID == id
							  select asi).ToList();

			if(assignments == null)
			{
				//TODO
				//Throw exception
				//return null;
			}

			List<AssignmentViewModel> viewModel = new List<AssignmentViewModel>();
			
			foreach(var assignm in assignments)
			{
				AssignmentViewModel model = new AssignmentViewModel
				{
					ID = assignm.ID,
					Name = assignm.Name,
					SetDate = assignm.SetDate,
					DueDate = assignm.DueDate,
					CourseID = assignm.CourseID,
					IsActive = assignm.IsActive,
					IsRemoved = assignm.IsRemoved,
					Description = assignm.Description,
				};
				viewModel.Add(model);
			}
			
			return viewModel;
		}

		internal void CreateAssignment(AssignmentViewModel model)
		{
			_db.Assignments.Add(new Assignment
			{
				Name = model.Name,
				CourseID = model.CourseID,
				Description = model.Description,
				DueDate = model.DueDate,
				ID = model.ID,
				IsActive = model.IsActive,
				IsRemoved = model.IsRemoved,
				SetDate = model.SetDate,
			});
			_db.SaveChanges();
		}

		internal void EditAssignment(AssignmentViewModel model)
		{
			var edit = (from assign in _db.Assignments
						where assign.ID == model.ID
						select assign).FirstOrDefault();

			if(edit != null)
			{
				edit.Name = model.Name;
				edit.SetDate = model.SetDate;
				edit.DueDate = model.DueDate;
				edit.Description = model.Description;
				edit.ID = model.ID;
				edit.IsActive = model.IsActive;
				edit.IsRemoved = model.IsRemoved;

				_db.SaveChanges();
			}
			else
			{
				// DO Something!!
			}
		}

		public bool SaveSubmissionfile(string file)
		{
			return true;
		}

		public void RunTest(int SubmissionId)
		{
			
		}

		private static void FileCompiler(int submissionId)
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
