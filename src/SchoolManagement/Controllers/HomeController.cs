using Microsoft.AspNetCore.Mvc;
using SchoolManagement.DataRepositories;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System.Collections.Generic;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public HomeController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public ViewResult Index()
        {
            IEnumerable<Student> students = _studentRepository.GetAllStudents();
            return View(students);
        }

        public ViewResult Details(int id)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Student = _studentRepository.GetStudentById(id),
                PageTitle = "学生详情"
            };
            //ViewBag.PageTitle = "学生详情";
            //ViewData["Student"] = model;
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                Student newStudent = _studentRepository.Insert(student);
                return RedirectToAction(nameof(Details), new { id = student.Id });
            }

            return View();
        }
    }
}
