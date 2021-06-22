using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Models;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class WelcomeController : Controller
    {
        private readonly IRepository<Student, int> _studentRepository;

        public WelcomeController(IRepository<Student, int> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<string> Index()
        {
            var student = await _studentRepository.GetAll().FirstOrDefaultAsync();

            var longCount = await _studentRepository.LongCountAsync();

            var count = _studentRepository.Count();

            return $"{student.Name} + {longCount} + {count}";
        }
    }
}
