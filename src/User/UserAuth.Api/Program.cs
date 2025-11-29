using System.Data;
using FluentMigrator.Runner;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Npgsql;
using UserAuth.Api.Configuration;
using UserAuth.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfig(builder.Configuration);

builder.Configuration
    .AddEnvironmentVariables(); // ← обязательно, иначе ConnectionStrings__DefaultConnection не прочитается

//добавляем механизм проверки бд, шлет запорлс в бд каждые 1-2 секунды, хранит состояние в памяти в сервисе HealthCheckService
builder.Services.AddHealthChecks()
    .AddNpgSql( 
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        name: "postgres",
        tags: new[] { "db", "postgres" });

var app = builder.Build();

//миграции
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    runner.MigrateUp();
}

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapAuthEndpoints();
app.MapUserEndpoints();

app.UseAuthentication();
app.UseAuthorization();

//создает эндпоинт для проверки состояния бд HealthCheckService
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
