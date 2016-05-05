namespace Mooshak___H37.Models.Entities
{

	public class Testrun
	{
		public int ID { get; set; }

		//public int TestCaseID { get; set; }

		public int SubmissionID { get; set; }

		public bool IsSuccess { get; set; }

		public string ResultComments { get; set; }

		public bool IsRemoved { get; set; }

		public virtual Submission Submission { get; set; }

		//public virtual TestCase TestCase { get; set; }
	}
}
