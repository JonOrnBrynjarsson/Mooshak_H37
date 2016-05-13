using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class TestCaseViewModel
	{
		public int ID { get; set; }
		[Display(Name = "Input String")]
		public string Inputstring { get; set; }
		[Display(Name = "Output String")]
		public string Outputstring { get; set; }
		public int MilestoneID { get; set; }
	}
}