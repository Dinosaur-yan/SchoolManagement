using SchoolManagement.Infrastructure;
using SchoolManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.DataRepositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Student Delete(int id)
        {
            Student student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _context.Students.AsEnumerable();
        }

        public Student GetStudentById(int id)
        {
            return _context.Students.Find(id);
        }

        public Student Insert(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }

        public Student Update(Student updateStudent)
        {
            var student = _context.Students.Attach(updateStudent);
            student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateStudent;
        }
    }
}
