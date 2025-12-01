using UserAuth.Application.Security;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Tests.Infrastructure;

public class StubJwtProvider : IJwtProvider
{
    public string GenerateToken(User user)
    {
        return "auth_token";
    }
}