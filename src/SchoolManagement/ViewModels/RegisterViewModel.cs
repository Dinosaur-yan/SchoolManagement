using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮箱")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]   //DataType特性不提供任何验证，它主要服务于我们的视图文件
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码与确认密码不一致，请重新输入")]
        public string ConfirmPassword { get; set; }
    }
}
