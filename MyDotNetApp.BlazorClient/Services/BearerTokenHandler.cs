namespace MyDotNetApp.BlazorClient.Services;

using System.Net.Http.Headers;
using Blazored.LocalStorage;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly ILocalStorageService _ls;
    public BearerTokenHandler(ILocalStorageService ls) => _ls = ls;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var token = await _ls.GetItemAsync<string>("keycloak_token");
        if (!string.IsNullOrWhiteSpace(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(req, ct);
    }
}