using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mooshak___H37.Models.Entities
{
	public class User
	{

		public int ID { get; set; }

		[StringLength(256)]
		public string Name { get; set; }

		[DefaultValue(0)]
		public bool IsRemoved { get; set; }

		[Required]
		public string AspNetUserId { get; set; }
		public virtual ApplicationUser AspNetUser { get; set; }

	}
}
