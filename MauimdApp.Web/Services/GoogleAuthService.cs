using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Blazored.LocalStorage;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests.Parameters;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace MauimdApp.Web.Services;

public class GoogleAuthService(
    ILocalStorageService _localStorage,
    NavigationManager _navigation
    )
{
    private const string RERIDIRECT_URI = "https://localhost:7276/auth/callback";
    private const string CLIENT_ID = "";
    private const string CLIENT_SECRET = "";

    private TokenResponse? _tokenResponse;
    private bool _isAuthorized;

    public async ValueTask<bool> IsAuthorizedAsync()
    {
        if (_isAuthorized && _tokenResponse != null)
            return true;

        var tokenResponse = await _localStorage.GetItemAsync<TokenResponse>("token_response");

        if (tokenResponse is null)
            return false;

        Console.WriteLine("Some Message");

        Console.WriteLine(tokenResponse.IssuedUtc + " | " + DateTime.UtcNow);

        Console.WriteLine(tokenResponse.RefreshToken);

        if (tokenResponse.IsStale)
        {
            await _localStorage.RemoveItemAsync("token_response");
            await RefreshTokenAsync(tokenResponse.RefreshToken);
            Console.WriteLine("Token refreshed");
        }
        else
        {
            _tokenResponse = tokenResponse;
        }

        _isAuthorized = true;
        return true;
    }


    public void Authorize()
    {
        var redirectUri = GetFlow()
            .CreateAuthorizationCodeRequest(RERIDIRECT_URI)
            .Build();
        
        _navigation.NavigateTo(redirectUri.AbsoluteUri, true);
    }

    private async Task SetTokenResponseAsync(TokenResponse tokenResponse)
    {
        _tokenResponse = tokenResponse;
        await _localStorage.SetItemAsync("token_response", tokenResponse);
        _isAuthorized = true;
    }


    private async Task RefreshTokenAsync(string refreshToken)
    {
        var tokenResponse = await GetFlow()
            .RefreshTokenAsync("user", refreshToken, CancellationToken.None);

        if (tokenResponse is null)
            return;

        await SetTokenResponseAsync(tokenResponse);
    }

    public async Task ExchangeCodeAsync(string code)
    {
        //var tokenResponse = await GetFlow()
        //    .ExchangeCodeForTokenAsync("user", code, RERIDIRECT_URI, CancellationToken.None);

        var tokenRequest = new AuthorizationCodeTokenRequest()
        {
            RedirectUri = RERIDIRECT_URI,
            Scope = DriveService.ScopeConstants.DriveFile,
            Code = code,
            ClientId = CLIENT_ID,
            ClientSecret = CLIENT_SECRET,
        };

        using HttpClient client = new();
        using HttpRequestMessage request = new(HttpMethod.Post, "https://accounts.google.com/o/oauth2/v2/auth");

        request.Content = ParameterUtils.CreateFormUrlEncodedContent(request);

        var result = await client.SendAsync(request);

        var str = await result.Content.ReadAsStringAsync();
        Console.WriteLine(str);
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(str);

        Console.WriteLine(tokenResponse);
        if (tokenResponse is null)
            return;

        Console.WriteLine(tokenResponse);

        await SetTokenResponseAsync(tokenResponse);
    }

    public UserCredential GetUserCredential()
    {
        if (_tokenResponse is null)
            throw new InvalidOperationException("Token response is null");
        return new UserCredential(GetFlow(), "user", _tokenResponse);
    }


    private static GoogleAuthorizationCodeFlow GetFlow()
    {
        return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = CLIENT_ID,
                ClientSecret = CLIENT_SECRET
            },
            Scopes = [DriveService.ScopeConstants.DriveFile]
        });
    }



}
