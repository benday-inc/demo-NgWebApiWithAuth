# Benday.Identity.CosmosDb

ASP.NET Core Identity implementation using Azure Cosmos DB as the backing store.

## Features

- Full ASP.NET Core Identity support with Cosmos DB storage
- User management (create, update, delete, find)
- Role-based access control
- Claims-based authorization
- Account lockout protection
- Two-factor authentication (2FA) support
- External login providers (Google, Facebook, Microsoft, etc.)
- Phone number verification
- Security stamp management for token invalidation
- LINQ query support

## Installation

```bash
dotnet add package Benday.Identity.CosmosDb --prerelease
```

## Dependencies

- [Benday.CosmosDb](https://www.nuget.org/packages/Benday.CosmosDb) - Cosmos DB repository pattern library
- Microsoft.Extensions.Identity.Core

## Implemented Interfaces

### User Store
- `IUserStore<IdentityUser>`
- `IUserPasswordStore<IdentityUser>`
- `IUserEmailStore<IdentityUser>`
- `IUserRoleStore<IdentityUser>`
- `IUserSecurityStampStore<IdentityUser>`
- `IUserLockoutStore<IdentityUser>`
- `IUserClaimStore<IdentityUser>`
- `IUserTwoFactorStore<IdentityUser>`
- `IUserPhoneNumberStore<IdentityUser>`
- `IUserAuthenticatorKeyStore<IdentityUser>`
- `IUserTwoFactorRecoveryCodeStore<IdentityUser>`
- `IUserLoginStore<IdentityUser>`
- `IQueryableUserStore<IdentityUser>`

### Role Store
- `IRoleStore<IdentityRole>`
- `IRoleClaimStore<IdentityRole>`
- `IQueryableRoleStore<IdentityRole>`

## Usage

Register the identity stores in your `Program.cs` or startup configuration:

```csharp
// Register Cosmos DB repositories
services.AddCosmosRepository<IdentityUser, CosmosDbUserStore>(options =>
{
    options.DatabaseId = "YourDatabase";
    options.ContainerId = "Users";
});

services.AddCosmosRepository<IdentityRole, CosmosDbRoleStore>(options =>
{
    options.DatabaseId = "YourDatabase";
    options.ContainerId = "Roles";
});

// Register Identity
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders();
```

## License

MIT License - see LICENSE file for details.
