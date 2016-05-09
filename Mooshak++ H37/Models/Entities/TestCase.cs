namespace Mooshak___H37.Models.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("TestCases")]
	public class TestCase
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TestCase()
		{
		Testruns = new HashSet<Testrun>();
		}

		public int ID { get; set; }

		public string Inputstring { get; set; }

		public int MilestoneID { get; set; }

		public bool? IsOnlyForTeacher { get; set; }

		public bool IsRemoved { get; set; }

		public virtual Milestone Milestone { get; set; }


		public virtual ICollection<Testrun> Testruns { get; set; }
	}
}
