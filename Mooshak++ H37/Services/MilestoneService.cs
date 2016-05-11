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
using Microsoft.AspNet.Identity;

namespace Mooshak___H37.Services
{
	public class MilestoneService
	{
		private readonly ApplicationDbContext _db;

		public MilestoneService()
		{
			_db = new ApplicationDbContext();
		}

		/// <summary>
		/// Finds ID of Current User
		/// </summary>
		/// <returns>ID of LOGGED IN User</returns>
		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser &&
							  x.IsRemoved != true
							  select x.ID).FirstOrDefault();

			if (currUserID == null)
			{
				throw new Exception("User does not exist or has been removed.");
			}

			return currUserID;
		}

		//Creates Milestone with Given Milestone Model
		internal void CreateMilestone(MilestoneViewmodel model, int assigID)
		{
			_db.Milestones.Add(new Milestone
			{
				AllowedSubmissions = model.AllowedSubmissions,
				AssignmentID = assigID,
				Description = model.Description,
				ID = model.ID,
				IsRemoved = model.IsRemoved,
				Name = model.Name,
				Percentage = model.Percentage,
			});
			_db.SaveChanges();
		}
		/// <summary>
		/// Finds Milestones for given Assignment ID, that are not
		/// Marked as removed. 
		/// </summary>
		/// <param name="assigID"></param>
		/// <returns>List Of Milestones</returns>
		public List<MilestoneViewmodel> GetMilestonesForAssignment(int assigID)
		{
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID
							  where miles.AssignmentID == assigID && 
							  miles.IsRemoved != true
							  select miles).ToList();

			var viewModel = new List<MilestoneViewmodel>();

			//Creates list of View Models for Milestone.
			foreach (var mil in milestones)
			{
				MilestoneViewmodel model = new MilestoneViewmodel
				{
					AssignmentID = mil.AssignmentID,
					Description = mil.Description,
					AllowedSubmissions = mil.AllowedSubmissions,
					ID = mil.ID,
					IsRemoved = mil.IsRemoved,
					Name = mil.Name,
					Percentage = mil.Percentage,
				};
				viewModel.Add(model);
			}

			return viewModel;
		}

		/// <summary>
		/// Finds Sum of all Milestone Percentages in Given Assignment.
		/// </summary>
		/// <param name="assigID"></param>
		/// <returns>Percentage of All Milestones in Given Assignment</returns>
		public double GetTotalMilestonePercentageForAssignment(int assigID)
		{
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID
							  where miles.AssignmentID == assigID &&
							  miles.IsRemoved != true
							  select miles).ToList();

			if (milestones == null)
			{
				//No milestones have been created
			}

			var viewModel = new List<MilestoneViewmodel>();

			double totalPercentage = 0;

			//Calculates total percentage
			foreach (var mil in milestones)
			{
				totalPercentage += mil.Percentage;
			}

			return totalPercentage;
		}

		/// <summary>
		/// Checks if User Can Still submit a Milestone, Depending on
		/// How many times he has already submitted
		/// </summary>
		/// <param name="milestoneID"></param>
		/// <returns>True or False</returns>
		public bool UserCanSubmitMilestone(int milestoneID)
		{
			//Returns ID of current User
			var currUser = GetCurrentUser();

			//Finds all submissions from Current User
			var submissions = (from s in _db.Submissions
							   where s.UserID == currUser && s.MilestoneID == milestoneID
							   && s.IsRemoved != true
							   select s.ID).Count();

			//Finds how often a User Can submit
			var allowedSubmissions = (from x in _db.Milestones
									  where x.ID == milestoneID
									  && x.IsRemoved != true
									  select x.AllowedSubmissions).SingleOrDefault();

			//Returns true if User has not submitted to many times
			if (submissions >= allowedSubmissions)
			{
				//return true;
				throw new Exception("You have already submitted the maximum number of times");
			}

			return true;
		}

		/// <summary>
		/// Checks if Current Milestones in Assignment with the New Milestone that is
		/// Being created Exceed 100%
		/// </summary>
		/// <param name="model"></param>
		/// <param name="assignmentID"></param>
		/// <returns>True or False</returns>
		public bool TeacherCanCreateMilestone(MilestoneViewmodel model, int assignmentID)
		{
			//Current Percentage of Milestones in Assignment
			var totalPercentage = GetTotalMilestonePercentageForAssignment(assignmentID);

			//Percentage of Milestone that is being created
			var milestonePercentage = model.Percentage;

			//Percentage that will be after Creation
			var currPercentage = totalPercentage + milestonePercentage;

			//Returns false if it would exceed 100%
			if (currPercentage > 100)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Finds Milestone with Given Milestone ID
		/// </summary>
		/// <param name="milID"></param>
		/// <returns>Milestone</returns>
		public MilestoneViewmodel GetSingleMilestone(int milID)
		{
			//Finds Milestone that is associated with given Milestone ID
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID 
							  where miles.ID == milID &&
							  miles.IsRemoved != true
							  select miles).FirstOrDefault();

			if (milestones == null)
			{
				throw new Exception("Milestone does not exist or has been removed.");
			}

			MilestoneViewmodel model = new MilestoneViewmodel
			{
				ID = milestones.ID,
				AllowedSubmissions = milestones.AllowedSubmissions,
				AssignmentID = milestones.AssignmentID,
				Description = milestones.Description,
				IsRemoved = milestones.IsRemoved,
				Name = milestones.Name,
				Percentage = milestones.Percentage,
			};

			return model;
		}

		//Edits Milestone with given Model and Id.
		internal void EditMilestone(MilestoneViewmodel model, int milestoneID)
		{
			var edit = (from mil in _db.Milestones where mil.ID == 
						milestoneID
						&& mil.IsRemoved != true
						select mil).FirstOrDefault();

			if (edit != null)
			{
				edit.AllowedSubmissions = model.AllowedSubmissions;
				//model.AssignmentID;
				edit.Description = model.Description;
				//model.ID;
				edit.IsRemoved = model.IsRemoved;
				edit.Name = model.Name;
				edit.Percentage = model.Percentage;

				_db.SaveChanges();
			}

			else
			{
				throw new Exception("Milestone that is being edited does not exist or has been removed.");
			}
		}

		//Removes Milestone with Associated Model
		internal void RemoveMilestone(MilestoneViewmodel model)
		{
			var milestone = (from mile in _db.Milestones
							where mile.ID == model.ID
							select mile).FirstOrDefault();

			if (milestone == null)
			{
				throw new Exception("Trying to remove a Milestone that does not exist or has been removed.");
			}

			milestone.IsRemoved = true;
			_db.SaveChanges();
		}

		public List<Testrun> getTestrunsOutcomeForSubmission(int submissionId)
		{
			List<Testrun> tRuns = (from t in _db.Testruns
								   where t.SubmissionID == submissionId
								   select t).ToList();
			return tRuns;

		}

		/// <summary>
		/// Finds all Milestones in system
		/// </summary>
		/// <returns>Number of milestones</returns>
        public int NumberOfMilestones()
        {
            var milestones = (from mil in _db.Milestones
                           select mil).Count();

            return milestones;
        }
    }
}