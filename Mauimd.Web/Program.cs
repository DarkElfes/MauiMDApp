using Blazored.LocalStorage;
using Mauimd.Web.Components;
using Mauimd.Web.Components.Auth;
using Mauimd.Web.Services;
using MauimdApp.Shared.Services;
using MauimdApp.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddAntDesign();

builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddSingleton<GoogleAuthService>();
builder.Services.AddSingleton<IStorageProvider, GoogleStorageProvider>();
builder.Services.AddSingleton<INoteManager, NoteManager>();
builder.Services.AddScoped<IMDConvertor, MDConvertor>();

await builder.Build().RunAsync();
 