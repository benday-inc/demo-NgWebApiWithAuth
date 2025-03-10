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
        [FromRoute]string ownerId)
    {
        if (ownerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank");
        }

        var results = await _Service.GetAllAsync(ownerId);

        return Ok(results);
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
        else
        {
            var result = await _Service.SaveAsync(value);

            return Ok(result);
        }
    }
}
