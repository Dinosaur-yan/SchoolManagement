using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.DataRepositories;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentRepository _studentRepository;

        public HomeController(IWebHostEnvironment webHostEnvironment, IStudentRepository studentRepository)
        {
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Any())
                {
                    foreach (IFormFile file in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        uniqueFileName = Guid.NewGuid().ToString("N") + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        file.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }

                Student newStudent = new()
                {
                    Name = model.Name,
                    Major = model.Major,
                    Email = model.Email,
                    PhotoPath = uniqueFileName,
                };

                _studentRepository.Insert(newStudent);
                return RedirectToAction(nameof(Details), new { id = newStudent.Id });
            }

            return View();
        }
    }
}
