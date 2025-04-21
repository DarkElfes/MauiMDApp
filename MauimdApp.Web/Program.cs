using Blazored.LocalStorage;
using MauimdApp.Shared.Services;
using MauimdApp.Shared.Services.Abstractions;
using MauimdApp.Web;
using MauimdApp.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAntDesign();
builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddSingleton<GoogleAuthService>();
 
builder.Services.AddSingleton<IStorageProvider, GoogleStorageProvider>();
builder.Services.AddSingleton<INoteManager, NoteManager>();
builder.Services.AddScoped<IMDConvertor, MDConvertor>();

await builder.Build().RunAsync();
