using System.ComponentModel;

namespace Mooshak___H37.Models.Entities
{

	public class Testrun
	{
		public int ID { get; set; }

		public int TestCase { get; set; }

		public int SubmissionID { get; set; }

		public bool IsSuccess { get; set; }

		public string ResultComments { get; set; }

		[DefaultValue(0)]
		public bool IsRemoved { get; set; }

		public virtual Submission Submission { get; set; }

		//public virtual TestCase TestCase { get; set; }
	}
}
