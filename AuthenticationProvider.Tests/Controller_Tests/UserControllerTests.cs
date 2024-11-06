using AuthenticationProvider.Controllers;
using AuthenticationProvider.DTOs;
using AuthenticationProvider.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;


namespace AuthenticationProvider.Tests.Controller_Tests;
public class UserControllerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _userController = new UserController(_userManagerMock.Object);
    }

    //GetSelf
    [Fact]
    public async Task GetSelf_ShouldReturnUserDto_IfUserIsFound()
    {
        // Arrange
        var userId = "user-id";
        var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(userClaims)) }
        };

        var user = new UserEntity
        {
            Id = userId,
            Email = "imaginaryusert@email.com",
            FirstName = "Tester",
            LastName = "Testerson"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userController.GetSelf();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(actionResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(user.Email, returnedUser.Email);
    }

    //GetAllUsers
    [Fact]
    public async Task GetAllUsers_ShouldReturnListOfUsers()
    {
        // Arrange
        var users = new List<UserEntity>
        {
            new UserEntity { Id = "1", FirstName = "Marcus", LastName = "Marcusson", Email = "marcus@email.com" },
            new UserEntity { Id = "2", FirstName = "Billy", LastName = "Billysson", Email = "billy@email.com" }
        }.AsQueryable();

        _userManagerMock.Setup(x => x.Users).Returns(users);

        // Act
        var result = await _userController.GetAllUsers();

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsType<List<object>>(actionResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }

    //GetUserById
    [Fact]
    public async Task GetUserById_ShouldReturnUserDto_IfUserExists()
    {
        // Arrange
        var userId = "1";
        var user = new UserEntity { Id = userId, Email = "user@email.com" };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userController.GetUserById(userId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(actionResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    //GetUserByEmail
    [Fact]
    public async Task GetUserByEmail_ShouldReturnNotFound_IfUserNotFound()
    {
        // Arrange
        var email = "imaginaryusert@email.com";
        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((UserEntity)null);

        // Act
        var result = await _userController.GetUserByEmail(email);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    //UpdateUser
    [Fact]
    public async Task UpdateUser_ShouldReturnOk_IfUpdateSucceeds()
    {
        // Arrange
        var userId = "1";
        var userDto = new UserDto { Id = userId, Email = "updated@email.com", FirstName = "Updatedemail" };
        var user = new UserEntity { Id = userId, Email = "old@email.com" };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userController.UpdateUser(userId, userDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    //DeleteUser
    [Fact]
    public async Task DeleteUser_ShouldReturnBadRequest_IfDeletionFails()
    {
        // Arrange
        var userId = "1";
        var user = new UserEntity { Id = userId };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

        // Act
        var result = await _userController.DeleteUser(userId);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Delete failed", ((IdentityError[])actionResult.Value).First().Description);
    }
}
