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
		private ApplicationDbContext _db;

		public MilestoneService()
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
		public List<MilestoneViewmodel> GetMilestonesForAssignment(int assigID)
		{
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID
							  where miles.AssignmentID == assigID && 
							  miles.IsRemoved != true
							  select miles).ToList();

			var viewModel = new List<MilestoneViewmodel>();

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

		public double GetTotalMilestonePercentageForAssignment(int assigID)
		{
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID
							  where miles.AssignmentID == assigID &&
							  miles.IsRemoved != true
							  select miles).ToList();

			var viewModel = new List<MilestoneViewmodel>();

			double totalPercentage = 0;

			foreach (var mil in milestones)
			{
				totalPercentage += mil.Percentage;
			}

			return totalPercentage;
		}

		public bool UserCanSubmitMilestone(int milestoneID)
		{

			var currUser = GetCurrentUser();

			var submissions = (from s in _db.Submissions
							   where s.UserID == currUser && s.MilestoneID == milestoneID
							   select s.ID).Count();

			var allowedSubmissions = (from x in _db.Milestones
									  where x.ID == milestoneID
									  select x.AllowedSubmissions).SingleOrDefault();

			if (submissions <= allowedSubmissions)
			{
				return true;
			}

			return false;
		}

		public bool TeacherCanCreateMilestone(MilestoneViewmodel model, int assignmentID)
		{
			var totalPercentage = GetTotalMilestonePercentageForAssignment(assignmentID);

			var milestonePercentage = model.Percentage;

			var currPercentage = totalPercentage + milestonePercentage;

			if (currPercentage > 100)
			{
				return false;
			}
			return true;
		}

		public MilestoneViewmodel GetSingleMilestone(int milID)
		{
			var milestones = (from miles in _db.Milestones
							  orderby miles.ID
							  where miles.ID == milID
							  select miles).FirstOrDefault();

			if (milestones == null)
			{
				//DO SOMETHING
				//throw new exception / skila NULL(ekki skila null hér)
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

		internal void EditMilestone(MilestoneViewmodel model, int milestoneID)
		{
			var edit = (from mil in _db.Milestones where mil.ID == 
						milestoneID select mil).FirstOrDefault();

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
				// DO SOMETHING!!!
			}
		}

		internal void RemoveMilestone(MilestoneViewmodel model)
		{
			var milestone = (from mile in _db.Milestones
							where mile.ID == model.ID
							select mile).FirstOrDefault();

			if (milestone == null)
			{
				//DO SOMEHTING
			}

			milestone.IsRemoved = true;
			_db.SaveChanges();
		}

		public List<Testrun> getTestrunsOutcomeForSubmission(int submissionId)
		{
			List<Testrun> tRuns = (from t in _db.Testruns
								   where t.ID == submissionId
								   select t).ToList();
			return tRuns;

		}

        public int NumberOfMilestones()
        {
            var milestones = (from mil in _db.Milestones
                           select mil).Count();

            return milestones;
        }
    }
}