using AuthenticationProvider.Contexts;
using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;

namespace AuthenticationProvider.Repositories
{
    public class WishlistRepo(DataContext context) : BaseRepo<WishlistEntity>(context), IWishlistRepo
    {
        private readonly DataContext _context = context;
    }

}
