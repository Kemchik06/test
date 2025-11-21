namespace UserAuth.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public string HashedPassword { get; set; }
    
    public string Email { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
}