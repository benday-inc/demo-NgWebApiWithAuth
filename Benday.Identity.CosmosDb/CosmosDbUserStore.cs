using Benday.CosmosDb.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Benday.Identity.CosmosDb;

public class CosmosDbUserStore : CosmosOwnedItemRepository<IdentityUser>, 
    IUserStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IUserRoleStore<IdentityUser>
{
    public CosmosDbUserStore(
       IOptions<CosmosRepositoryOptions<IdentityUser>> options,
       CosmosClient client, ILogger<CosmosDbUserStore> logger) :
       base(options, client, logger)
    {
    }

    public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        var match = user.Claims.Find(x => x.ClaimType == ClaimTypes.Role && x.ClaimValue == roleName);

        if (match == null)
        {
            user.Claims.Add(new IdentityUserClaim()
            {
                ClaimType = ClaimTypes.Role,
                ClaimValue = roleName
            });

            await SaveAsync(user);
        }
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        await SaveAsync(user);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        await DeleteAsync(user);

        return IdentityResult.Success;
    }

    public void Dispose()
    {
        
    }

    public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var query = await GetQueryable();

        var queryable = query.Queryable.Where(x => x.NormalizedEmail == normalizedEmail);

        var results = await GetResults(queryable, GetQueryDescription(), query.PartitionKey);

        return results.FirstOrDefault();
    }

    public async Task<IdentityUser?> FindByIdAsync(
        string userId, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(IdentityConstants.SystemOwnerId, userId);
    }

    public async Task<IdentityUser?> FindByNameAsync(
        string normalizedUserName, CancellationToken cancellationToken)
    {
        var query = await GetQueryable();

        var queryable = query.Queryable.Where(x => x.NormalizedUserName == normalizedUserName);

        var results = await GetResults(queryable, GetQueryDescription(), query.PartitionKey);

        return results.FirstOrDefault();
    }

    public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<bool>(user.EmailConfirmed);
    }

    public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.NormalizedEmail);
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.PasswordHash);
    }

    public Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var roles = user.Claims.FindAll(x => x.ClaimType == ClaimTypes.Role);

        return Task.FromResult<IList<string>>(roles.Select(x => x.ClaimValue).ToList());
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string>(user.Id);
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.UserName);
    }

    public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var query = await GetQueryable();

        var queryable = query.Queryable.Where(x =>  
            x.Claims.Any(y => y.ClaimType == ClaimTypes.Role && y.ClaimValue == roleName));

        var results = await GetResults(queryable, GetQueryDescription(), query.PartitionKey);

        return results;
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var hasPassword = string.IsNullOrEmpty(user.PasswordHash) == false;

        return Task.FromResult<bool>(hasPassword);
    }

    public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        if (user.Claims.Any(x => x.ClaimType == ClaimTypes.Role && x.ClaimValue == roleName))
        {
            return Task.FromResult<bool>(true);
        }
        else
        {
            return Task.FromResult<bool>(false);
        }
    }

    public Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        var match = user.Claims.Find(x => x.ClaimType == ClaimTypes.Role && x.ClaimValue == roleName);

        if (match != null)
        {
            user.Claims.Remove(match);
            return SaveAsync(user);
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException($"{nameof(email)} is null or empty.", nameof(email));
        }

        user.Email = email;

        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;

        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            throw new ArgumentException($"{nameof(normalizedEmail)} is null or empty.", nameof(normalizedEmail));
        }

        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            throw new ArgumentException($"{nameof(passwordHash)} is null or empty.", nameof(passwordHash));
        }

        user.PasswordHash = passwordHash;

        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentException($"{nameof(userName)} is null or empty.", nameof(userName));
        }

        user.UserName = userName;

        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        await SaveAsync(user);

        return IdentityResult.Success;
    }
}


