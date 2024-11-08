using AuthenticationProvider.Contexts;
using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationProvider.Tests.Services_Tests;

public class WishlistService_Tests
{
    private readonly IWishlistService _wishlistService;
    private readonly Mock<IWishlistRepo> _wishlistRepoMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;

    public WishlistService_Tests()
    {

        _wishlistRepoMock = new Mock<IWishlistRepo>();
        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _wishlistService = new WishlistService(_wishlistRepoMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task CreateWishlist_ShouldReturnTrue_IfUserExists()
    {
        // Arrange
        var userId = "123test";
        var user = new UserEntity
        {
            Id = userId,
            Email = "test@example.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.CreateOneAsync(It.IsAny<WishlistEntity>())).ReturnsAsync(new WishlistEntity());

        // Act
        var result = await _wishlistService.CreateWishlist(userId);

        // Assert
        Assert.True(result);
        _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
        _wishlistRepoMock.Verify(x => x.CreateOneAsync(It.IsAny<WishlistEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateWishlist_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "non-existent-user-id";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _wishlistService.CreateWishlist(userId);

        // Assert
        Assert.False(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
        _wishlistRepoMock.Verify(repo => repo.CreateOneAsync(It.IsAny<WishlistEntity>()), Times.Never);
    }
    [Fact]
    public async Task AddProductToWishlist_ShouldReturnTrue_IfProductAddedSuccessfully()
    {
        // Arrange
        var userId = "123test";
        var productId = "product123";
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { "product1", "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);
        _wishlistRepoMock.Setup(x => x.UpdateOne(It.IsAny<WishlistEntity>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.AddProductToWishlist(userId, productId);

        // Assert
        Assert.True(result);
        _wishlistRepoMock.Verify(repo => repo.UpdateOne(It.Is<WishlistEntity>(w => w.ProductIds.Contains(productId))), Times.Once);
    }

    [Fact]
    public async Task AddProductToWishlist_ShouldReturnFalse_IfProductAlreadyInWishlist()
    {
        // Arrange
        var userId = "123test";
        var productId = "product1"; 
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { "product1", "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.AddProductToWishlist(userId, productId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveProductFromWishlist_ShouldReturnTrue_IfProductRemovedSuccessfully()
    {
        // Arrange
        var userId = "123test";
        var productId = "product1";
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { productId, "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);
        _wishlistRepoMock.Setup(x => x.UpdateOne(It.IsAny<WishlistEntity>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.RemoveProductFromWishlist(userId, productId);

        // Assert
        Assert.True(result);
        _wishlistRepoMock.Verify(repo => repo.UpdateOne(It.Is<WishlistEntity>(w => !w.ProductIds.Contains(productId))), Times.Once);
    }

    [Fact]
    public async Task RemoveProductFromWishlist_ShouldReturnFalse_IfProductNotInWishlist()
    {
        // Arrange
        var userId = "123test";
        var productId = "nonexistentproduct";
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { "product1", "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.RemoveProductFromWishlist(userId, productId);

        // Assert
        Assert.False(result); 
    }

    [Fact]
    public async Task GetWishlist_ShouldReturnWishlist_IfUserExists()
    {
        // Arrange
        var userId = "123test";
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { "product1", "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.GetWishlist(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.ProductIds.Count);
    }

    [Fact]
    public async Task GetWishlist_ShouldReturnNull_IfUserDoesNotExist()
    {
        // Arrange
        var userId = "non-existent-user-id";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _wishlistService.GetWishlist(userId);

        // Assert
        Assert.Null(result); 
    }

    [Fact]
    public async Task ClearWishlist_ShouldReturnTrue_IfWishlistClearedSuccessfully()
    {
        // Arrange
        var userId = "123test";
        var user = new UserEntity { Id = userId };
        var wishlist = new WishlistEntity
        {
            UserId = userId,
            ProductIds = new List<string> { "product1", "product2" }
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _wishlistRepoMock.Setup(x => x.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<WishlistEntity, bool>>>())).ReturnsAsync(wishlist);
        _wishlistRepoMock.Setup(x => x.UpdateOne(It.IsAny<WishlistEntity>())).ReturnsAsync(wishlist);

        // Act
        var result = await _wishlistService.ClearWishlist(userId);

        // Assert
        Assert.True(result);
        Assert.Empty(wishlist.ProductIds);
        _wishlistRepoMock.Verify(repo => repo.UpdateOne(It.Is<WishlistEntity>(w => w.ProductIds.Count == 0)), Times.Once);
    }
}
