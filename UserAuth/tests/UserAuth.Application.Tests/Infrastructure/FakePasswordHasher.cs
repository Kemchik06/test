using UserAuth.Application.Security;

namespace UserAuth.Application.Tests.Infrastructure;

public class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return "hashed" + password;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return hashedPassword == HashPassword(password);
    }
}