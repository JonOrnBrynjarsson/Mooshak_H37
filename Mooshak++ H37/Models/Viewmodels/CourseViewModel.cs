using Mooshak___H37.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
    public class CourseViewModel
    {
        public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yy 00:00:00}",
               ApplyFormatInEditMode = true)]

		[Display(Name = "Start Date")]
		public DateTime? StartDate { get; set; }

		[Display(Name = "Active Course")]
		public bool Isactive { get; set; }
        public bool IsRemoved { get; set; }
		public List<AssignmentViewModel> Assignments { get; set; }
        public List<UserViewModel> User { get; set; }
    }
}