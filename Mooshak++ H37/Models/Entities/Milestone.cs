namespace Mooshak___H37.Models.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Milestone
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Milestone()
		{
			Submissions = new HashSet<Submission>();
			TestCases = new HashSet<TestCase>();
		}

		public int ID { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		[Column(TypeName = "text")]
		[Required]
		public string Description { get; set; }

		public int AllowedSubmissions { get; set; }

		public double? Grade { get; set; }

		public int AssignmentID { get; set; }

		public bool IsRemoved { get; set; }

		public virtual Assignment Assignment { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Submission> Submissions { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<TestCase> TestCases { get; set; }
	}
}
