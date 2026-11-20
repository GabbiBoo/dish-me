using DishMeAPP.ViewModels;
using Microsoft.Maui.Controls;

namespace DishMeAPP.Views;

/// <summary>
/// Order page code-behind 点单页面后台代码
/// </summary>
public partial class OrderPage : ContentPage
{
    /// <summary>View-model instance 视图模型实例</summary>
    public OrderViewModel ViewModel { get; }

    /// <summary>Parameter-less constructor 默认构造函数</summary>
    public OrderPage()
    {
        InitializeComponent();                 // 只调用，不实现
        ViewModel = new OrderViewModel();
        BindingContext = ViewModel;
    }

    /// <summary>Constructor with injected view-model 带视图模型参数的构造函数</summary>
    public OrderPage(OrderViewModel viewModel)
    {
        InitializeComponent();                 // 只调用，不实现
        ViewModel = viewModel ?? new OrderViewModel();
        BindingContext = ViewModel;
    }


    /// <summary>
    /// Page appearing handler 页面出现时的处理
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Page appearing initialization logic 页面出现时的初始化逻辑
        // For example: refresh data, reset state, etc. 例如：刷新数据、重置状态等
    }

    /// <summary>
    /// Page disappearing handler 页面消失时的处理
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Page disappearing cleanup logic 页面消失时的清理逻辑
        // For example: save draft, cleanup resources, etc. 例如：保存草稿、清理资源等
    }

    /// <summary>
    /// Handle back button press 处理返回按钮事件
    /// </summary>
    /// <returns>Whether handled the back event 是否处理了返回事件</returns>
    protected override bool OnBackButtonPressed()
    {
        try
        {
            // Allow default back behavior 允许默认返回行为
            return base.OnBackButtonPressed();
        }
        catch (Exception)
        {
            // Handle any platform-specific exceptions 处理平台特定异常
            return false;
        }
    }
}