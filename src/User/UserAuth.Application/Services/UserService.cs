using UserAuth.Application.Interfaces;
using UserAuth.Application.Security;
using UserAuth.Domain.Entities;
using UserAuth.Domain.Interfaces;

namespace UserAuth.Application.Services;

public class UserService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }
    
    public async Task Register(string email, string password, string name)
    {
        // Hash the password before storing it
        var hashedPassword = _passwordHasher.HashPassword(password);
        
        await _userRepository.AddAsync(new User
        {
            Email = email,
            HashedPassword = hashedPassword,
            Name = name
        });
    }
    
    public async Task<string> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new Exception($"User with email {email} not found");
        }
        
        var result =_passwordHasher.VerifyPassword(password, user.HashedPassword);
        
        if (!result)
        {
            throw new Exception("Invalid password");
        }
        
        return _jwtProvider.GenerateToken(user);
    }
}