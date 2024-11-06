using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AuthenticationProvider.Services;

public class WishlistService(IWishlistRepo wishlistRepo, UserManager<UserEntity> userManager) : IWishlistService
{
    private readonly IWishlistRepo _wishlistRepo = wishlistRepo;
    private readonly UserManager<UserEntity> _userManager = userManager;

    //create a new wishlist and add it to the user with the given id
    public async Task<bool> CreateWishlist(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var wishlist = new WishlistEntity
                {
                    UserId = userId,
                    User = user,
                    ProductIds = new List<string>()
                };

                await _wishlistRepo.CreateOneAsync(wishlist);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
        }
        return false;
    }

}
