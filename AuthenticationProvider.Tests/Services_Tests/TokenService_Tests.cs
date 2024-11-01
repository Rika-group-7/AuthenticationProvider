using AuthenticationProvider.Entities;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationProvider.Tests.Services_Tests;

public class TokenService_Tests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly TokenService _tokenService;

    public TokenService_Tests()
    {
        // Create mocks
        _configurationMock = new Mock<IConfiguration>();
        _userManagerMock = CreateUserManagerMock();

        // Setup configuration values
        _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("a1b2c3d4e5f6-3f9s-s17w-4824-c085ee20");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("https://testdomain.com");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("https://testdomain.com");

        _tokenService = new TokenService(_configurationMock.Object, _userManagerMock.Object);
    }
    // Create a mock of UserManager<UserEntity>, very convoluted, had to ask AI for help
    private Mock<UserManager<UserEntity>> CreateUserManagerMock()
    {
        return new Mock<UserManager<UserEntity>>(
            new Mock<IUserStore<UserEntity>>().Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<UserEntity>>(),
            new IUserValidator<UserEntity>[0],
            new IPasswordValidator<UserEntity>[0],
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<UserEntity>>>()
        );
    }



    [Fact]
    public async Task GenerateJwtToken_ShouldReturnToken_IfUserIsValid()
    {
        // Arrange
        var testUser = new UserEntity
        {
            Id = "1",
            Email = "test@testsson.com",
            UserName = "Testsson",
            IsAdmin = true
        };

        _userManagerMock.Setup(x => x.GetRolesAsync(testUser)).ReturnsAsync(new List<string> { "Admin" });

        // Act
        var token = await _tokenService.GenerateJwtToken(testUser);

        // Assert
        Assert.NotNull(token);
        Assert.IsType<string>(token);
    }
}


