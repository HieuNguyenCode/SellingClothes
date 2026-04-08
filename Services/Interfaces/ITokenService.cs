using Core.Entities;

namespace Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Users user);
    string GenerateRefreshToken(Users user);
    string GetJtiFromToken(string token);
    string GetSubFromToken(string token);
}
