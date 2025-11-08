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
                //fonts.AddFont("BBHSansHegarty-Semibold.ttf", "BBHSansHegartySemibold");
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
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<LoginPage>();

#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("media", (handler, view) =>
{
    if (handler.PlatformView is Android.Webkit.WebView androidWebView)
    {
        androidWebView.Settings.JavaScriptEnabled = true;
        androidWebView.Settings.MediaPlaybackRequiresUserGesture = false;
    }
});
#endif

#if IOS
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("media", (handler, view) =>
        {
            if (handler.PlatformView is WebKit.WKWebView wk)
            {
                wk.Configuration.AllowsInlineMediaPlayback = true;
            }
        });
#endif

        return builder.Build();
    }
}
