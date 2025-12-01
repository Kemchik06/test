using System.Data;
using System.Text;
using Dapper;
using UserAuth.Domain.Entities;
using UserAuth.Domain.Interfaces;
using UserAuth.Domain.Models;

namespace UserAuth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<User[]> SelectAsync(SelectUserModel model, CancellationToken token)
    {
        var sql = new StringBuilder(@"
        SELECT id, email, name, hashed_password, created_at, is_active
        FROM users
        WHERE 1=1");

        var parameters = new DynamicParameters();

        if (model.Ids != null && model.Ids.Length > 0)
        {
            sql.Append($" AND id = ANY(@{nameof(model.Ids)})");
            parameters.Add($"@{nameof(model.Ids)}", model.Ids);
        }

        if (model.Emails != null && model.Emails.Length > 0)
        {
            sql.Append($" AND email = ANY(@{nameof(model.Emails)})");
            parameters.Add($"@{nameof(model.Emails)}", model.Emails);
        }
        
        if(model.CreatedFrom != null)
        {
            sql.Append($" AND \"CreatedAt\" >= @{nameof(model.CreatedFrom)}");
            parameters.Add($"@{nameof(model.CreatedFrom)}", model.CreatedFrom.Value);
        }
        
        if (model.CreatedTo.HasValue)
        {
            sql.Append($" AND createdAt < @{nameof(model.CreatedTo)}");
            parameters.Add($"@{nameof(model.CreatedTo)}", model.CreatedTo.Value);
        }
        
        sql.Append(" ORDER BY name DESC");
        
        if (model.Limit.HasValue)
        {
            sql.Append($" LIMIT @{nameof(model.Limit)}");
            parameters.Add($"@{nameof(model.Limit)}", model.Limit.Value);
        }

        if (model.Offset.HasValue)
        {
            sql.Append($" OFFSET @{nameof(model.Offset)}");
            parameters.Add($"{nameof(model.Offset)}", model.Offset.Value);
        }
        
        var users = await _db.QueryAsync<User>(
            new CommandDefinition(sql.ToString(), 
                parameters, 
                cancellationToken: token));
        
        return users.ToArray();
    }
    
    public async Task AddAsync(User user, CancellationToken token)
    {
        const string sql = @"
            INSERT INTO users (email, hashed_password, name)
            VALUES (@Email, @HashedPassword, @Name)";
        
        await _db.ExecuteAsync(new CommandDefinition(sql, new
        {
            user.Email,
            user.HashedPassword,
            user.Name,
        }, cancellationToken: token));
    }

    public async Task UpdateAsync(User user, CancellationToken token)
    {
        const string sql = @"
            UPDATE users
            SET email = COALESCE(@Email, email),
                name = COALESCE(@Name, name),
            WHERE id = (@Id)";

        var parameters = new
        {
            user.Email,
            user.Name,
            user.Id
        };

        await _db.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: token));
    }

    public async Task DeleteAsync(Guid id, CancellationToken token)
    {
        const string sql = "DELETE FROM users WHERE id = @Id";
        
        await _db.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: token));
    }
}