
using Benday.CosmosDb.Utilities;
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

    /// <summary>
    /// Configures ASP.NET Core Identity
    /// </summary>
    public void ConfigureIdentity()
    {
        var cosmosConfig = _Builder.Configuration.GetCosmosConfig();

        _Builder.Services.AddCosmosRepository(setup =>
        {
            setup.ConnectionString = cosmosConfig.ConnectionString;
            setup.DatabaseId = cosmosConfig.DatabaseName;
        });

        _Builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 3;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.User.RequireUniqueEmail = true;
        })
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

