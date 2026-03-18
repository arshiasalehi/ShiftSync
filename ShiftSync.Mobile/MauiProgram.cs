using Microsoft.Extensions.Logging;
using ShiftSync.Mobile.Services;
using ShiftSync.Mobile.ViewModels;
using ShiftSync.Mobile.Views;

namespace ShiftSync.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(GetApiBaseUrl())
        });

        builder.Services.AddSingleton<SessionState>();
        builder.Services.AddSingleton<ApiClient>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<JoinBusinessViewModel>();
        builder.Services.AddSingleton<AvailabilityViewModel>();
        builder.Services.AddSingleton<ShiftsViewModel>();
        builder.Services.AddSingleton<PayrollViewModel>();

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<JoinBusinessPage>();
        builder.Services.AddSingleton<AvailabilityPage>();
        builder.Services.AddSingleton<ShiftsPage>();
        builder.Services.AddSingleton<PayrollPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static string GetApiBaseUrl()
    {
#if ANDROID
        return "http://10.0.2.2:5008/";
#else
        return "http://localhost:5008/";
#endif
    }
}
