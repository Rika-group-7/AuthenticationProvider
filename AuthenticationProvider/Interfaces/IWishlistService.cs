namespace AuthenticationProvider.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> CreateWishlist(string userId);
    }
}