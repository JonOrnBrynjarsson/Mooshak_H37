namespace Mooshak___H37.Models.Entities
{
	using System.ComponentModel.DataAnnotations;

	public class GroupMember
	{
		public int ID { get; set; }

		public int AssignmentID { get; set; }

		[Required]
		public int UserID { get; set; }

		public bool IsRemoved { get; set; }

		public virtual User User { get; set; }

		public virtual Assignment Assignment { get; set; }
	}
}
