using DishMeAPP.ViewModels;
using Microsoft.Maui.Media;

namespace DishMeAPP.Views;

/*
 * Camera Setup Notes 相机设置说明:
 *
 * Android Emulator 安卓模拟器:
 * - Edit AVD → Advanced Settings → Camera: Set to "Webcam0" or "Emulated"
 * - 编辑AVD → 高级设置 → 相机：设置为"Webcam0"或"Emulated"
 *
 * iOS Simulator iOS模拟器:
 * - Camera not supported in simulator, use physical device
 * - 模拟器不支持相机，请使用真机测试
 *
 * Permissions 权限:
 * - Android: CAMERA permission in AndroidManifest.xml
 * - iOS: NSCameraUsageDescription in Info.plist
 */

/// <summary>
/// 菜谱页面后台代码
/// </summary>
public partial class RecipesPage : ContentPage
{
    /// <summary>
    /// 视图模型实例
    /// </summary>
    public RecipeViewModel ViewModel { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public RecipesPage()
    {
        InitializeComponent();

        // 初始化视图模型
        ViewModel = new RecipeViewModel();
        BindingContext = ViewModel;
    }

    /// <summary>
    /// 构造函数（带视图模型参数）
    /// </summary>
    /// <param name="viewModel">视图模型实例</param>
    public RecipesPage(RecipeViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel ?? new RecipeViewModel();
        BindingContext = ViewModel;
    }

    /// <summary>
    /// 页面出现时的处理
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 页面出现时的初始化逻辑
        // 例如：刷新数据、重置状态等
    }

    /// <summary>
    /// 页面消失时的处理
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // 页面消失时的清理逻辑
        // 例如：保存草稿、清理资源等
    }

    /// <summary>
    /// 处理返回按钮事件
    /// </summary>
    /// <returns>是否处理了返回事件</returns>
    protected override bool OnBackButtonPressed()
    {
        // 如果有未保存的更改，询问用户是否确认离开
        if (HasUnsavedChanges())
        {
            Dispatcher.Dispatch(async () =>
            {
                var result = await DisplayAlert(
                    "Confirm Leave",
                    "You have unsaved changes. Are you sure you want to leave?",
                    "Leave",
                    "Cancel");

                if (result)
                {
                    // 用户确认离开，执行导航
                    await Shell.Current.GoToAsync("..");
                }
            });

            // 阻止默认的返回行为
            return true;
        }

        // 允许默认的返回行为
        return base.OnBackButtonPressed();
    }

    /// <summary>
    /// 检查是否有未保存的更改
    /// </summary>
    /// <returns>是否有未保存的更改</returns>
    private bool HasUnsavedChanges()
    {
        if (ViewModel?.CurrentRecipe == null)
            return false;

        // 检查是否有任何字段被填写 Check if any fields have been filled
        return !string.IsNullOrWhiteSpace(ViewModel.RecipeName) ||
               !string.IsNullOrWhiteSpace(ViewModel.RecipeSteps) ||
               !string.IsNullOrWhiteSpace(ViewModel.RecipeIngredients) ||
               !string.IsNullOrWhiteSpace(ViewModel.RecipeImagePath);
    }

    /// <summary>
    /// Upload image event handler 上传图片事件处理函数
    /// </summary>
    /// <param name="sender">Event sender 事件发送者</param>
    /// <param name="e">Event arguments 事件参数</param>
    private async void UploadImage(object sender, EventArgs e)
    {
        try
        {
            // Show selection options 弹出选择操作方式
            var action = await DisplayActionSheet(
                "Select Image Source",
                "Cancel",
                null,
                "Take Photo",
                "Choose from Gallery");

            if (action == "Cancel") return;

            // Get photo based on user selection 根据用户选择获取照片
            FileResult? photo = action switch
            {
                "Take Photo" => await TakePhotoAsync(),
                "Choose from Gallery" => await PickPhotoFromGalleryAsync(),
                _ => null
            };

            if (photo != null)
            {
                // Save photo and update UI 保存照片并更新UI
                var localPath = await SavePhotoAsync(photo);
                ViewModel.RecipeImagePath = localPath;

                await DisplayAlert("Success", "Image uploaded successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Upload failed: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Take photo using camera 使用相机拍照
    /// Simplified implementation with inline permission handling 简化实现，内联权限处理
    /// </summary>
    /// <returns>Photo file result 拍照文件结果</returns>
    private async Task<FileResult?> TakePhotoAsync()
    {
        try
        {
            // Check and request camera permission 检查并请求相机权限
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    var shouldOpenSettings = await DisplayAlert(
                        "Camera Permission Required 需要相机权限",
                        "Camera access is needed to take photos. Open Settings? 需要相机权限来拍照，打开设置？",
                        "Settings 设置", "Cancel 取消");

                    if (shouldOpenSettings)
                    {
                        try
                        {
                            AppInfo.ShowSettingsUI();
                        }
                        catch (Exception settingsEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to open settings: {settingsEx.Message}");
                        }
                    }

                    return null;
                }
            }

            // Detailed device and camera diagnostics 详细的设备和相机诊断
            var platform = DeviceInfo.Platform;
            var deviceType = DeviceInfo.DeviceType;
            var isVirtual = deviceType == DeviceType.Virtual;
            var deviceModel = DeviceInfo.Model;
            var osVersion = DeviceInfo.VersionString;

            System.Diagnostics.Debug.WriteLine($"=== Camera Diagnostics 相机诊断 ===");
            System.Diagnostics.Debug.WriteLine($"Platform: {platform}");
            System.Diagnostics.Debug.WriteLine($"DeviceType: {deviceType}");
            System.Diagnostics.Debug.WriteLine($"IsVirtual: {isVirtual}");
            System.Diagnostics.Debug.WriteLine($"Model: {deviceModel}");
            System.Diagnostics.Debug.WriteLine($"OS Version: {osVersion}");

            // Check MediaPicker support 检查MediaPicker支持
            var mediaPickerSupported = MediaPicker.Default.IsCaptureSupported;
            System.Diagnostics.Debug.WriteLine($"MediaPicker.IsCaptureSupported: {mediaPickerSupported}");

            // Development mode: Force enable camera for testing 开发模式：强制启用相机用于测试
            var isSupported = mediaPickerSupported;

            if (!isSupported)
            {
                System.Diagnostics.Debug.WriteLine("Camera not supported by MediaPicker, applying development overrides...");

                // Force enable for mobile platforms (development/testing) 为移动平台强制启用（开发/测试）
                if (platform == DevicePlatform.Android || platform == DevicePlatform.iOS)
                {
                    System.Diagnostics.Debug.WriteLine("DEVELOPMENT MODE: Forcing camera support for mobile platform");
                    isSupported = true;
                }

                // Additional check: If we're in debug mode, be more lenient 额外检查：如果在调试模式，更宽松
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DEBUG MODE: Enabling camera for development");
                isSupported = true;
#endif
            }

            System.Diagnostics.Debug.WriteLine($"Final camera support decision: {isSupported}");
            System.Diagnostics.Debug.WriteLine("=== End Diagnostics 诊断结束 ===");

            if (!isSupported)
            {
                // Show detailed error with device info 显示包含设备信息的详细错误
                var errorMessage = $"Camera not supported on this device.\n" +
                                 $"Platform: {platform}\n" +
                                 $"Device: {deviceModel}\n" +
                                 $"Type: {deviceType}\n\n" +
                                 $"Use gallery instead?";

                var useGallery = await DisplayAlert(
                    "Camera Not Available",
                    errorMessage,
                    "Use Gallery", "Cancel");

                return useGallery ? await PickPhotoFromGalleryAsync() : null;
            }

            // Take photo 拍照
            System.Diagnostics.Debug.WriteLine("Attempting to launch camera...");

            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a photo for your recipe"
                });

                if (photo != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Camera success! Photo: {photo.FileName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Camera returned null (user cancelled)");
                }

                return photo;
            }
            catch (Exception cameraEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Camera launch failed: {cameraEx.GetType().Name}: {cameraEx.Message}");
                throw; // Re-throw to be caught by outer try-catch
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Not Supported", "Camera not supported", "OK");
            return null;
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission Denied", "Camera permission denied", "OK");
            return null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Camera error: {ex.Message}", "OK");
            return null;
        }
    }



    /// <summary>
    /// Pick photo from gallery 从相册选择照片
    /// Simplified implementation 简化实现
    /// </summary>
    /// <returns>Photo file result 照片文件结果</returns>
    private async Task<FileResult?> PickPhotoFromGalleryAsync()
    {
        try
        {
            // Check and request photo permission 检查并请求照片权限
            // Note: Photos permission is only required on iOS 14+ 注意：照片权限仅在iOS 14+上需要
#if IOS
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var photoStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (photoStatus != PermissionStatus.Granted)
                {
                    photoStatus = await Permissions.RequestAsync<Permissions.Photos>();
                    if (photoStatus != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permission Required",
                            "Photo access needed", "OK");
                        return null;
                    }
                }
            }
#endif

            // Pick from gallery 从相册选择
            return await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a photo for your recipe"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Gallery error: {ex.Message}", "OK");
            return null;
        }
    }

    /// <summary>
    /// Save photo to local storage 保存图片到本地存储
    /// </summary>
    /// <param name="photo">Photo file 图片文件</param>
    /// <returns>Local file path 本地文件路径</returns>
    private async Task<string> SavePhotoAsync(FileResult photo)
    {
        try
        {
            // Create local storage directory 创建本地存储目录
            var localPath = Path.Combine(FileSystem.AppDataDirectory, "Images");
            Directory.CreateDirectory(localPath);

            // Generate unique filename 生成唯一文件名
            var fileName = $"recipe_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.jpg";
            var filePath = Path.Combine(localPath, fileName);

            // Copy and compress file to local storage 复制并压缩文件到本地存储
            using var sourceStream = await photo.OpenReadAsync();
            using var localFileStream = File.Create(filePath);

            // Simple copy for now - you can add image compression here if needed
            // 简单复制 - 如果需要可以在这里添加图片压缩
            await sourceStream.CopyToAsync(localFileStream);

            return filePath;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Save Error 保存错误",
                $"Failed to save photo 保存照片失败: {ex.Message}",
                "OK 确定");
            throw;
        }
    }

    /// <summary>
    /// Test camera functionality - for debugging 测试相机功能 - 用于调试
    /// Call this method to test camera without UI interaction 调用此方法测试相机而无需UI交互
    /// </summary>
    public async Task TestCameraFunctionality()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== CAMERA TEST START 相机测试开始 ===");

            // Test 1: Check device info 测试1：检查设备信息
            System.Diagnostics.Debug.WriteLine($"Platform: {DeviceInfo.Platform}");
            System.Diagnostics.Debug.WriteLine($"DeviceType: {DeviceInfo.DeviceType}");
            System.Diagnostics.Debug.WriteLine($"Model: {DeviceInfo.Model}");

            // Test 2: Check MediaPicker availability 测试2：检查MediaPicker可用性
            try
            {
                var captureSupported = MediaPicker.Default.IsCaptureSupported;
                System.Diagnostics.Debug.WriteLine($"MediaPicker.IsCaptureSupported: {captureSupported}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking MediaPicker: {ex.Message}");
            }

            // Test 3: Check permissions 测试3：检查权限
            try
            {
                var cameraPermission = await Permissions.CheckStatusAsync<Permissions.Camera>();
                System.Diagnostics.Debug.WriteLine($"Camera Permission: {cameraPermission}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking camera permission: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("=== CAMERA TEST END 相机测试结束 ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera test failed: {ex}");
        }
    }
}
