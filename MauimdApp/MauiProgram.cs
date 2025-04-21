using MauimdApp.Services;
using MauimdApp.Shared.Services;
using MauimdApp.Shared.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace MauimdApp
{
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddAntDesign();


            //Add Services
#if WINDOWS
            builder.Services.AddSingleton<IStorageProvider, WindowsStorageProvider>();
#endif
            builder.Services.AddSingleton<INoteManager, NoteManager>();
            builder.Services.AddScoped<IMDConvertor, MDConvertor>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
