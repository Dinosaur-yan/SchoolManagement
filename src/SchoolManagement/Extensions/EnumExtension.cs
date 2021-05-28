using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SchoolManagement.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的显示名字
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum @enum)
        {
            Type type = @enum.GetType();
            MemberInfo[] memberInfos = type.GetMember(@enum.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                object attr = memberInfos[0].GetCustomAttribute<DisplayAttribute>();
                if (attr != null)
                {
                    return ((DisplayAttribute)attr).Name;
                }
            }
            return @enum.ToString();
        }
    }
}
