using CommunityToolkit.Maui;
using ConnectivityLibrary.Services;
using FoodopolyClientV2.ViewModels;
using FoodopolyClientV2.Views;
using FoodopolyClientV2.Views.Popups;
//using ConnectivityLibrary.Services; service static atm, so not addiing it here?

namespace FoodopolyClientV2;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();

        builder.Services.AddTransient<GameJoinPage>();
        builder.Services.AddTransient<GameJoinViewModel>();

        builder.Services.AddTransient<GamePage>();
        builder.Services.AddTransient<GameViewModel>();

        //builder.Services.AddTransient<PropertyPopup>();

        //services
        builder.Services.AddTransient<SignalRHubServices>();

        return builder.Build();
    }
}
