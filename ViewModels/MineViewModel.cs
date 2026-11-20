using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DishMeAPP.Models;
using DishMeAPP.Services;

namespace DishMeAPP.ViewModels
{
    /// <summary>
    /// Mine page view model 我的页面视图模型
    /// </summary>
    public class MineViewModel : INotifyPropertyChanged
    {
        private readonly UserSettingsService _userSettingsService;
        private UserRole _currentUserRole;

        public MineViewModel()
        {
            _userSettingsService = new UserSettingsService();
            _currentUserRole = _userSettingsService.GetCurrentUserRole();

            // Initialize commands 初始化命令
            SwitchRoleCommand = new Command(async () => await SwitchRoleAsync());
            ManageCategoriesCommand = new Command(async () => await ManageCategoriesAsync());
            OrderHistoryCommand = new Command(async () => await OrderHistoryAsync());
            AboutAppCommand = new Command(async () => await AboutAppAsync());
        }

        #region Properties 属性

        /// <summary>
        /// Current user role 当前用户角色
        /// </summary>
        public UserRole CurrentUserRole
        {
            get => _currentUserRole;
            set
            {
                if (_currentUserRole != value)
                {
                    _currentUserRole = value;
                    _userSettingsService.SetCurrentUserRole(value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RoleButtonText));
                }
            }
        }

        /// <summary>
        /// Role button text 角色按钮文本
        /// </summary>
        public string RoleButtonText => $"Current Role: {CurrentUserRole.ToDisplayString()}";

        #endregion

        #region Commands 命令

        /// <summary>
        /// Switch role command 切换角色命令
        /// </summary>
        public ICommand SwitchRoleCommand { get; }

        /// <summary>
        /// Manage categories command 管理分类命令
        /// </summary>
        public ICommand ManageCategoriesCommand { get; }

        /// <summary>
        /// Order history command 订单历史命令
        /// </summary>
        public ICommand OrderHistoryCommand { get; }

        /// <summary>
        /// About app command 关于应用命令
        /// </summary>
        public ICommand AboutAppCommand { get; }

        #endregion

        #region Command Methods 命令方法

        /// <summary>
        /// Switch user role 切换用户角色
        /// </summary>
        private async Task SwitchRoleAsync()
        {
            try
            {
                var result = await SafeDisplayActionSheet(
                    "Select Role",
                    "Cancel",
                    null,
                    "Chef",
                    "Diner");

                if (!string.IsNullOrEmpty(result) && result != "Cancel")
                {
                    var newRole = UserRoleExtensions.ParseRole(result);
                    CurrentUserRole = newRole;

                    await SafeDisplayAlert(
                        "Role Changed",
                        $"Your role has been changed to {newRole.ToDisplayString()}",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error switching role: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to switch role", "OK");
            }
        }

        /// <summary>
        /// Manage categories 管理分类
        /// </summary>
        private async Task ManageCategoriesAsync()
        {
            try
            {
                await SafeDisplayAlert(
                    "Manage Categories",
                    "Category management feature coming soon!",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error managing categories: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to open category management", "OK");
            }
        }

        /// <summary>
        /// Show order history 显示订单历史
        /// </summary>
        private async Task OrderHistoryAsync()
        {
            try
            {
                await SafeDisplayAlert(
                    "Order History",
                    "Order history feature coming soon!",
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing order history: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to load order history", "OK");
            }
        }

        /// <summary>
        /// Show app information 显示应用信息
        /// </summary>
        private async Task AboutAppAsync()
        {
            try
            {
                var (version, build) = _userSettingsService.GetAppVersionInfo();
                var (platform, model) = _userSettingsService.GetDeviceInfo();

                var aboutMessage = $"DishMe Recipe App\n\n" +
                                 $"Version: {version} (Build {build})\n" +
                                 $"Platform: {platform}\n" +
                                 $"Device: {model}\n\n" +
                                 $"Developer: DishMe Team\n" +
                                 $"© 2024 DishMe. All rights reserved.\n\n" +
                                 $"A recipe management and ordering app\n" +
                                 $"designed for food lovers everywhere.";

                await SafeDisplayAlert(
                    "About DishMe",
                    aboutMessage,
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing app info: {ex.Message}");
                await SafeDisplayAlert("Error", "Failed to load app information", "OK");
            }
        }

        #endregion

        #region Helper Methods 辅助方法

        /// <summary>
        /// Safely display alert dialog 安全显示警告对话框
        /// </summary>
        private async Task<bool> SafeDisplayAlert(string title, string message, string accept, string? cancel = null)
        {
            try
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    if (cancel != null)
                        return await mainPage.DisplayAlert(title, message, accept, cancel);
                    else
                    {
                        await mainPage.DisplayAlert(title, message, accept);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying alert: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Safely display action sheet 安全显示操作表
        /// </summary>
        private async Task<string?> SafeDisplayActionSheet(string title, string cancel, string? destruction, params string[] buttons)
        {
            try
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    return await mainPage.DisplayActionSheet(title, cancel, destruction, buttons);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying action sheet: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}