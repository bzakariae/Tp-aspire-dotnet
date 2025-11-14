using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "keycloak_token";
    private readonly ILocalStorageService _localStorage;
    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(Anonymous);

        var principal = CreateClaimsPrincipalFromJwt(token);
        return new AuthenticationState(principal);
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        var principal = CreateClaimsPrincipalFromJwt(token);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private ClaimsPrincipal CreateClaimsPrincipalFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        // exp check (nouvelle API)
        var exp = jwt.Payload.Expiration; // long?
        if (exp.HasValue &&
            DateTimeOffset.UtcNow >= DateTimeOffset.FromUnixTimeSeconds(exp.Value))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = new List<Claim>(jwt.Claims);

        // Name / Email
        if (!string.IsNullOrEmpty(jwt.Subject))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, jwt.Subject));
        if (jwt.Payload.TryGetValue("preferred_username", out var pu) && pu is string u)
            claims.Add(new Claim(ClaimTypes.Name, u));
        if (jwt.Payload.TryGetValue("email", out var em) && em is string e) // 👈 parenthèse corrigée
            claims.Add(new Claim(ClaimTypes.Email, e));

        // Rôles: "roles"
        foreach (var r in jwt.Claims.Where(c => c.Type == "roles"))
            claims.Add(new Claim(ClaimTypes.Role, r.Value));

        // Rôles: realm_access.roles
        var realmAccess = jwt.Payload.TryGetValue("realm_access", out var ra) ? ra as IDictionary<string, object> : null;
        if (realmAccess != null && realmAccess.TryGetValue("roles", out var rr) && rr is IEnumerable<object> arr1)
            foreach (var r in arr1.OfType<string>())
                claims.Add(new Claim(ClaimTypes.Role, r));

        // Rôles: resource_access.{client}.roles
        var resourceAccess = jwt.Payload.TryGetValue("resource_access", out var res) ? res as IDictionary<string, object> : null;
        if (resourceAccess != null)
            foreach (var kv in resourceAccess)
                if (kv.Value is IDictionary<string, object> client && client.TryGetValue("roles", out var r2) && r2 is IEnumerable<object> arr2)
                    foreach (var r in arr2.OfType<string>())
                        claims.Add(new Claim(ClaimTypes.Role, r));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }


}
