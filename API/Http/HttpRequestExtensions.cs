namespace API.Http;

public static class HttpRequestExtensions
{
    public static string GetBearerToken(this HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) ||
            !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        return authHeader["Bearer ".Length..].Trim();
    }
}