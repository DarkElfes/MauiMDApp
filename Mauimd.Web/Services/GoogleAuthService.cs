using Blazored.LocalStorage;
using FluentResults;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Components;

namespace Mauimd.Web.Services;

public class GoogleAuthService(
    NavigationManager _navigation,
    ILocalStorageService _localStorage
    )
{
    public bool IsAuthorized { get; set; }


    private readonly IEnumerable<string> SCOPES = ["https://www.googleapis.com/auth/drive.file"];
    private readonly string REDIRECT_URI = "https://localhost:7123/auth-callback";
    private readonly string CLIENT_ID = "1083600727656-is84qk4ghu497nbhl7mh3mg1os0tl6fc.apps.googleusercontent.com";

    public async Task<Result> AuthorizeAsync()
    {
        string? status = await _localStorage.GetItemAsync<string>("AuthStatus");

        switch (status)
        {
            case null:
                await RedirectAsync();
                break;
            case "Authorized":
                return Result.Ok();
            case "Authorizing":
                return Result.Fail("Authorizing");
        }

        return Result.Ok();
    }

    public async Task ExchangeCodeForTokenAsync(string code)
    {
        var token = await GetFlow()
            .ExchangeCodeForTokenAsync(
                userId: "user",
                code: code,
                redirectUri: REDIRECT_URI,
                taskCancellationToken: CancellationToken.None);

        await _localStorage.SetItemAsync("AuthStatus", "Authorized");
        await _localStorage.SetItemAsync("Token", token);

        _navigation.NavigateTo("/");
    }



    private async Task RedirectAsync()
    {
        GoogleCredential credential = GoogleCredential.FromFile("client_secret.json")
               .CreateScoped(Oauth2Service.Scope.UserinfoProfile);

        // Create the Oauth2 service
        var service = new Oauth2Service(new BaseClientService.Initializer
        {
            HttpClientInitializer =,
            ApplicationName = "My Application"
        });

        Userinfo userInfo = service.Userinfo.Get().Execute();
        //var url = GetFlow()
        //    .CreateAuthorizationCodeRequest(REDIRECT_URI)
        //    .Build()
        //    .ToString();

        //await _localStorage.SetItemAsync("AuthStatus", "Authorizing");
        //_navigation.NavigateTo(url);
    }

    private GoogleAuthorizationCodeFlow GetFlow()
        => new(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets()
            {
                ClientId = CLIENT_ID,
            },
            Scopes = SCOPES,
        });
}

