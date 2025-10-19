using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MoovieApp.Pages;
using MoovieApp.Services;
using MoovieApp.ViewModels;

namespace MoovieApp;

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
                fonts.AddFont("BBHSansHegarty-Regular.ttf", "BBHSansHegartyRegular");
                fonts.AddFont("BBHSansHegarty-Semibold.ttf", "BBHSansHegartySemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddHttpClient(TmdbService.HttpClientName,
            HttpClient => HttpClient.BaseAddress = new Uri("https://api.themoviedb.org"));

        builder.Services.AddSingleton<TmdbService>();
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailViewModel>();
        builder.Services.AddTransient<MovieDetailsPage>();

        return builder.Build();
    }
}
