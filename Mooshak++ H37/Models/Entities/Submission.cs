using System.ComponentModel;

namespace Mooshak___H37.Models.Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	
	public class Submission
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Submission()
		{
			ErrorReports = new HashSet<ErrorReport>();
		}

		public int ID { get; set; }

		public int MilestoneID { get; set; }

		
		private int UserID { get; set; }

		[DefaultValue(0)]
		public bool IsGraded { get; set; }

		public double Grade { get; set; }

		[Required]
		public string ProgramFileLocation { get; set; }

		public bool IsRemoved { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ErrorReport> ErrorReports { get; set; }

		public virtual Milestone Milestone { get; set; }

		public virtual User User { get; set; }
	}
}
