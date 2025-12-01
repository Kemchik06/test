using System.Security.Claims;
using UserAuth.Api.Contracts;
using UserAuth.Application.Interfaces;

namespace UserAuth.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
            .RequireAuthorization()
            .WithTags("Users");

        group.MapPost("/me", Me)
            .WithName("GetUserById")
            .Produces<MeUserResponse>(200)
            .ProducesProblem(404);
        
        return app;
    }

    private static async Task<IResult> Me(IUserService userService, ClaimsPrincipal userCalims, CancellationToken token)
    {
        var userIdClaim = userCalims.FindFirst("userId") 
                          ?? userCalims.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();
        
        var user = await userService.GetById(userId, token);
        
        return Results.Ok(new MeUserResponse
        {
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        });
    }
}