namespace UserAuth.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public string HashedPassword { get; set; }
    
    public string Email { get; set; }
    
    public string Name { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public bool IsActive { get; set; }
}