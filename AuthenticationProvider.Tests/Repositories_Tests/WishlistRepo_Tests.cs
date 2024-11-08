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

    [Fact]
    public async Task GetWishlistByUserIdAsync_ShouldReturnWishlist_IfExists()
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

        await _wishlistRepo.CreateOneAsync(wishlist); 

        // Act
        var result = await _wishlistRepo.GetWishlistByUserIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wishlist.UserId, result.UserId);
        Assert.Equal(wishlist.ProductIds, result.ProductIds);
    }

    [Fact]
    public async Task GetWishlistByUserIdAsync_ShouldReturnNull_IfNotExists()
    {
        // Act
        var result = await _wishlistRepo.GetWishlistByUserIdAsync("non-existent-user-id");

        // Assert
        Assert.Null(result); 
    }

    [Fact]
    public async Task DeleteOne_ShouldDeleteWishlist_AndReturnTrue()
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

        await _wishlistRepo.CreateOneAsync(wishlist);

        // Act
        var result = await _wishlistRepo.DeleteOne(w => w.UserId == user.Id);

        // Assert
        Assert.True(result);

        var deletedWishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == user.Id);
        Assert.Null(deletedWishlist);
    }

    [Fact]
    public async Task DeleteOne_ShouldReturnFalse_WhenWishlistNotFound()
    {
        // Act
        var result = await _wishlistRepo.DeleteOne(w => w.UserId == "non-existent-user-id");

        // Assert
        Assert.False(result); 
    }

    [Fact]
    public async Task UpdateOne_ShouldUpdateExistingWishlist_AndReturnUpdatedEntity()
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

        await _wishlistRepo.CreateOneAsync(wishlist); 

        wishlist.ProductIds.Add("4"); 

        // Act
        var updatedResult = await _wishlistRepo.UpdateOne(wishlist);

        // Assert
        var updatedWishlist = await _context.Wishlists.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == wishlist.Id);
        Assert.NotNull(updatedWishlist);
        Assert.Contains("4", updatedWishlist.ProductIds);
    }
}
