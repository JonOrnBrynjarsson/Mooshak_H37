using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
					Grade = x.Grade,
					AssignmentID = x.AssignmentID,
					IsRemoved = x.IsRemoved			
				}).ToList();

            var coursename = (from asi in _db.Courses
                              where asi.ID == assignment.CourseID
                              select asi).FirstOrDefault();

            System.Diagnostics.Debug.WriteLine("INFO");
            System.Diagnostics.Debug.WriteLine(id);
            System.Diagnostics.Debug.WriteLine(coursename.Name);


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
            }; 
			        
            return model;
        }


    }
}
