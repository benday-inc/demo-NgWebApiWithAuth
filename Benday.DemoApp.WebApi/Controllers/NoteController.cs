﻿using Benday.CosmosDb.ServiceLayers;
using Benday.DemoApp.Api;
using Cosmos.Abstracts.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Benday.DemoApp.WebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class NoteController : ControllerBase
{
    private readonly IOwnedItemServiceBase<Note> _NoteService;

    public NoteController(IOwnedItemServiceBase<Note> noteService)
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
}
