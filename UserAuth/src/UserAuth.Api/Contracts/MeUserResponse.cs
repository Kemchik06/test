namespace UserAuth.Api.Contracts;

public class MeUserResponse
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}