using Benday.CosmosDb.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Benday.Identity.CosmosDb;

public class CosmosDbUserStore : CosmosOwnedItemRepository<IdentityUser>, 
    IUserStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>
{
    public CosmosDbUserStore(
       IOptions<CosmosRepositoryOptions<IdentityUser>> options,
       CosmosClient client, ILogger<CosmosDbUserStore> logger) :
       base(options, client, logger)
    {
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

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.PasswordHash);
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string>(user.Id);
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.UserName);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var hasPassword = string.IsNullOrEmpty(user.PasswordHash) == false;

        return Task.FromResult<bool>(hasPassword);
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


