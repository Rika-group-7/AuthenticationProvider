using AuthenticationProvider.Entities;

namespace AuthenticationProvider.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(UserEntity user);
    }
}
