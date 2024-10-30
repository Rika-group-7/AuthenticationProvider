using AuthenticationProvider.DTOs;
using AuthenticationProvider.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationProvider.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager;

    public UserController(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    //get all users med BARA: ID, FirstName,LastName och Email, eftersom en admin behöver inte se all information innan den trycker på en specifik user
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        var userList = users.Select(user => new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        }).ToList();

        return Ok(userList);
    }

    //hämtar en user med ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsAdmin = user.IsAdmin,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProfileDescription = user.ProfileDescription,
            Gender = user.Gender,
            Age = user.Age
        };

        return Ok(userDto);
    }

    //hämtar en user med Epost
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsAdmin = user.IsAdmin,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProfileDescription = user.ProfileDescription,
            Gender = user.Gender,
            Age = user.Age
        };

        return Ok(userDto);
    }

    //updatera användaren
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Email = userDto.Email;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.IsAdmin = userDto.IsAdmin;  //det här bör kunna uppdatera bara Superadmin!
        user.ProfilePictureUrl = userDto.ProfilePictureUrl;
        user.ProfileDescription = userDto.ProfileDescription;
        user.Gender = userDto.Gender;
        user.Age = userDto.Age;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok("User updated successfully.");
        }

        return BadRequest(result.Errors);
    }

    //delete user
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok("User deleted successfully.");
        }

        return BadRequest(result.Errors);
    }
}
