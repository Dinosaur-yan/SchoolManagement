using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Students
{
    public class StudentService : IStudentService
    {
        public readonly IRepository<Student, int> _studentRepository;

        public StudentService(IRepository<Student, int> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string orderBy, int pageSize = 10)
        {
            var query = _studentRepository.GetAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
            }

            query = query.OrderBy(orderBy);

            return await query.Skip((currentPage - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }
    }
}
