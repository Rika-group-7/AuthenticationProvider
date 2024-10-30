using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationProvider.Entities;

public class UserEntity : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsAdmin { get; set; } = false; // Default value is false, can be changed when sending a model as httpPost request
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileDescription { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
}
