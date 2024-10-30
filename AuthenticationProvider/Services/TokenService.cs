﻿using AuthenticationProvider.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationProvider.Services;

public class TokenService(IConfiguration configuration)
{
    // get the secret key, issuer and audience from the appsettings.json file
    private readonly string _secretKey = configuration["Jwt:SecretKey"]!;
    private readonly string _issuer = configuration["Jwt:Issuer"]!;
    private readonly string _audience = configuration["Jwt:Audience"]!;

    public string GenerateJwtToken(UserEntity user)
    {
        // create a new security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        // create new credentials
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // create new claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.UserName!),
            // add the isAdmin claim, important for authorization of admin users
            new Claim("isAdmin", user.IsAdmin.ToString())
        };
        // create a new token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );
        // return the token as a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
