using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models.EnumTypes;

namespace SchoolManagement.Controllers
{
    [Authorize]
    public class SomeController1 : Controller
    {
        [AllowAnonymous]
        public string Anyone()
        {
            return "任何人都可以访问Anyone(), 因为我添加了AllowAnonymous属性";
        }

        public string Logined()
        {
            return "只有登录后的用户都可以访问我, 因为控制器有Authorize属性";
        }

        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public string OnlyAdmin()
        {
            return "只有Admin角色才能访问我";
        }

        [Authorize(Roles = "Admin, User")]
        public string AdminOrUser()
        {
            return "只有拥有Admin或者User角色即可访问我";
        }

        [Authorize(Roles = nameof(RoleEnum.Admin))]
        [Authorize(Roles = nameof(RoleEnum.User))]
        public string AdminAndUser()
        {
            return "必须同时拥有Admin和User角色才能访问我";
        }
    }
}
