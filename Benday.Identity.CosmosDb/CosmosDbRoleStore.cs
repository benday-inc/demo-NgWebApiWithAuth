using Benday.CosmosDb.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Benday.Identity.CosmosDb
{
    public class CosmosDbRoleStore :
        CosmosOwnedItemRepository<IdentityRole>, IRoleStore<IdentityRole>
    {
        public CosmosDbRoleStore(
           IOptions<CosmosRepositoryOptions<IdentityRole>> options,
           CosmosClient client, ILogger<CosmosDbRoleStore> logger) :
           base(options, client, logger)
        {
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            await SaveAsync(role);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            await DeleteAsync(role);

            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await GetByIdAsync(IdentityConstants.SystemOwnerId, roleId);
        }

        public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var query = await GetQueryable();

            var queryable = query.Queryable.Where(x => x.NormalizedName == normalizedRoleName);

            var results = await GetResults(queryable, GetQueryDescription(), query.PartitionKey);

            return results.FirstOrDefault();
        }

        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Id);
        }

        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException($"{nameof(roleName)} is null or empty.", nameof(roleName));
            }

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            await SaveAsync(role);

            return IdentityResult.Success;
        }
    }
}


