using DishMeAPP.Models;

namespace DishMeAPP.Services
{
    /// <summary>
    /// User settings service for managing app preferences 用户设置服务，用于管理应用偏好
    /// </summary>
    public class UserSettingsService
    {
        private const string USER_ROLE_KEY = "user_role";
        private const string FIRST_LAUNCH_KEY = "first_launch";

        /// <summary>
        /// Get current user role 获取当前用户角色
        /// </summary>
        /// <returns>Current user role 当前用户角色</returns>
        public UserRole GetCurrentUserRole()
        {
            try
            {
                var roleString = Preferences.Get(USER_ROLE_KEY, UserRole.Diner.ToDisplayString());
                return UserRoleExtensions.ParseRole(roleString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user role: {ex.Message}");
                return UserRole.Diner; // Default fallback 默认回退
            }
        }

        /// <summary>
        /// Set current user role 设置当前用户角色
        /// </summary>
        /// <param name="role">User role to set 要设置的用户角色</param>
        public void SetCurrentUserRole(UserRole role)
        {
            try
            {
                Preferences.Set(USER_ROLE_KEY, role.ToDisplayString());
                System.Diagnostics.Debug.WriteLine($"User role set to: {role.ToDisplayString()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting user role: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if this is the first app launch 检查是否为首次启动应用
        /// </summary>
        /// <returns>True if first launch 如果是首次启动则返回true</returns>
        public bool IsFirstLaunch()
        {
            try
            {
                var isFirstLaunch = Preferences.Get(FIRST_LAUNCH_KEY, true);
                if (isFirstLaunch)
                {
                    Preferences.Set(FIRST_LAUNCH_KEY, false);
                }
                return isFirstLaunch;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking first launch: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clear all user settings 清除所有用户设置
        /// </summary>
        public void ClearAllSettings()
        {
            try
            {
                Preferences.Clear();
                System.Diagnostics.Debug.WriteLine("All user settings cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get app version information 获取应用版本信息
        /// </summary>
        /// <returns>App version info 应用版本信息</returns>
        public (string Version, string Build) GetAppVersionInfo()
        {
            try
            {
                var version = AppInfo.VersionString ?? "1.0.0";
                var build = AppInfo.BuildString ?? "1";
                return (version, build);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting app version: {ex.Message}");
                return ("1.0.0", "1");
            }
        }

        /// <summary>
        /// Get device information 获取设备信息
        /// </summary>
        /// <returns>Device info 设备信息</returns>
        public (string Platform, string Model) GetDeviceInfo()
        {
            try
            {
                var platform = DeviceInfo.Platform.ToString();
                var model = DeviceInfo.Model ?? "Unknown";
                return (platform, model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting device info: {ex.Message}");
                return ("Unknown", "Unknown");
            }
        }
    }
}
