using UserAuth.Api.Contracts;
using UserAuth.Application.Interfaces;

namespace UserAuth.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login",Login)
            .WithName("Login").WithDescription("Authenticate user and return JWT token.");

        group.MapPost("/register", Register)
            .WithName("Register").WithDescription("Register a new user.");

        return app;
    }

    private static async Task<IResult> Register(IUserService userService, RegisterRequest request)
    {
        await userService.Register(request.Email, request.Password, request.Name);
        
        return Results.Ok();
    }
    
    
    private static async Task<IResult> Login(IUserService userService, LoginRequest request)
    {
        await  userService.Login(request.Email, request.Password);
        return Results.Ok();
    }
}