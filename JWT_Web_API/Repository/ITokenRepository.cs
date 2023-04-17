using JWT_Web_API.Models;

namespace JWT_Web_API.Repository
{
    public interface ITokenRepository
    {
        Tokens Authenticate(Users users);
    }
}
