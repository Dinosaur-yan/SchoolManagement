using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCourseId { get; set; }

        public int CourseId { get; set; }

        public int StudentId { get; set; }

        public Course Course { get; set; }

        public Student Student { get; set; }
    }
}
