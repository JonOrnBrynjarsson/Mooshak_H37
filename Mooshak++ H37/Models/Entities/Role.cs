using System.ComponentModel.DataAnnotations;

namespace Mooshak___H37.Models.Entities
{
	public class Role
	{
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }


	}
}
