﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolManagement.Models;
using SchoolManagement.Models.EnumTypes;
using SchoolManagement.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Basic

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByEmailAsync(model.Email);
                //if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                //{
                //    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                //    return View(model);
                //}

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RemeberMe, false);
                if (result.Succeeded)
                {
                    // 使用returnUrl可能会引起开放式重定向漏洞
                    // 使用LocalRedirect()代替Redirect()，它指定了非本地URL，则抛出异常
                    // 也可以使用Url.IsLocalUrl()，它判断检查提供的URL是否是本地URL
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);
                    _logger.LogInformation(confirmationLink);

                    if (_signInManager.IsSignedIn(User) && User.IsInRole(nameof(RoleEnum.Admin)))
                    {
                        return RedirectToAction("listusers", "admin");
                    }

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("index", "home");
                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = $"在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮箱验证，单击确认链接即可完成";
                    return View("Error");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱：{email} 已经被注册时用了。");
            }
        }

        #endregion

        #region ExtensionLogin

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError(string.Empty, $"第三方登录提供程序错误：{remoteError}");
                return View("Login", loginViewModel);
            }

            // 从外部登录提供者,即微软账户体系中，获取关于用户的登录信息。
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"加载外部登录信息出错。");
                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (!string.IsNullOrEmpty(email))
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, $"您的电子邮箱还未进行验证。");
                    return View("Login", loginViewModel);
                }
            }


            //如果用户之前已经登录过了，会在AspNetUserLogins表有对应的记录，这个时候无需创建新的记录，直接使用当前记录登录系统即可。
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //如果AspNetUserLogins表中没有记录，则代表用户没有一个本地帐户，这个时候我们就需要创建一个记录了。  
                if (!string.IsNullOrEmpty(email))
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await _userManager.CreateAsync(user);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);
                        _logger.LogInformation(confirmationLink);

                        ViewBag.ErrorTitle = "注册成功";
                        ViewBag.ErrorMessage = $"在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮箱验证，单击确认链接即可完成";
                        return View("Error");
                    }

                    // 在AspNetUserLogins表中,添加一行用户数据，然后将当前用户登录到系统中
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }

            // 如果我们获取不到电子邮件地址，我们需要将请求重定向到错误视图中。
            ViewBag.ErrorTitle = $"我们无法从提供商:{info.LoginProvider}中解析到您的邮件地址 ";
            ViewBag.ErrorMessage = "请通过联系 ltm@ddxc.org 寻求技术支持。";

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"当前{userId}无效";
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = $"您的电子邮箱还未进行验证";
            return View("Error");
        }

        #endregion

        #region Email

        [HttpGet]
        public IActionResult ActivateUserEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUserEmail(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮件地址查询用户地址
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    { //生成电子邮件确认令牌
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        //生成电子邮件的确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.Message = "如果你在我们系统有注册账户，我们已经发了邮件到您的邮箱中，请前往邮箱激活您的用户。";
                        //重定向用户到忘记密码确认视图
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }
            }
            ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";
            // 为了避免帐户枚举和暴力攻击，所以不进行用户不存在或邮箱未验证的提示
            return View("ActivateUserEmailConfirmation", ViewBag.Message);
        }

        #endregion

        #region 忘记密码功能 

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮件地址查询用户地址
                var user = await _userManager.FindByEmailAsync(model.Email);
                // 如果找到了用户并且确认了电子邮件
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    //生成重置密码令牌
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // 生成密码重置链接
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // 将密码重置链接记录到文件中
                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    //重定向用户到忘记密码确认视图
                    return View("ForgotPasswordConfirmation");
                }

                // 为了避免帐户枚举和暴力攻击，所以不进行用户不存在或邮箱未验证的提示
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {

            // 如果密码重置令牌或电子邮件为空，则有可能是用户在试图篡改密码重置链接
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "无效的密码重置令牌");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过电子邮件查找用户
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // 重置用户密码
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    //显示验证错误信息。触发行为，当密码重置令牌已用，或密码复杂性规则不符合标准时
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // 为了避免帐户枚举和暴力攻击，不要提示用户不存在               
                return View("ResetPasswordConfirmation");
            }
            // 如果模型验证未通过，则显示验证错误
            return View(model);
        }

        #endregion
    }
}
