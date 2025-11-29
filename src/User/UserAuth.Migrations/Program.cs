// UserAuth.Migrations/Program.cs

using System;
using System.IO;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

// Строим конфигурацию один раз — правильно и безопасно
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables() // Переменные окружения перекрывают всё
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Connection string 'DefaultConnection' not found. " +
                           "Set it via environment variable ConnectionStrings__DefaultConnection or DATABASE_URL");

var services = new ServiceCollection()
    .AddFluentMigratorCore() //Регистрируем базовые сервисы FluentMigrator
    .ConfigureRunner(rb => rb
        .AddPostgres()//подключается драйвер Npgsql
        .WithGlobalConnectionString(connectionString)//Указываем строку подключения
        .ScanIn(typeof(Program).Assembly).For.Migrations())// классы с атрибутом migration
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);//собираем контейнер

using var scope = services.CreateScope();// создали scope контейнера
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

if (args.Length > 0)
{
    switch (args[0].ToLowerInvariant())
    {
        case "--down":
        case "down":
            Console.WriteLine("Откатываем все миграции до 0...");
            runner.MigrateDown(0);
            break;

        case "--list":
        case "list":
            runner.ListMigrations();
            break;

        case "--migrate":
        case "up":
        default:
            Console.WriteLine("Применяем все новые миграции (MigrateUp)...");
            runner.MigrateUp();
            break;
    }
}
else
{
    Console.WriteLine("Нет аргументов → применяем все новые миграции (MigrateUp)");
    runner.MigrateUp();
}

Console.WriteLine("Миграции завершены успешно.");
return 0; // важно для Docker/K8s — код возврата 0 = успех
