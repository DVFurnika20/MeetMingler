using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
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
    
    [HttpGet]
    public async Task<ActionResult<BaseCollectionVM<EventVM>>> GetAllPaginatedAndFiltered([FromQuery] PaginationOptions paginationOptions, [FromQuery] EventFilter filter)
    {
        var result = await eventService.GetAllPaginatedAndFilteredAsync(paginationOptions, filter);
        return Ok(result);
    }
    
    [HttpGet("{metadataKey}")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctMetadataValues([FromRoute] string metadataKey)
    {
        var result = await eventService.GetDistinctMetadataValuesAsync(metadataKey).ToListAsync();
        return Ok(result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> SetCancelled([FromRoute] Guid id, [FromBody] bool cancelled)
    {
        var result = await eventService.SetCancelledAsync(id, cancelled);
        if (!result)
        {
            return BadRequest("Cancellation status update failed.");
        }
        return Ok();
    }
    
    [HttpPost("{eventId:guid}")]
    public async Task<ActionResult<EventVM>> AddMetadata([FromRoute] Guid eventId, [FromBody] EventMetadataIM metadata)
    {
        var result = await eventService.AddMetadataAsync(eventId, metadata);
        if (result == null)
        {
            return BadRequest("Metadata addition failed.");
        }
        return Ok(result);
    }
    
    [HttpDelete("{eventId:guid}/{key}")]
    public async Task<ActionResult> DeleteMetadata([FromRoute] Guid eventId, [FromRoute] string key)
    {
        var result = await eventService.DeleteMetadataAsync(eventId, key);
        if (result == null)
        {
            return BadRequest("Metadata deletion failed.");
        }
        return Ok();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var result = await eventService.DeleteAsync(id);
        if (!result)
        {
            return BadRequest("Event deletion failed.");
        }
        return Ok();
    }
}