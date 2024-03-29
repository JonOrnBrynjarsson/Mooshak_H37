﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
    public class AssignmentViewModel
    {
        public int ID { get; set; }
		[Required(ErrorMessage  = "Name is required")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
		[Display(Name = "Set Date")]
		public DateTime? SetDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
		[Display(Name = "Due Date")]
		public DateTime? DueDate { get; set; }
		public int CourseID { get; set; }

		[Display(Name = "Active Assignment")]
		public bool IsActive { get; set; }
        public bool IsRemoved { get; set; }
		public string Description { get; set; }
		public List<MilestoneViewmodel> Milestones { get; set; }
		public List<UserViewModel> Users { get; set; }
		public string CourseName { get; set; }
        public bool Submitted { get; set; }
        public double TotalGrade { get; set; }
	}
}