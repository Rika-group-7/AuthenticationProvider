using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;

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

    // get wishlist according to the user
    public async Task<WishlistEntity?> GetWishlist(string userId)
    {
        try
        {
            var predicate = (Expression<Func<WishlistEntity, bool>>)(w => w.UserId == userId);
            return await _wishlistRepo.GetOneAsync(predicate);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return null;
        }
    }

    // add product to wishlist
    public async Task<bool> AddProductToWishlist(string userId, string productId)
    {
        try
        {
            var wishlist = await GetWishlist(userId);
            if (wishlist != null && !wishlist.ProductIds.Contains(productId))
            {
                wishlist.ProductIds.Add(productId);
                await _wishlistRepo.UpdateOne(wishlist);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return false;
        }
    }

    // remove one product from the wishlist
    public async Task<bool> RemoveProductFromWishlist(string userId, string productId)
    {
        try
        {
            var wishlist = await GetWishlist(userId);
            if (wishlist != null && wishlist.ProductIds.Contains(productId))
            {
                wishlist.ProductIds.Remove(productId);
                await _wishlistRepo.UpdateOne(wishlist);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return false;
        }
    }

    // clear all products from wishlist
    public async Task<bool> ClearWishlist(string userId)
    {
        try
        {
            var wishlist = await GetWishlist(userId);
            if (wishlist != null)
            {
                wishlist.ProductIds.Clear();
                await _wishlistRepo.UpdateOne(wishlist);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return false;
        }
    }

}
