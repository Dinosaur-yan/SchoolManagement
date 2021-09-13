using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos;
using SchoolManagement.Application.Students;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentService _studentService;
        private readonly IRepository<Student, int> _studentRepository;

        public HomeController(
            IDataProtectionProvider provider,
            IWebHostEnvironment webHostEnvironment,
            IStudentService studentService,
            IRepository<Student, int> studentRepository
            )
        {
            _protector = provider.CreateProtector("school_management");
            _webHostEnvironment = webHostEnvironment;
            _studentService = studentService;
            _studentRepository = studentRepository;
        }

        public async Task<IActionResult> Index(int? currentPage, int pageSize, string searchString, string sortBy = "Id")
        {
            ViewBag.CurrentFilter = searchString?.Trim();

            PaginationModel paginationModel = new()
            {
                Count = await _studentRepository.CountAsync(),
                CurrentPage = currentPage ?? 1
            };

            var students = await _studentService.GetPaginatedResult(paginationModel.CurrentPage, searchString, sortBy);

            paginationModel.Data = students?.Select(t =>
            {
                t.EncryptedId = _protector.Protect(t.Id.ToString());
                return t;
            }).ToList();
            return View(students);
        }

        public async Task<IActionResult> Details(string id)
        {
            Student student = await DecryptedStudentAsync(id);
            if (student == null)
            {
                //Response.StatusCode = StatusCodes.Status404NotFound;
                //return View("StudentNotFound", id);
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试";
                return View("NotFound");
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
                    EnrollmentDate = model.EnrollmentDate
                };

                _studentRepository.Insert(newStudent);
                var encryptedId = _protector.Protect(newStudent.Id.ToString());
                return RedirectToAction(nameof(Details), new { id = encryptedId });
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            Student student = await DecryptedStudentAsync(id);

            if (student == null)
            {
                //Response.StatusCode = StatusCodes.Status404NotFound;
                //return View("StudentNotFound", id);

                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试";
                return View("NotFound");
            }

            StudentEditViewModel studentEditViewModel = new()
            {
                Id = id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath,
                EnrollmentDate = student.EnrollmentDate,
            };
            return View(studentEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = await DecryptedStudentAsync(model.Id);

                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                student.EnrollmentDate = model.EnrollmentDate;
                student.PhotoPath = ProcessUploadedFile(null, model.Photos);

                Student updatedstudent = _studentRepository.Update(student);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var student = await _studentRepository.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试";
                return View("NotFound");
            }

            await _studentRepository.DeleteAsync(s => s.Id == id);
            return RedirectToAction(nameof(Index));
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

        /// <summary>
        /// 解密学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<Student> DecryptedStudentAsync(string id)
        {
            string decryptedId = _protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);
            Student student = await _studentRepository.FirstOrDefaultAsync(s => s.Id == decryptedStudentId);
            return student;
        }
    }
}
