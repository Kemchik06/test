using System.Data;
using Dapper;
using UserAuth.Domain.Entities;
using UserAuth.Domain.Interfaces;

namespace UserAuth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM \"Users\" WHERE \"Id\" = @Id";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = "SELECT * FROM \"Users\" WHERE \"Email\" = @Email LIMIT 1";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = "SELECT * FROM \"Users\" ORDER BY \"CreatedAt\" DESC";
        return await _db.QueryAsync<User>(sql);
    }

    public async Task AddAsync(User user)
    {
        const string sql = @"
            INSERT INTO ""Users"" (""Id"", ""Email"", ""Name"", ""CreatedAt"")
            VALUES (@Id, @Email, @Name, @CreatedAt)";

        await _db.ExecuteAsync(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE ""Users""
            SET ""Email"" = @Email,
                ""Name"" = @Name
            WHERE ""Id"" = @Id";

        await _db.ExecuteAsync(sql, user);
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM \"Users\" WHERE \"Id\" = @Id";
        await _db.ExecuteAsync(sql, new { Id = id });
    }
}