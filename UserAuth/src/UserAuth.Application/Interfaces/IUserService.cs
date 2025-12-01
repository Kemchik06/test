using UserAuth.Application.Models;

namespace UserAuth.Application.Interfaces;

public interface IUserService
{
    Task Register(string email, string password, string name, CancellationToken token);
    
    Task<string> Login(string email, string password, CancellationToken token);
    
    Task<UserDto> GetById(Guid id, CancellationToken token);
}