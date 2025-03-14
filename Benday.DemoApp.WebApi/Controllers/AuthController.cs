﻿using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Benday.DemoApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using IdentityRole = Benday.Identity.CosmosDb.IdentityRole;
using IdentityUser = Benday.Identity.CosmosDb.IdentityUser;


namespace Benday.DemoApp.WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly IConfiguration _Configuration;

    public AuthController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _UserManager = userManager;
        _SignInManager = signInManager;
        _Configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };

            var validationContext = new ValidationContext(user);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(user, validationContext, results, true);


            var result = await _UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully!" });
            }
            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _SignInManager.SignOutAsync();
        return Ok(new { message = "User logged out successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _UserManager.FindByEmailAsync(model.Email);
        if (user == null || !await _UserManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private JwtConfig GetJwtConfiguration()
    {
        var config = new JwtConfig();

        config.Secret = _Configuration["Jwt:Secret"].ThrowIfEmptyOrNull("Jwt:Secret");
        config.Issuer = _Configuration["Jwt:Issuer"].ThrowIfEmptyOrNull("Jwt:Issuer");
        config.Audience = _Configuration["Jwt:Audience"].ThrowIfEmptyOrNull("Jwt:Audience");

        return config;
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        user.Claims.ForEach(claim =>
        {
            claims.Add(new Claim(claim.ClaimType, claim.ClaimValue));
        });

        var config = GetJwtConfiguration();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config.Issuer,
            audience: config.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
