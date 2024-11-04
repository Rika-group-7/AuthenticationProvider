using AuthenticationProvider.Contexts;
using AuthenticationProvider.Entities;
using AuthenticationProvider.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationProvider.Tests.Repositories_Tests;

public class WishlistRepo_Tests
{
    private readonly DataContext _context;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly WishlistRepo _wishlistRepo;

    public WishlistRepo_Tests()
    {
        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _context = new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        _wishlistRepo = new WishlistRepo(_context);
    }

    [Fact]
    public async void CreateOne_ShouldCreateOneWishlist_AndReturnEntity()
    {
        // Arrange
        var user = new UserEntity
        {
            Id = "1",
            Email = "test@example.com"
        };

        var wishlist = new WishlistEntity
        {
            UserId = user.Id,
            User = user,
            ProductIds = new List<string> { "1", "2", "3" }
        };

        // Act
        var result = await _wishlistRepo.CreateOneAsync(wishlist);

        // Assert
        var savedWishlist = await _context.Wishlists.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == wishlist.Id);

        Assert.NotNull(result);
        Assert.Equal(wishlist.UserId, savedWishlist.UserId);
        Assert.Equal(wishlist.ProductIds, savedWishlist.ProductIds);
    }
}
