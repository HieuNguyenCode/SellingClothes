using Core.Entities;

namespace Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    string GetJtiFromToken(string token);
    string GetSubFromToken(string token);
}
