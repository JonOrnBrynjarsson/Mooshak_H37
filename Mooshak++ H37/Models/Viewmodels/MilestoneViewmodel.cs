using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class MilestoneViewmodel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int AllowedSubmissions { get; set; }
		public double? Grade { get; set; }
		public int AssignmentID { get; set; }
		public bool IsRemoved { get; set; }
		

	}
}