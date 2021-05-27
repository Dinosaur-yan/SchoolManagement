using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models.EnumTypes
{
    /// <summary>
    /// 主修科目
    /// </summary>
    public enum MajorEnum
    {
        /// <summary>
        /// 未分配
        /// </summary>
        [Display(Name = "未分配")]
        None,

        /// <summary>
        /// 计算机科学
        /// </summary>
        [Display(Name = "计算机科学")]
        ComputerScience,

        /// <summary>
        /// 电子商务
        /// </summary>
        [Display(Name = "电子商务")]
        ElectronicCommerce,

        /// <summary>
        /// 数学
        /// </summary>
        [Display(Name = "数学")]
        Mathematics
    }
}
