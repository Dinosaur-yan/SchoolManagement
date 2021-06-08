using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;
using System.Linq;

namespace SchoolManagement.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Seed();

            var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var foreignKey in foreignKeys)
            {
                #region 外键约束

                //         No Action(不执行): 默认行为，删除主表数据行时，依赖表中的数据不会执行任何操作，此时会产生错误，并回滚delete语句
                //         Cascade(级联删除): 删除主表数据行时，依赖表中的数据行也会同步删除
                //        Set Null(设置为空): 删除主表数据行时，将依赖表中的数据行的外键更新为null。为了满足此约束，目标表的外键列必须可为空值
                // Set Default(设置为默认值): 删除主表数据行时，将依赖表中的数据行的外键更新为默认值。为了满足此约束，目标表的外键列必须具有默认值定义；
                //                            如果外键可为空值，并且未显示设置默认值，则将使用null作为该列的隐式默认值

                #endregion

                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
