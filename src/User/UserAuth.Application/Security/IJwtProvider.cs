using UserAuth.Domain.Entities;

namespace UserAuth.Application.Security;

public interface IJwtProvider
{
    string GenerateToken(User user);
}