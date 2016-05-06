using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak___H37.Models;
using Mooshak___H37.Models.Viewmodels;


namespace Mooshak___H37.Services
{
	class AssigmentsService
	{
		private ApplicationDbContext _db;

		public AssigmentsService()
		{
			_db = new ApplicationDbContext();
		}

        public List<AssignmentViewModel> getAllAssignments()
        {

			var assignments = (from assi in _db.Assignments
							   orderby assi.DueDate descending
							   select assi);

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

		public List<CourseViewModel> GetCourseAssignments()
		{
				var courses = _db.Courses
				.Select(x => new CourseViewModel
				{
					ID = x.ID,
					Name = x.Name,
					Isactive = x.Isactive,
					IsRemoved = x.IsRemoved,
					StartDate = x.Startdate,
					Assignments = (from asi in _db.Assignments
								   where asi.CourseID == x.ID select asi).ToList(),
				}).ToList();

			var viewModel = new List<CourseViewModel>();

			foreach (var course in courses)
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


    }
}
