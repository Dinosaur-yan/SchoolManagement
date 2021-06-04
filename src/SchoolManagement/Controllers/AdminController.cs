using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new()
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={id}的信息不存在，请重试。";
                return View("../Error/NotFound");
            }

            var model = new RoleEditViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={model.Id}的信息不存在，请重试。";
                return View("../Error/NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.RoleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试。";
                return View("../Error/NotFound");
            }

            var model = new List<UserRoleViewModel>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                userRoleViewModel.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(string roleId, List<UserRoleViewModel> models)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试。";
                return View("../Error/NotFound");
            }

            foreach (var model in models)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);

                switch (model.IsSelected, isInRole)
                {
                    case (true, false):
                        await _userManager.AddToRoleAsync(user, role.Name);
                        break;

                    case (false, true):
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                        break;

                    default:
                        continue;
                }
            }

            return RedirectToAction(nameof(EditRole), new { Id = roleId });
        }
    }
}
