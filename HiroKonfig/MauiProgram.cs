using HiroKonfig.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace HiroKonfig;

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
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		builder.Services
			.AddScoped<IAuthenticationService, AuthenticationService>()
			.AddScoped<IUserService, UserService>()
			.AddScoped<IHttpService, HttpService>()
			.AddScoped<ILocalStorageService, LocalStorageService>()
			.AddScoped<IAktionService, AktionService>()
			.AddScoped<IKundeService, KundeService>();


        builder.Services.AddScoped(x => {
            // var apiUrl = new Uri(builder.Configuration["apiUrl"]);
            // var apiUrl = new Uri("https://localhost:5001");
            var apiUrl = new Uri("https://hlws.hirolift.de");

            return new HttpClient() { BaseAddress = apiUrl };
        });


        return builder.Build();
	}
}
