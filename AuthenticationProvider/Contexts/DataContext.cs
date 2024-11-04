using AuthenticationProvider.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationProvider.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<WishlistEntity> Wishlists { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WishlistEntity>()
            .HasOne(w => w.User)
            .WithOne(u => u.Wishlist)
            .HasForeignKey<WishlistEntity>(w => w.UserId);
    }
}
