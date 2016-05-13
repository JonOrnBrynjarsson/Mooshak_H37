using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class MilestoneViewmodel
	{
		public int ID { get; set; }
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Description is required")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Submissions are required")]
		[Display(Name = "Allowed Submissions")]
		public int AllowedSubmissions { get; set; }
		//public double? Grade { get; set; }
		public int AssignmentID { get; set; }
		public bool IsRemoved { get; set; }
		[Required(ErrorMessage = "Percentage is required")]
		public double Percentage { get; set; }
		public int UserSubmissions { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
		[Display(Name = "Date Set")]
		public DateTime DateSet { get; set; }
		[Display(Name = "Due Date")]
		public DateTime DueDate { get; set; }
		public int TotalSubmissions { get; set; }

	}
}