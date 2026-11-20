using DishMeAPP.ViewModels;

namespace DishMeAPP.Views
{
    /// <summary>
    /// Mine page code-behind 我的页面代码后台
    /// </summary>
    public partial class MinePage : ContentPage
    {
        public MinePage()
        {
            InitializeComponent();
            BindingContext = new MineViewModel();
        }
    }
}
