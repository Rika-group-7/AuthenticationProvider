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
}
