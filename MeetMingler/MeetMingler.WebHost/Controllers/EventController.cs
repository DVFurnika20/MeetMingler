using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Models.User;
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
    public async Task<ActionResult<EventVM>> Create([FromBody] EventDictionaryIM eventDictionaryIm, CancellationToken ct)
    {
        var eventIm = new EventIM
        {
            Title = eventDictionaryIm.Title,
            Description = eventDictionaryIm.Description,
            StartTime = eventDictionaryIm.StartTime,
            EndTime = eventDictionaryIm.EndTime,
            Metadata = eventDictionaryIm.Metadata.Select(kvp => new EventMetadataIM
            {
                Key = kvp.Key,
                Value = kvp.Value
            }).ToList()
        };
        
        var result = await eventService.CreateAsync(eventIm, ct);
        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventExtendedVM>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await eventService.GetByIdAsync(id, ct);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{creatorId:guid}")]
    public async Task<ActionResult<IEnumerable<EventVM>>> GetByCreator([FromRoute] Guid creatorId,
        [FromQuery] List<string> includeMetadataKeys, [FromQuery] PaginationOptions paginationOptions,
        CancellationToken ct)
    {
        var result = await eventService.GetCollectionByCreatorAsync(creatorId, includeMetadataKeys, paginationOptions, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BaseCollectionVM<EventVM>>> GetAllPaginatedAndFiltered(
        [FromQuery] PaginationOptions paginationOptions, [FromBody] EventFilter filter, CancellationToken ct)
    {
        var result = await eventService.GetAllPaginatedAndFilteredAsync(paginationOptions, filter, ct);
        return Ok(result);
    }

    [HttpGet("{metadataKey}")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctMetadataValues([FromRoute] string metadataKey, CancellationToken ct)
    {
        var result = await eventService.GetDistinctMetadataValuesAsync(metadataKey).ToListAsync(ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> SetCancelled([FromRoute] Guid id, [FromBody] bool cancelled, CancellationToken ct)
    {
        var result = await eventService.SetCancelledAsync(id, cancelled, ct);
        if (!result)
        {
            return Forbid();
        }

        return Ok();
    }

    [HttpPost("{eventId:guid}")]
    public async Task<ActionResult<EventVM>> AddMetadata([FromRoute] Guid eventId, [FromBody] EventMetadataIM metadata, CancellationToken ct)
    {
        var result = await eventService.AddMetadataAsync(eventId, metadata, ct);
        if (result == null)
        {
            return Forbid();
        }

        return Ok(result);
    }
    
    [HttpPost("{eventId:guid}")]
    public async Task<ActionResult> RegisterUserForEvent([FromRoute] Guid eventId, CancellationToken ct)
    {
        var result = await eventService.RegisterUserForEventAsync(eventId, ct);
        if (!result)
        {
            return Forbid();
        }

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DateTime>>> GetDates([FromQuery] DateTime startDateRange, [FromQuery] DateTime endDateRange, CancellationToken ct)
    {
        var result = await eventService.GetDatesAsync(startDateRange, endDateRange, ct);
        return Ok(result);
    }
    
    [HttpPut("{eventId:guid}")]
    public async Task<ActionResult<EventVM>> Update([FromRoute] Guid eventId, [FromBody] EventUM updateModel, CancellationToken ct)
    {
        var result = await eventService.UpdateAsync(eventId, updateModel, ct);
        if (result == null)
        {
            return Forbid();
        }

        return Ok(result);
    }

    [HttpDelete("{eventId:guid}/{key}")]
    public async Task<ActionResult> DeleteMetadata([FromRoute] Guid eventId, [FromRoute] string key, CancellationToken ct)
    {
        var result = await eventService.DeleteMetadataAsync(eventId, key, ct);
        if (result == null)
        {
            return Forbid();
        }

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await eventService.DeleteAsync(id, ct);
        if (!result)
        {
            return Forbid();
        }

        return Ok();
    }
    
    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<UserVM>> GetAttendees([FromRoute] Guid eventId,
        [FromQuery] PaginationOptions pagination, CancellationToken ct = default)
    {
        var result = await eventService.GetAttendees(eventId, pagination, ct);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<EventVM>> GetSelfAttendance([FromQuery] PaginationOptions pagination,
        CancellationToken ct = default)
    {
        var result = await eventService.GetCurrentUserAttendance(pagination, ct);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}