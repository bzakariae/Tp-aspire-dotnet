namespace MyDotNetApp.ApiService.Services;

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var id = (ClaimsIdentity)principal.Identity!;
        var rolesAlreadyMapped = id.FindFirst(ClaimTypes.Role) != null;
        if (rolesAlreadyMapped) return Task.FromResult(principal);

        var access = id.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrEmpty(access))
        {
            using var doc = JsonDocument.Parse(access);
            if (doc.RootElement.TryGetProperty("roles", out var roles) && roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var r in roles.EnumerateArray())
                    id.AddClaim(new Claim(ClaimTypes.Role, r.GetString()!));
            }
        }

        var resAccess = id.FindFirst("resource_access")?.Value;
        if (!string.IsNullOrEmpty(resAccess))
        {
            using var doc = JsonDocument.Parse(resAccess);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.TryGetProperty("roles", out var roles) && roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (var r in roles.EnumerateArray())
                        id.AddClaim(new Claim(ClaimTypes.Role, r.GetString()!));
                }
            }
        }

        return Task.FromResult(principal);
    }
}
