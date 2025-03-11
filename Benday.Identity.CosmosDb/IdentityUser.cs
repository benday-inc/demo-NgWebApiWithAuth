using Benday.CosmosDb.DomainModels;

namespace Benday.Identity.CosmosDb
{
    public class IdentityUser : SystemOwnedItem
    {
        public string UserName { get; set; } = string.Empty;
        public string NormalizedUserName 
        { 
            get
            {
                return UserName.ToUpperInvariant();
            } 
            set
            {
                // do nothing
            }
        } 
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail 
        { 
            get
            {
                return Email.ToUpperInvariant();
            }
            set
            {

            }
        }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public List<IdentityUserClaim> Claims { get; set; } = new List<IdentityUserClaim>();        
    }
}


