namespace Mooshak___H37.Models.Entities
{

	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;


	[Table("UserCourseRelation")]
	public class UserCourseRelation
	{
		public int ID { get; set; }

		public int CourseID { get; set; }

		[Required]
		public int UserID { get; set; }

		[Required]
		public int RoleID { get; set; }

		public bool IsRemoved { get; set; }

		public virtual Role Role { get; set; }

		public virtual User User { get; set; }

		public virtual Course Course { get; set; }
	}
}
