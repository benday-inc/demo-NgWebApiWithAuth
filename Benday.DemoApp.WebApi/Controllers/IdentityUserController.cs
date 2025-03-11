using System.Security.Claims;
using Benday.CosmosDb.ServiceLayers;
using Benday.CosmosDb.Utilities;
using Benday.DemoApp.Api;
using Benday.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityRole = Benday.Identity.CosmosDb.IdentityRole;
using IdentityUser = Benday.Identity.CosmosDb.IdentityUser;

namespace Benday.DemoApp.WebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class IdentityUserController : ControllerBase
{
    private readonly ICosmosDbUserStore _UserStore;
    private readonly IRoleClaimStore<IdentityRole> _RoleClaimStore;

    public IdentityUserController(
        ICosmosDbUserStore service,
        IRoleClaimStore<IdentityRole> roleClaimStore)
    {
        _UserStore = service;
        _RoleClaimStore = roleClaimStore;
    }

    [HttpGet("getallbyownerid/{ownerId}")]
    public async Task<ActionResult<List<IdentityUser>>> GetAllByOwnerId(
        [FromRoute] string ownerId)
    {
        if (ownerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank");
        }

        var results = await _UserStore.GetAllAsync(ownerId);

        return Ok(results);
    }

    [HttpGet("GetByIdAndOwnerId/{ownerId}/{id}")]
    public async Task<ActionResult<IdentityUser>> GetByIdAndOwnerId(
        [FromRoute] string ownerId, [FromRoute] string id)
    {
        if (ownerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank");
        }

        if (id.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("Id cannot be null or blank");
        }

        var results = await _UserStore.GetByIdAsync(ownerId, id);

        if (results == null)
        {
            return NotFound();
        }
        else
        {
            var roles = await _UserStore.GetRolesAsync(results, new CancellationToken());

            return Ok(results);
        }
    }

    [HttpPost("{ownerId}")]
    public async Task<ActionResult<IdentityUser>> Post(
        [FromRoute] string ownerId,
        [FromBody] IdentityUser value)
    {
        if (value == null)
        {
            return BadRequest("Value cannot be null.");
        }
        else if (value.OwnerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank.");
        }
        else if (value.OwnerId != ownerId)
        {
            return BadRequest("OwnerId in URL does not match OwnerId in value.");
        }
        else
        {
            // reload a fresh copy from cosmos
            var toModel = await _UserStore.GetByIdAsync(value.OwnerId, value.Id);

            if (toModel == null)
            {
                return BadRequest("Could not find user in database.");
            }

            await VerifyRolesExist(toModel);

            await UpdateClaims(value, toModel);

            // await _UserStore.SaveAsync(toModel);
            await _UserStore.UpdateAsync(toModel, new CancellationToken());

            return Ok(toModel);
        }
    }

    private async Task VerifyRolesExist(IdentityUser toModel)
    {
        foreach (var role in toModel.Claims.Where(c => c.ClaimType == ClaimTypes.Role))
        {
            var existingRole = await _RoleClaimStore.FindByNameAsync(role.ClaimValue, CancellationToken.None);

            if (existingRole == null)
            {
                existingRole = new IdentityRole();
                existingRole.Name = role.ClaimValue;
                existingRole.NormalizedName = role.ClaimValue.ToUpper();

                existingRole.AddClaim(new Claim(ClaimTypes.Role, role.ClaimValue));

                await _RoleClaimStore.UpdateAsync(existingRole, CancellationToken.None);
            }
        }
    }

    private async Task UpdateClaims(IdentityUser fromValue, IdentityUser toValue)
    {
        // claims from the request
        var fromClaims = fromValue.Claims;

        // claims from the database
        var toClaims = toValue.Claims;

        foreach (var fromClaim in fromClaims)
        {
            if (ClaimExists(toClaims, fromClaim) == false)
            {
                await _UserStore.AddToRoleAsync(toValue, fromClaim.ClaimValue, CancellationToken.None);
            }
        }

        var removeThese = new List<IdentityUserClaim>();

        foreach (var existingClaim in toClaims)
        {
            if (ClaimExists(fromClaims, existingClaim) == false)
            {
                removeThese.Add(existingClaim);
            }
        }

        if (removeThese.Count > 0)
        {
            foreach (var removeThis in removeThese)
            {
                toClaims.Remove(removeThis);
                await _UserStore.RemoveFromRoleAsync(toValue, removeThis.ClaimValue, CancellationToken.None);
            }            
        }
    }

    private bool ClaimExists(List<IdentityUserClaim> claims, IdentityUserClaim search)
    {
        var match = claims.Where(c =>
            c.ClaimType == search.ClaimType &&
            c.ClaimValue == search.ClaimValue).FirstOrDefault();

        if (match == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
