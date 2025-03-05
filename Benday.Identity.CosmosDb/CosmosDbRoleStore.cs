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

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}


