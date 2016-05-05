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

        public List<StudentAssignmentViewModel> getAssignments()
        {

            var assignments = (from assi in _db.Assignments
                               orderby assi.DueDate descending
                               select assi);

            var viewModel = new List<StudentAssignmentViewModel>();

            foreach (var assignm in assignments)
            {
                StudentAssignmentViewModel model = new StudentAssignmentViewModel
                {
                    Name = assignm.Name,
                    Date = assignm.DueDate,
                    ID = assignm.ID,
                };
                viewModel.Add(model);
            }

            return viewModel;
        }
    }
}
