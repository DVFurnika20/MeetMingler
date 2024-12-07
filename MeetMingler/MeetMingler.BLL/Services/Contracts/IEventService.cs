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
    /// <param name="cf">Cancellation token</param>
    /// <returns>The view model of an event</returns>
    Task<EventVM?> GetByIdAsync(Guid id, CancellationToken cf = default);
 
    /// <summary>
    /// Retrieve all events created by a user
    /// </summary>
    /// <param name="creatorId">The user's unique identifier</param>
    /// <param name="includeMetadataKeys">The metadata key/value pairs to include in the resultset</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of events</returns>
    Task<IEnumerable<EventVM>> GetCollectionByCreatorAsync(Guid creatorId, List<string> includeMetadataKeys, CancellationToken cf = default);

    /// <summary>
    /// Retrieve all distinct metadata values for a given metadata key
    /// </summary>
    /// <param name="metadataKey">Metadata key</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of values related to the metadata key</returns>
    IQueryable<string> GetDistinctMetadataValuesAsync(string metadataKey, CancellationToken cf = default);
 
    // TODO: add methods for updating single event

    // TODO: add methods for cancelling/reinstating event
    // TODO: add methods for adding/updating/deleting event metadata
    // TODO: add methods for deleting single event

    /// <summary>
    /// Set the cancellation status of an event.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancelled">Cancellation status</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<bool> SetCancelledAsync(Guid id, bool cancelled, CancellationToken cf = default);
    
    // insert if no key is present for event
    // update if key is present for event
    /// <summary>
    /// Add or update metadata for an event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="metadata">The metadata input model</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>The updated event view model</returns>
    Task<EventVM?> AddMetadataAsync(Guid eventId, EventMetadataIM metadata, CancellationToken cf = default);
    
    /// <summary>
    /// Delete metadata for an event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="key">The metadata key</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>The updated event view model</returns>
    Task<EventVM?> DeleteMetadataAsync(Guid eventId, string key, CancellationToken cf = default);

    /// <summary>
    /// Delete an event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<bool> DeleteAsync(Guid eventId, CancellationToken cf = default);
}