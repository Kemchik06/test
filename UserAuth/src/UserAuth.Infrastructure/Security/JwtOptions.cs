namespace UserAuth.Infrastructure.Security;

public class JwtOptions
{
    public string SecretKey { get; set; }
    
    public int ExoireHours { get; set; }
}