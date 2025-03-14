﻿using Benday.CosmosDb.ServiceLayers;
using Benday.CosmosDb.Utilities;
using Benday.DemoApp.Api;
using Microsoft.AspNetCore.Mvc;

namespace Benday.DemoApp.WebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class NoteController : ControllerBase
{
    private readonly IOwnedItemService<Note> _NoteService;

    public NoteController(IOwnedItemService<Note> noteService)
    {
        _NoteService = noteService;
    }

    [HttpGet("{ownerId}")]
    public async Task<ActionResult<List<Note>>> GetAll(
        [FromRoute]string ownerId)
    {
        if (ownerId.IsNullOrWhiteSpace() == true)
        {
            return BadRequest("OwnerId cannot be blank");
        }

        var results = await _NoteService.GetAllAsync(ownerId);

        return Ok(results);
    }

    [HttpPost("{ownerId}")]
    public async Task<ActionResult<Note>> Post(
        [FromRoute] string ownerId,
        [FromBody] Note value)
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
            var result = await _NoteService.SaveAsync(value);

            return Ok(result);
        }
    }
}
