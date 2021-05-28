namespace SchoolManagement.ViewModels
{
    /// <summary>
    /// 编辑学生的视图模型
    /// </summary>
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 已经存在数据库中的图片文件路径
        /// </summary>
        public string ExistingPhotoPath { get; set; }
    }
}
