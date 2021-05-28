using SchoolManagement.Models.EnumTypes;

namespace SchoolManagement.Models
{
    public class Student
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主修科目
        /// </summary>
        public MajorEnum? Major { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string PhotoPath { get; set; }
    }
}
