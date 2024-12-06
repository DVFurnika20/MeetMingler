using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.WebHost.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class EventController(IAuthService authService, ICurrentUser currentUser, IEventService eventService)
    : BaseController(authService, currentUser)
{
    [HttpPost]
    public async Task<ActionResult<EventVM>> Create([FromBody] EventIM eventIm)
    {
        var result = await eventService.CreateAsync(eventIm);
        if (result == null)
        {
            return BadRequest("Event creation failed.");
        }
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventVM>> GetById([FromRoute] Guid id)
    {
        var result = await eventService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
    
    [HttpGet("{creatorId:guid}")]
    public async Task<ActionResult<IEnumerable<EventVM>>> GetByCreator([FromRoute] Guid creatorId, [FromQuery] List<string> includeMetadataKeys)
    {
        var result = await eventService.GetCollectionByCreatorAsync(creatorId, includeMetadataKeys);
        return Ok(result);
    }
    
    [HttpGet("{metadataKey}")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctMetadataValues([FromRoute] string metadataKey)
    {
        var result = await eventService.GetDistinctMetadataValuesAsync(metadataKey).ToListAsync();
        return Ok(result);
    }
}