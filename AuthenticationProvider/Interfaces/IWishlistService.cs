using AuthenticationProvider.Entities;

namespace AuthenticationProvider.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> CreateWishlist(string userId);
        Task<WishlistEntity?> GetWishlist(string userId);
        Task<bool> AddProductToWishlist(string userId, string productId);
        Task<bool> RemoveProductFromWishlist(string userId, string productId);
        Task<bool> ClearWishlist(string userId);
    }
}