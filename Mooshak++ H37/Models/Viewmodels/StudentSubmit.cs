using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
	public class StudentSubmit
	{
		[ConfigurationPropertyAttribute("maxRequestLength", DefaultValue = 50000)]
		public int milestone { get; set; }
		public HttpPostedFile file { get; set; }
	}
}
