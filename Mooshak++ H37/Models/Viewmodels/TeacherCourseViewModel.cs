using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak___H37.Models.Viewmodels
{
    public class TeacherCourseViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Isactive { get; set; }
        public bool IsRemoved { get; set; }
    }
}