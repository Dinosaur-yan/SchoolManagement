using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SchoolManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            //ViewBag.ExceptionPath = ((ExceptionHandlerFeature)exceptionHandlerPathFeature).Path;
            //ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            //ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;

            _logger.LogError($"路径{((ExceptionHandlerFeature)exceptionHandlerPathFeature).Path}，产生了一个错误：{exceptionHandlerPathFeature.Error.Message}");

            return View("Error");
        }

        [Route("error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            //ViewBag.Path = statusCodeResult.OriginalPath;
            //ViewBag.QueryString = statusCodeResult.OriginalQueryString;

            switch (statusCode)
            {
                case StatusCodes.Status404NotFound:
                default:
                    ViewBag.ErrorMessage = "抱歉，用户访问的页面不存在";

                    _logger.LogError($"发生了一个404错误，路径{statusCodeResult.OriginalPath + statusCodeResult.OriginalQueryString}");
                    return View("NotFound");
            }
        }
    }
}
