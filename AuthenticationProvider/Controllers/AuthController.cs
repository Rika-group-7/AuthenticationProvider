using AuthenticationProvider.Entities;
using AuthenticationProvider.Interfaces;
using AuthenticationProvider.Models;
using AuthenticationProvider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.WebRequestMethods;

namespace AuthenticationProvider.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<UserEntity> userManager, ITokenService tokenService, IVerificationService verificationService) : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IVerificationService _verificationService = verificationService;

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

        var result = await _userManager.CreateAsync(user, signUpModel.Password);
        if (result.Succeeded)
        {
            // Generate EmailToken and save it with the user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.EmailConfirmationToken = token;
            await _userManager.UpdateAsync(user);

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                Console.WriteLine("Sending VerificationRequest");
                try
                {
                    // Send email and token to VerificationProvider
                    await _verificationService.SendVerificationRequest(user.Email, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed while Sending VerificationRequest: {ex.Message}");
                    return BadRequest($"Failed while Sending VerificationRequest: {ex.Message}");
                }
            }
            else Console.WriteLine("Fail to VerificationRequest");


            // assign role based on the IsAdmin property, default is User
            if (signUpModel.IsAdmin == true)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            // return ok if the user is created successfully
            return Ok(new { message = "User created successfully", userId = user.Id });
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
        var token = await _tokenService.GenerateJwtToken(user);
        // return the token
        return Ok(new { Token = token });
    }

    [HttpGet("test")]
    [Authorize(Roles = "Admin")]
    public IActionResult Test()
    {
        // return ok if the user is authorized
        return Ok("SUCCESS!?");
    }


    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountModel model)
    {
        // Call VerificationProvider to validate the code
        var isValid = await _verificationService.ValidateVerificationCodeAsync(model.Email, model.Code);

        if (isValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Ok("Email confirmed successfully.");
            }
            return BadRequest("User not found.");
        }
        return BadRequest("Invalid verification code.");
    }
}
