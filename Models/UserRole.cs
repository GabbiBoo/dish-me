namespace DishMeAPP.Models
{
    /// <summary>
    /// User role enumeration 用户角色枚举
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Chef role - can create and manage recipes 厨师角色 - 可以创建和管理菜谱
        /// </summary>
        Chef,

        /// <summary>
        /// Diner role - can browse and order dishes 食客角色 - 可以浏览和点餐
        /// </summary>
        Diner
    }

    /// <summary>
    /// User role extensions 用户角色扩展方法
    /// </summary>
    public static class UserRoleExtensions
    {
        /// <summary>
        /// Convert role to display string 将角色转换为显示字符串
        /// </summary>
        /// <param name="role">User role 用户角色</param>
        /// <returns>Display string 显示字符串</returns>
        public static string ToDisplayString(this UserRole role)
        {
            return role switch
            {
                UserRole.Chef => "Chef",
                UserRole.Diner => "Diner",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Parse string to user role 解析字符串为用户角色
        /// </summary>
        /// <param name="roleString">Role string 角色字符串</param>
        /// <returns>User role 用户角色</returns>
        public static UserRole ParseRole(string roleString)
        {
            return roleString?.ToLower() switch
            {
                "chef" => UserRole.Chef,
                "diner" => UserRole.Diner,
                _ => UserRole.Diner // Default to Diner 默认为食客
            };
        }
    }
}
