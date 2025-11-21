namespace UserAuth.Application.Interfaces;

public interface IUserService
{
    Task Register(string email, string password, string name);
    
    Task<string> Login(string email, string password);
}