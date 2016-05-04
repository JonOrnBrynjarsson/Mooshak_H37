namespace Mooshak___H37.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;


    public class ErrorReport
    {
        public int ID { get; set; }

        public DateTime DateOccurred { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        public int? CourseID { get; set; }

        public int? AssignmentID { get; set; }

        public int? MilestoneID { get; set; }

        public int? SubmissionID { get; set; }

        public bool IsRemoved { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Assignment Assignment { get; set; }

        public virtual Course Course { get; set; }

        public virtual Submission Submission { get; set; }
    }
}
