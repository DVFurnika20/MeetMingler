using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Services.Contracts.Generic;

namespace MeetMingler.BLL.Services.Contracts;

public interface IEventService : IGenericCreateService<EventVM, EventIM>,
    IGenericFilteredCollectionService<EventVM, EventFilter>
{
    /// <summary>
    /// Retrieve an event by its unique identifier.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <returns>The view model of an event</returns>
    Task<EventVM> GetEventByIdAsync(Guid id);
    
    /// <summary>
    /// Retrieve all events created by a user
    /// </summary>
    /// <param name="creatorId">The user's unique identifier</param>
    /// <param name="includeMetadataKeys">The metadata key/value pairs to include in the resultset</param>
    /// <returns>A collection of events</returns>
    Task<IQueryable<EventVM>> GetEventsByCreatorIdAsync(Guid creatorId, List<string> includeMetadataKeys);
    
    /// <summary>
    /// Retrieve all distinct metadata values for a given metadata key
    /// </summary>
    /// <param name="metadataKey">Metadata key</param>
    /// <returns>A collection of values related to the metadata key</returns>
    Task<IQueryable<string>> GetDistinctMetadataValuesAsync(string metadataKey);
    
    // TODO: add methods for updating single event
    // TODO: add methods for cancelling/reinstating event
    // TODO: add methods for updating adding/updating/deleting event metadata
    // TODO: add methods for deleting single event
}