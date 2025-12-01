using UserAuth.Application.Interfaces;
using UserAuth.Application.Models;
using UserAuth.Application.Security;
using UserAuth.Domain.Entities;
using UserAuth.Domain.Interfaces;
using UserAuth.Domain.Models;

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
    
    public async Task Register(string email, string password, string name, CancellationToken token)
    {
        var hashedPassword = _passwordHasher.HashPassword(password);
        
        await _userRepository.AddAsync(new User
        {
            Email = email,
            HashedPassword = hashedPassword,
            Name = name
        }, token);
    }
    
    public async Task<string> Login(string email, string password, CancellationToken token)
    {
        var userQuery = await _userRepository.SelectAsync(new SelectUserModel
        {
            Emails = [email],
            Limit = 1
        }, token);

        var user = userQuery.FirstOrDefault();
        if (user == null)
        {
            throw new Exception($"User with email {email} not found");
        }
        
        var result = _passwordHasher.VerifyPassword(password, user.HashedPassword);
        if (!result)
        {
            throw new Exception("Invalid password");
        }
        
        return _jwtProvider.GenerateToken(user);
    }

    public async Task<UserDto> GetById(Guid id, CancellationToken token)
    {
        var userQuery = await _userRepository.SelectAsync(new SelectUserModel
        {
            Ids = [id],
            Limit = 1
        }, token);

        var user = userQuery.FirstOrDefault();

        if (user is null)
        {
            throw new Exception("User not found");
        }

        return new UserDto
        {
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }
}