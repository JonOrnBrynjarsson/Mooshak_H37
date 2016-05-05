namespace Mooshak___H37.Models.Entities
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("Courses")]
	public class Course
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Course()
		{
			Assignments = new HashSet<Assignment>();
			ErrorReports = new HashSet<ErrorReport>();
			UserCourseRelations = new HashSet<UserCourseRelation>();
		}

		public int ID { get; set; }

		[Required]
		public string Name { get; set; }

		public DateTime Startdate { get; set; }

		public bool Isactive { get; set; }

		public bool IsRemoved { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Assignment> Assignments { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ErrorReport> ErrorReports { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<UserCourseRelation> UserCourseRelations { get; set; }
	}
}
