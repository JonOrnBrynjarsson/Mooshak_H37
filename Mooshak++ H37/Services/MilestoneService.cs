﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Entities;
using Mooshak___H37.Models.Viewmodels;

namespace Mooshak___H37.Services
{
	public class MilestoneService
	{
		private readonly IAppDataContext _db;
		private readonly UsersService _usersService;

		public MilestoneService(IAppDataContext dbContext)
		{
			_db = dbContext ?? new ApplicationDbContext();
			_usersService = new UsersService(null);
		}

		/// <summary>
		/// Gets all milestones associated to a specific assignment
		/// </summary>
		/// <param name="assignmentId">the id of the assignment for finding the milestones</param>
		/// <returns>List of a Milestone entity class</returns>
		private List<Milestone> getMilestones (int assignmentId)
        {
            var milestones = (from miles in _db.Milestones
                              orderby miles.ID
                              where miles.AssignmentID == assignmentId &&
                              miles.IsRemoved == false
                              select miles).ToList();
            return milestones;
        }

		/// <summary>
		/// Creates a milestone in the Database (Milestones table)
		/// </summary>
		/// <param name="model">Holds all the info of the new milestone</param>
		/// <param name="assignmentId">id of assignment the milestone belongs to</param>
		internal void createMilestone(MilestoneViewmodel model, int assignmentId)
		{
			_db.Milestones.Add(new Milestone
			{
				AllowedSubmissions = model.AllowedSubmissions,
				AssignmentID = assignmentId,
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
		/// <param name="assignmentId"></param>
		/// <returns>List Of Milestones</returns>
		public List<MilestoneViewmodel> getMilestonesForAssignment(int assignmentId)
		{
            var milestones = getMilestones(assignmentId);
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
		/// <param name="assignmentId">The assignment Id</param>
		/// <returns>Percentage of All Milestones in Given Assignment</returns>
		public double getTotalMilestonePercentageForAssignment(int assignmentId)
		{
            var milestones = getMilestones(assignmentId);

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
		public void userCanSubmitMilestone(int milestoneId)
		{
			//Returns ID of current User
			var currUser = _usersService.getUserIdForCurrentApplicationUser();

			//Finds all submissions from Current User
			var submissions = (from s in _db.Submissions
							   where s.UserID == currUser && s.MilestoneID == milestoneId
							   && s.IsRemoved != true
							   select s.ID).Count();

			//Finds how often a User Can submit
			var allowedSubmissions = (from x in _db.Milestones
									  where x.ID == milestoneId
									  && x.IsRemoved != true
									  select x.AllowedSubmissions).SingleOrDefault();

			//Returns true if User has not submitted to many times
			if (submissions >= allowedSubmissions)
			{
				throw new Exception("You have already submitted the maximum number of times");
			}
		}

		/// <summary>
		/// Checks if Current Milestones in Assignment with the New Milestone that is
		/// Being created Exceed 100%
		/// </summary>
		/// <param name="model"></param>
		/// <param name="assignmentId"></param>
		/// <returns>True or False</returns>
		public bool teacherCanCreateMilestone(MilestoneViewmodel model, int assignmentId)
		{
			//Current Percentage of Milestones in Assignment
			var totalPercentage = getTotalMilestonePercentageForAssignment(assignmentId);

			//Percentage of Milestone that is being created
			var milestonePercentage = model.Percentage;

			//Percentage that will be after Creation
			var currPercentage = totalPercentage + milestonePercentage;

			//Returns false if it would exceed 100%
			if (currPercentage > 100)
			{
                throw new Exception("Current Milestone will make Total Milestones exceed 100%");
            }
			return true;
		}

		/// <summary>
		/// Finds Milestone with Given Milestone ID
		/// </summary>
		/// <param name="milestoneId"></param>
		/// <returns>Milestone</returns>
		public MilestoneViewmodel getSingleMilestone(int milestoneId)
		{
			//Finds Milestone that is associated with given Milestone ID
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID 
							  where miles.ID == milestoneId &&
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
		internal void editMilestone(MilestoneViewmodel model, int milestoneId)
		{
			var edit = (from mil in _db.Milestones where mil.ID == 
						milestoneId
						&& mil.IsRemoved != true
						select mil).FirstOrDefault();

			if (edit != null)
			{
				edit.AllowedSubmissions = model.AllowedSubmissions;
				edit.Description = model.Description;
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


		public int allowedSubmissionsForMilestone(int milestoneId)
		{
			var submissions = (from allowedSubs in _db.Milestones
							   where allowedSubs.ID == milestoneId &&
							   allowedSubs.IsRemoved != true
							   select allowedSubs.AllowedSubmissions).FirstOrDefault();
			return submissions;
		}

		//Removes Milestone with Associated Model
		internal void removeMilestone(MilestoneViewmodel model)
		{
			var milestone = (from mile in _db.Milestones
							where mile.ID == model.ID
							&& mile.IsRemoved == false
							select mile).FirstOrDefault();

			if (milestone == null)
			{
				throw new Exception("Trying to remove a Milestone that does not exist or has been removed.");
			}

			milestone.IsRemoved = true;
			_db.SaveChanges();
		}

		/// <summary>
		/// Sets all milestones isRemoved to true for the matching assignment id we get as param.
		/// </summary>
		public void removeMilestoneByAssignmentId(int assignmentId)
		{
			var crs = (from item in _db.Milestones
					   where item.AssignmentID == assignmentId
					   && item.IsRemoved == false
					   select item).ToList();
			if(crs != null)
			{
				foreach (var c in crs)
				{
					c.IsRemoved = true;
				}
			}
		}



		/// <summary>
		/// Finds all Milestones in system
		/// </summary>
		/// <returns>Number of milestones</returns>
        public int numberOfMilestones()
        {
            var milestones = (from mil in _db.Milestones
							  where mil.IsRemoved == false
                           select mil).Count();

            return milestones;
        }
		/// <summary>
		/// Gets the Milestone ID from the database based on a specific submission ID.
		/// </summary>
		/// <param name="submissionId">The "ID" of the submission for the milestone</param>
		/// <returns>ID of Milestone</returns>
		public int getMilestoneIdBySubmitId(int submissionId)
		{
			int milestoneId = (from s in _db.Submissions
							   where s.ID == submissionId
							   select s.MilestoneID).SingleOrDefault();
			return milestoneId;
		}

		
	}
}