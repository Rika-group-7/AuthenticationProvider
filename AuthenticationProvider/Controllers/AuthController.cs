using AuthenticationProvider.Entities;
using AuthenticationProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationProvider.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<UserEntity> userManager) : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager = userManager;

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
}
