using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace DishMeAPP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });



#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
#endif

        return builder.Build();
    }
}
