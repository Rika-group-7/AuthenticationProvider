using AuthenticationProvider.Entities;

namespace AuthenticationProvider.Interfaces;

public interface IWishlistRepo : IBaseRepo<WishlistEntity>
{
    Task<WishlistEntity?> GetWishlistByUserIdAsync(string userId);
}
