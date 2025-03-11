using Benday.CosmosDb.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Benday.Identity.CosmosDb;

public interface ICosmosDbUserStore : IOwnedItemRepository<IdentityUser>, 
    IUserStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IUserRoleStore<IdentityUser>
{
}


