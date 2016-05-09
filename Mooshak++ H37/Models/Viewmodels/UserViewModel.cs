using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class UserViewModel
	{
		public string Name { get; set; }
		public List<int> CourseID { get; set; }
		public int RoleID { get; set; }
		public string Email { get; set; }
	}
}