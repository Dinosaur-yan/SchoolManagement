using SchoolManagement.Models;
using System.Collections.Generic;

namespace SchoolManagement.DataRepositories
{
    public interface IStudentRepository
    {
        Student GetStudent(int id);

        IEnumerable<Student> GetAllStudents();
    }
}
