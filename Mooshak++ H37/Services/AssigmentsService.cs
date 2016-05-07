using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;
using Microsoft.AspNet.Identity;
using Mooshak___H37.Models.Entities;

namespace Mooshak___H37.Services
{
	class AssigmentsService
	{
		private ApplicationDbContext _db;

		public int GetCurrentUser()
		{
			var currentUser = System.Web.HttpContext.Current.User.Identity.GetUserId();
			var currUserID = (from x in _db.Users
							  where x.AspNetUserId == currentUser
							  select x.ID).FirstOrDefault();

			return currUserID;
		}

		public AssigmentsService()
		{
			_db = new ApplicationDbContext();
		}

        public List<AssignmentViewModel> getAllAssignments()
        {
			var currUsId = GetCurrentUser();

			var userCourses = (from uscr in _db.UserCourseRelations
							   where currUsId == uscr.UserID
							   select uscr.CourseID).ToList();


			var assignments = (from assi in _db.Assignments
							   where userCourses.Contains(assi.CourseID)
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

			System.Diagnostics.Debug.WriteLine("===OUTPUT===");

			System.Diagnostics.Debug.WriteLine(assignments);

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
				Assignments = (from asi in _db.Assignments
							   where asi.CourseID == x.ID
							   select asi).ToList(),
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
				

			var milestones = _db.Milestones.Where(x => x.AssignmentID == id)
				.Select(x => new MilestoneViewmodel
				{
					ID = x.ID,
					AllowedSubmissions = x.AllowedSubmissions,
					Name = x.Name,
					Description = x.Description,
					AssignmentID = x.AssignmentID,
					IsRemoved = x.IsRemoved			
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
					  orderby user.Name ascending
					  select user).Select(x => new UserViewModel
					  {
						  CourseID = x.ID,
						  Name = x.Name
					  }).ToList();

            if (coursename == null)
            {
                // DO SOMETHING
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
		
		public void updateAssignment(AssignmentViewModel ass)
		{

			var item = (from it in _db.Assignments
						where it.ID == ass.ID
						select it).FirstOrDefault();

			if(item != null)
			{
				item.IsActive = ass.IsActive;
				item.IsRemoved = ass.IsRemoved;
				item.Name = ass.Name;
				item.SetDate = ass.SetDate;
				item.DueDate = ass.DueDate;
				item.CourseID = ass.CourseID;
				item.Description = ass.Description;

				_db.SaveChanges();
			}

		}
	}
}
