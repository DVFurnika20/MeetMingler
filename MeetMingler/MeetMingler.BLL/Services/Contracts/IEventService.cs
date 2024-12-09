using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Models.User;
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
    Task<EventExtendedVM?> GetByIdAsync(Guid id, CancellationToken cf = default);

    /// <summary>
    /// Retrieve all events created by a user
    /// </summary>
    /// <param name="creatorId">The user's unique identifier</param>
    /// <param name="includeMetadataKeys">The metadata key/value pairs to include in the resultset</param>
    /// <param name="pagination"></param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of events</returns>
    Task<IEnumerable<EventVM>> GetCollectionByCreatorAsync(Guid creatorId, List<string> includeMetadataKeys,
        PaginationOptions pagination, CancellationToken cf = default);

    /// <summary>
    /// Retrieve all distinct metadata values for a given metadata key
    /// </summary>
    /// <param name="metadataKey">Metadata key</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of values related to the metadata key</returns>
    IQueryable<string> GetDistinctMetadataValuesAsync(string metadataKey, CancellationToken cf = default);
 
    /// <summary>
    /// Update the event title, description, start date, and end date.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="updateModel">The event update model</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>The updated event view model</returns>
    Task<EventVM?> UpdateAsync(Guid eventId, EventUM updateModel, CancellationToken cf = default);

    /// <summary>
    /// Set the cancellation status of an event.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancelled">Cancellation status</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<bool> SetCancelledAsync(Guid id, bool cancelled, CancellationToken cf = default);
    
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
    
    /// <summary>
    /// Register a user for attendance to an event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<bool> RegisterUserForEventAsync(Guid eventId, CancellationToken cf = default);

    /// <summary>
    /// Retrieve dates on which events are happening.
    /// </summary>
    /// <param name="startDateRange">The start of the range of the search</param>
    /// <param name="endDateRange">The end of the range of the search</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of dates on which events are happening</returns>
    Task<IEnumerable<DateTime>> GetDatesAsync(DateTime startDateRange, DateTime endDateRange, CancellationToken cf = default);

    /// <summary>
    /// Gets all the participants' user data of an event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <param name="pagination">Pagination options</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns>A collection of users which are attending </returns>
    public Task<BaseCollectionVM<UserVM>?> GetAttendees(Guid eventId, PaginationOptions pagination,
        CancellationToken cf = default);

    /// <summary>
    /// Gets all the events that a user is participating in
    /// </summary>
    /// <param name="pagination">Pagination options</param>
    /// <param name="cf">Cancellation token</param>
    /// <returns></returns>
    public Task<BaseCollectionVM<EventVM>?> GetCurrentUserAttendance(PaginationOptions pagination,
        CancellationToken cf = default);
}