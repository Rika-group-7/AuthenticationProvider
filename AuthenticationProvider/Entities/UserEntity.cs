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


    // navigation for WishlistEntity
    public WishlistEntity Wishlist { get; set; } = null!;
}

public class WishlistEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;


    // Navigation property
    public UserEntity User { get; set; } = null!;

    //collection of Ids of the products
    public List<string> ProductIds { get; set; } = new List<string>();
}