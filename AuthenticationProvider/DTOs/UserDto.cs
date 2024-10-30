namespace AuthenticationProvider.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsAdmin { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileDescription { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
}
