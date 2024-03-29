namespace Mooshak___H37.Models.Entities
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Viewmodels;
	public class Assignment
	{
		public Assignment()
		{
			ErrorReports = new HashSet<ErrorReport>();
			GroupMembers = new HashSet<GroupMember>();
			Milestones = new HashSet<Milestone>();
		}
		
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }

		public DateTime? SetDate { get; set; }

		public DateTime? DueDate { get; set; }

		public int CourseID { get; set; }

		public bool IsActive { get; set; }

		public bool IsRemoved { get; set; }

		public string Description { get; set; }

		public virtual Course Course { get; set; }
		public virtual ICollection<ErrorReport> ErrorReports { get; set; }

		public virtual ICollection<GroupMember> GroupMembers { get; set; }

		public virtual ICollection<Milestone> Milestones { get; set; }
	}
}
