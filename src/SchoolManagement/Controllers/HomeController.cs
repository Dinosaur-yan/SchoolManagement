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
            Student student = _studentRepository.GetStudentById(id);
            if (student == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("StudentNotFound", id);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Student = student,
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

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = _studentRepository.GetStudentById(id);

            if (student == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("StudentNotFound", id);
            }

            StudentEditViewModel studentEditViewModel = new()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath
            };
            return View(studentEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = _studentRepository.GetStudentById(model.Id);

                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                student.PhotoPath = ProcessUploadedFile(null, model.Photos);

                Student updatedstudent = _studentRepository.Update(student);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private string ProcessUploadedFile(string originalPath, List<IFormFile> files)
        {
            if (files == null || !files.Any()) return originalPath;

            try
            {
                string uniqueFileName = null;
                foreach (IFormFile file in files)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString("N") + "_" + file.FileName;
                    string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }

                if (!string.IsNullOrWhiteSpace(originalPath))
                    return uniqueFileName;

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", originalPath ?? "");
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return uniqueFileName;
            }
            catch
            {
                return null;
            }
        }
    }
}
