using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;
using SchoolManagement.Models.EnumTypes;

namespace SchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(new Student
            {
                Id = 1,
                Name = "张三",
                Major = MajorEnum.ElectronicCommerce,
                Email = "zhangsan@xxx.com"
            });

            modelBuilder.Entity<Student>().HasData(new Student
            {
                Id = 2,
                Name = "李四",
                Major = MajorEnum.Mathematics,
                Email = "lisi@xxx.com"
            });
        }
    }
}
