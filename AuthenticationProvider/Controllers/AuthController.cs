using AuthenticationProvider.Entities;
using AuthenticationProvider.Models;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Authorization;
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
        // check if the model is valid
        if (!ModelState.IsValid)
        {
            // return bad request if the model is not valid
            return BadRequest(ModelState);
        }

        //check if the user already exists
        var userExists = await _userManager.FindByEmailAsync(signUpModel.Email);
        if (userExists != null)
        {
            // return bad request if the user already exists
            return BadRequest("user already exists");
        }
        // create a new user if the user does not exist
        var user = new UserEntity
        {
            Email = signUpModel.Email,
            UserName = signUpModel.Username,
            IsAdmin = signUpModel.IsAdmin
        };
        // create the user
        var result = await _userManager.CreateAsync(user, signUpModel.Password);
        if (result.Succeeded)
        {
            // return ok if the user is created successfully
            return Ok("User created successfully");
        }
        // return bad request if the user is not created successfully
        return BadRequest(result.Errors);
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInModel signInModel)
    {
        // check if the model is valid
        if (!ModelState.IsValid)
        {
            // return bad request if the model is not valid
            return BadRequest(ModelState);
        }

        // check if the user exists
        var user = await _userManager.FindByEmailAsync(signInModel.Email);
        // return unauthorized if the user does not exist or the password is incorrect
        if (user == null || !await _userManager.CheckPasswordAsync(user, signInModel.Password))
        {
            return Unauthorized("Invalid credentials");
        }
        // generate a token if the user exists and the password is correct
        var token = _tokenService.GenerateJwtToken(user);
        // return the token
        return Ok(new { Token = token });
    }

    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        // return ok if the user is authorized
        return Ok("SUCCESS!?");
    }
}
