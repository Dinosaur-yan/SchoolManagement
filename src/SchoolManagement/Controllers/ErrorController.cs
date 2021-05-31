using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewBag.ExceptionPath = ((ExceptionHandlerFeature)exceptionHandlerPathFeature).Path;
            ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;
            return View("Error");
        }

        [Route("error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case StatusCodes.Status404NotFound:
                default:
                    ViewBag.ErrorMessage = "抱歉，用户访问的页面不存在";
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QueryString = statusCodeResult.OriginalQueryString;
                    return View("NotFound");
            }
        }
    }
}
