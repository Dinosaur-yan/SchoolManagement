using SchoolManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string orderBy, int pageSize = 10);
    }
}
