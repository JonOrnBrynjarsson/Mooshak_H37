using System.ComponentModel.DataAnnotations.Schema;

namespace Mooshak___H37.Models.Entities
{
	using System;
	using System.ComponentModel.DataAnnotations;


	public class ErrorReport
	{
		public int ID { get; set; }

		public DateTime DateOccurred { get; set; }

		[Required]
		public int UserID { get; set; }

		public int? CourseID { get; set; }

		public int? AssignmentID { get; set; }

		public int? MilestoneID { get; set; }

		public int? SubmissionID { get; set; }

		public bool IsRemoved { get; set; }

		[Column("Message")]
		public string _message { get; set; }

		public virtual User User { get; set; }

		public virtual Course Course { get; set; }

		public virtual Submission Submission { get; set; }

		public virtual Milestone Milestone { get; set; }
	}
}
