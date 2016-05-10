using Mooshak___H37.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class SubmissionsViewModel
	{
		public int ID { get; set; }

		public int MilestoneID { get; set; }

		public int UserID { get; set; }
		public bool IsGraded { get; set; }

		public double Grade { get; set; }

		public string ProgramFileLocation { get; set; }

		public bool IsRemoved { get; set; }

		public DateTime DateSubmitted { get; set; }

		public virtual Milestone Milestone { get; set; }

		public List<UserViewModel> User { get; set; }
		public string UserName { get; set; }
	}
}