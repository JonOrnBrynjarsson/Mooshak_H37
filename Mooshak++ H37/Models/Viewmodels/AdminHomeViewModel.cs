using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mooshak___H37.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Mooshak___H37.Models.Viewmodels
{
	public class AdminHomeViewModel
	{
        public DateTime Date { get; set; }
		public string Message { get; set; }
		public string User { get; set; }
	}

}
