using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MeetMingler.WebHost.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class EventController(IAuthService authService, ICurrentUser currentUser, IEventService eventService)
    : BaseController(authService, currentUser)
{
    [HttpPost]
    public Task<ActionResult<EventVM>> Create()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:guid}")]
    public Task<ActionResult<EventVM>> GetById([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{creatorId:guid}")]
    public Task<ActionResult<IEnumerable<EventVM>>> GetByCreator([FromRoute] Guid creatorId)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<IEnumerable<string>>> GetDistinctMetadataValues()
    {
        throw new NotImplementedException();
    }
}