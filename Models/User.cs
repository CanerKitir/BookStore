
public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public decimal Balance { get; set; }
    public required string Role { get; set; }
    public virtual Cart Cart { get; set; }
    public ICollection<Order> Orders { get; set; }
}
