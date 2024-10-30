using AuthenticationProvider.Entities;
using AuthenticationProvider.Models;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationProvider.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<UserEntity> userManager, TokenService tokenService) : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //check if the user already exists
        var userExists = await _userManager.FindByEmailAsync(signUpModel.Email);
        if (userExists != null)
        {
            return BadRequest("user already exists");
        }
        var user = new UserEntity
        {
            Email = signUpModel.Email,
            UserName = signUpModel.Username,
            IsAdmin = signUpModel.IsAdmin
        };
        var result = await _userManager.CreateAsync(user, signUpModel.Password);
        if (result.Succeeded)
        {
            return Ok("User created successfully");
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInModel signInModel)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(signInModel.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, signInModel.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = _tokenService.GenerateJwtToken(user);

        return Ok(new { Token = token });
    }
}
