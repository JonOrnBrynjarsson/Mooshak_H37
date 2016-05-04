namespace Mooshak___H37.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
	using Mooshak___H37.Models;

	public class GroupMember
    {
        public int ID { get; set; }

        public int AssignmentID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        public bool IsRemoved { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Assignment Assignment { get; set; }
    }
}
