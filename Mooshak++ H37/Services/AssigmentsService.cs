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
                };
                viewModel.Add(model);
            }

            return viewModel;
        }
    }
}
