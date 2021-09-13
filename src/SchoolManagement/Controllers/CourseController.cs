using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
