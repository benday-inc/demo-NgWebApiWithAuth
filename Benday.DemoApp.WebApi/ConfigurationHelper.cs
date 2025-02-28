
using Cosmos.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdentityRole = Cosmos.Identity.IdentityRole;
using IdentityUser = Cosmos.Identity.IdentityUser;

namespace Benday.DemoApp.WebApi;

public class ConfigurationHelper
{
    private WebApplicationBuilder _Builder;

    public ConfigurationHelper(WebApplicationBuilder builder)
    {
        _Builder = builder;
    }

    public CosmosConfig GetCosmosConfig()
    {
        var config = new CosmosConfig();

        config.Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        config.Endpoint = "https://localhost:8081";
        config.DatabaseName = "BendayDemoApp";

        return config;
    }
    public void ConfigureIdentity()
    {
        _Builder.Services.AddCosmosRepository(setup =>
        {
            setup.ConnectionString = GetCosmosConfig().ConnectionString;
        });

        _Builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddCosmosStores()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        _Builder.Services.AddRazorPages();

        var jwtConfig = GetJwtConfiguration();

        var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);
        _Builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience
            };
        });

        _Builder.Services.AddAuthorization();
    }

    private JwtConfig GetJwtConfiguration()
    {
        var config = new JwtConfig();

        config.Secret = _Builder.Configuration["Jwt:Secret"].ThrowIfEmptyOrNull("Jwt:Secret");
        config.Issuer = _Builder.Configuration["Jwt:Issuer"].ThrowIfEmptyOrNull("Jwt:Issuer");
        config.Audience = _Builder.Configuration["Jwt:Audience"].ThrowIfEmptyOrNull("Jwt:Audience");

        return config;
    }

    public void Configure(WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapRazorPages();
    }
}

