using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
    public class AssignmentViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? SetDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int CourseID { get; set; }
        public bool IsActive { get; set; }
        public bool IsRemoved { get; set; }
		public string Description { get; set; }
		public List<MilestoneViewmodel> Milestones { get; set; }
		public List<UserViewModel> Users { get; set; }
		public string CourseName { get; set; }
		public List<CourseViewModel> CourseList { get; set; }
    }
}