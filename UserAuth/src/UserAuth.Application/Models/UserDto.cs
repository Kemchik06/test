namespace UserAuth.Application.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}