using UserAuth.Domain.Entities;
using UserAuth.Domain.Interfaces;
using UserAuth.Domain.Models;

namespace UserAuth.Application.Tests.Infrastructure;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    
    public Task AddAsync(User user, CancellationToken token)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User[]> SelectAsync(SelectUserModel model, CancellationToken token)
    {
        var users = _users.AsEnumerable();
        
        if (model.Ids is not null or { Length: > 0 })
        {
            users = users.Where(x => model.Ids.Contains(x.Id));
        }
        
        if (model.Emails is not null or { Length: > 0 })
        {
            users = users.Where(x => model.Emails.Contains(x.Email));
        }

        if (model.Offset is not null)
        {
            users = users.Skip(model.Offset.Value);
        }
        
        if (model.Limit is not null)
        {
            users = users.Take(model.Limit.Value);
        }
        
        return Task.FromResult(users.ToArray());
    }

    public Task UpdateAsync(User user, CancellationToken token)
    {
        var model = _users.FirstOrDefault(x => x.Id == user.Id);

        if (model is null)
        {
            return Task.CompletedTask;
        }
        
        model.Email = user.Email;
        model.Name = user.Name;
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken token)
    {
        _users.Remove(_users.FirstOrDefault(x => x.Id == id));
        return Task.CompletedTask;
    }
}