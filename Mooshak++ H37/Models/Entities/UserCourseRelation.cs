namespace Mooshak___H37.Models.Entities
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("UserCourseRelation")]
    public class UserCourseRelation
    {
        public int ID { get; set; }

        public int CourseID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        [Required]
        [StringLength(128)]
        public string RoleID { get; set; }

        public bool IsRemoved { get; set; }

        public virtual AspNetRole AspNetRole { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Course Course { get; set; }
    }
}
