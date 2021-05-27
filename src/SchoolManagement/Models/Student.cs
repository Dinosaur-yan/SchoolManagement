using SchoolManagement.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Display(Name = "名字")]
        [Required(ErrorMessage = "请输入名字，它不能为空")]
        public string Name { get; set; }

        [Display(Name = "主修科目")]
        public MajorEnum? Major { get; set; }

        [Display(Name = "电子邮箱")]
        [Required(ErrorMessage = "请输入邮箱地址，它不能为空")]
        [EmailAddress(ErrorMessage = "邮箱的格式不正确")]
        public string Email { get; set; }
    }
}
