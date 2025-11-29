using UserAuth.Domain.Entities;
using UserAuth.Domain.Models;

namespace UserAuth.Domain.Interfaces;

public interface IUserRepository
{
    
    Task AddAsync(User user, CancellationToken token);
    Task<User[]> SelectAsync(SelectUserModel model, CancellationToken token);
    Task UpdateAsync(User user, CancellationToken token);
    Task DeleteAsync(Guid id, CancellationToken token);
}