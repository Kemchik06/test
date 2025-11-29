namespace UserAuth.Domain.Models;

public class SelectUserModel
{
    public Guid[] Ids { get; set; }

    public string[] Emails { get; set; }

    public DateTimeOffset? CreatedFrom { get; set; }
    
    public DateTimeOffset? CreatedTo { get; set; }

    public int? Limit { get; set; }
    
    public int? Offset { get; set; }
}