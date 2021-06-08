using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.Models.EnumTypes;
using SchoolManagement.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region User

        public IActionResult ListUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到Id为{id}的用户";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Claims = userClaims.Select(t => t.Value).ToList(),
                Roles = userRoles.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到Id为{model.Id}的用户";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到Id为{id}的用户";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(nameof(ListUsers));
        }

        #endregion

        #region Role

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

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"无法找到Id为{id}的用户";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(nameof(ListRoles));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={id}的信息不存在，请重试。";
                return View("NotFound");
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
                return View("NotFound");
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
                return View("NotFound");
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
                return View("NotFound");
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

        #endregion
    }
}
