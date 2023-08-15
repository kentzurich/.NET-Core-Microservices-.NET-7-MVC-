using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services.JwtTokenGenerator
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
