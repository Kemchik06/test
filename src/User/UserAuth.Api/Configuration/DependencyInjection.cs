using System.Data;
using FluentMigrator.Runner;
using Microsoft.OpenApi;
using Npgsql;
using UserAuth.Api.Extensions;
using UserAuth.Application.Interfaces;
using UserAuth.Application.Security;
using UserAuth.Application.Services;
using UserAuth.Domain.Interfaces;
using UserAuth.Infrastructure.Repositories;
using UserAuth.Infrastructure.Security;

namespace UserAuth.Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Конфигурации
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        
        // 3. Инфраструктура (БД, Dapper, репозитории)
        services.AddInfrastructure(configuration);
        
        services.AddMigrations(configuration);

        // 4. Приложение (сервисы, провайдеры)
        services.AddApplication();

        // 5. API-слой (SwaggerGen, контроллеры и т.д.)
        services.AddApi();
        
        services.AddJwtAuthentication(configuration);

        return services;
    }
    
    private static void AddApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();   // ← критично для Minimal API
        services.AddOpenApi();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UserAuth API",
                Version = "v1",
            });
        });
    }

    private static void AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(UserAuth.Migrations.Migrations.CreateUsersTable).Assembly) // любой класс из миграций
                .For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
    
    
    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Вот сюда выносим всё, что связано с БД и Dapper
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true; // чтобы snake_case матчился с CamelCase
        
        services.AddScoped<IDbConnection>(sp =>
            new NpgsqlConnection(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddApplication(this IServiceCollection services)
    {
        // Это чистая бизнес-логика — не знает про БД и HTTP
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }

    // private static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddSerilog((sp, lc) => lc
    //         .ReadFrom.Configuration(configuration)
    //         .ReadFrom.Services(sp)
    //         .Enrich.FromLogContext()
    //         .Enrich.WithExceptionDetails()
    //         .Enrich.WithProperty("ServiceName", "LessonsService"));
    //     
    //     return services;
    // }
}