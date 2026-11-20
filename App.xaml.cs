using DishMeAPP.Views;

namespace DishMeAPP;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
