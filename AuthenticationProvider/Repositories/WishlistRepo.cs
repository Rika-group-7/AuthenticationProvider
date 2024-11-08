using AuthenticationProvider.Contexts;
using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationProvider.Repositories;

public class WishlistRepo : BaseRepo<WishlistEntity>, IWishlistRepo
{
    private readonly DataContext _context;

    public WishlistRepo(DataContext context) : base(context)
    {
        _context = context;
    }
    public async Task<WishlistEntity?> GetWishlistByUserIdAsync(string userId)
    {
        return await _context.Wishlists
                             .Where(w => w.UserId == userId)
                             .FirstOrDefaultAsync();
    }
}
