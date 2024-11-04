using AuthenticationProvider.Controllers;
using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using AuthenticationProvider.Models;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationProvider.Tests.Controller_Tests;

public class AuthController_Tests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthController _authController;

    public AuthController_Tests()
    {
        // create mock of userstore wich is needed for the usermanager
        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        // create mock of usermanager, it needs a lot of parameters
        _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        // create mock of configuration for the token service
        var configurationMock = new Mock<IConfiguration>();
        // create mock of token service, it needs configuration and usermanager
        _tokenServiceMock = new Mock<ITokenService>();
        // create auth controller with the mocks
        _authController = new AuthController(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task SignUp_ModelIsValid_ShouldReturnOk()
    {
        // Arrange
        var signUpModel = new SignUpModel
        {
            Email = "test@testing.com",
            Username = "xunittest",
            Password = "TestPassword123!",
            IsAdmin = false // default is false, not necessary to include in the model but for clarity
        };
        // mocks find by email to return null
        _userManagerMock.Setup(x => x.FindByEmailAsync(signUpModel.Email)).ReturnsAsync((UserEntity)null);
        // mocks create user to return success
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), signUpModel.Password)).ReturnsAsync(IdentityResult.Success);
        // mocks add to role to return success
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "User")).ReturnsAsync(IdentityResult.Success);



        // Act
        var result = await _authController.SignUp(signUpModel);



        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User created successfully", actionResult.Value);

    }

    [Fact]
    public async Task SignUp_ModelIsNotValid_ShouldReturnBadRequest()
    {
        // Arrange
        var signUpModel = new SignUpModel();
        _authController.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _authController.SignUp(signUpModel);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(actionResult.Value);
    }

    [Fact]
    public async Task SignUp_UserAlreadyExists_shouldReturnBadRequest()
    {
        // Arrange
        var signUpModel = new SignUpModel
        {
            Email = "test@testing.com",
            Username = "xunittest",
            Password = "TestPassword123!",
            IsAdmin = false
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(signUpModel.Email)).ReturnsAsync(new UserEntity());

        // Act
        var result = await _authController.SignUp(signUpModel);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("user already exists", actionResult.Value);
    }

    [Fact]
    public async Task SignIn_ShouldReturnOk_AndToken_IfUserExists()
    {
        //arrange
        var signInModel = new SignInModel
        {
            Email = "test@testing.com",
            Password = "TestPassword123!"
        };

        var user = new UserEntity
        {
            Email = signInModel.Email,
            UserName = "xunittest"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(signInModel.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, signInModel.Password)).ReturnsAsync(true);
        _tokenServiceMock.Setup(x => x.GenerateJwtToken(user)).ReturnsAsync("testtoken");

        //act
        var actionResult = Assert.IsType<OkObjectResult>(result);

        //assert
        var responseValue = actionResult.Value;
        var tokenProperty = responseValue.GetType().GetProperty("Token");
        Assert.NotNull(tokenProperty);

        var token = tokenProperty.GetValue(responseValue) as string;
        Assert.NotNull(token);
        Assert.Equal("testtoken", token);
    }
}
