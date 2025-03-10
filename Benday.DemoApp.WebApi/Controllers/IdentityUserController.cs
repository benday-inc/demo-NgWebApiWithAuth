using System.Security.Claims;
using Benday.CosmosDb.ServiceLayers;
using Benday.CosmosDb.Utilities;
using Benday.DemoApp.Api;
using Benday.Identity.CosmosDb;
using Microsoft.AspNetCore.Mvc;

namespace Benday.DemoApp.WebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class IdentityUserController : ControllerBase
{
    private readonly IOwnedItemService<IdentityUser> _Service;

    public IdentityUserController(IOwnedItemService<IdentityUser> service)
    {
        _Service = service;
    }

    [HttpGet("getallbyownerid/{ownerId}")]
    public async Task<ActionResult<List<IdentityUser>>> GetAllByOwnerId(
        [FromRoute] string ownerId)
    {
        if (ownerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank");
        }

        var results = await _Service.GetAllAsync(ownerId);

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

        var results = await _Service.GetByIdAsync(ownerId, id);

        if (results == null)
        {
            return NotFound();
        }
        else
        {
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
            var toModel = await _Service.GetByIdAsync(value.OwnerId, value.Id);

            if (toModel == null)
            {
                return BadRequest("Could not find user in database.");
            }

            UpdateClaims(value, toModel);

            await _Service.SaveAsync(toModel);            
            
            return Ok(toModel);
        }
    }

    private void UpdateClaims(IdentityUser fromValue, IdentityUser toValue)
    {
        // claims from the request
        var fromClaims = fromValue.Claims;

        // claims from the database
        var toClaims = toValue.Claims;

        foreach (var fromClaim in fromClaims)
        {
            if (ClaimExists(toClaims, fromClaim) == false)
            {
                toClaims.Add(fromClaim);
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
            removeThese.ForEach(c => toClaims.Remove(c));
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
