using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Services.Features;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Iduser.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.Role, user.Role)
        };
        var minutes = Convert.ToDouble(configuration["Jwt:ExpireMinutes"] ?? "60");
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        return CreateToken(claims, expires);
    }

    public string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Iduser.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };
        var days = Convert.ToDouble(configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
        var expires = DateTime.UtcNow.AddDays(days);

        return CreateToken(claims, expires);
    }

    public string GetJtiFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        token = token.Replace("Bearer ", "").Trim();

        if (string.IsNullOrEmpty(token) || !tokenHandler.CanReadToken(token)) return string.Empty;

        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public string GetSubFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        token = token.Replace("Bearer ", "").Trim();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken) return string.Empty;

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        return subClaim?.Value ?? string.Empty;
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expires)
    {
        var keyStr = configuration["Jwt:Key"] ?? string.Empty;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}