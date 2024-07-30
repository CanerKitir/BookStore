public class UserDto : UserSecurityDto
{

    public required string Role { get; set; }

}

public class UserSecurityDto
{
    public int Id { get; set; }
    public required string Username { get; set; }

    public required string Password { get; set; }

    public decimal Balance { get; set; }
}