using AutoMapper;
using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Data;
using MeetMingler.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.BLL.Services.Implementations;

public class EventService(IMapper mapper, ApplicationDbContext context) : IEventService
{
    public async Task<EventVM?> CreateAsync(EventIM im, CancellationToken cf = default)
    {
        // map input model to entity
        var eventEntity = mapper.Map<Event>(im);
        var eventMetadata = im.Metadata;
        
        // insert event and metadata
        await context.Events.AddAsync(eventEntity, cf);
        await context.EventMetadataEntries.AddRangeAsync(eventMetadata.Select(e =>
        {
            var m = mapper.Map<EventMetadata>(e);
            m.EventId = eventEntity.Id;
            return m;
        }), cf);
        
        // save changes and map to view model
        await context.SaveChangesAsync(cf);
        var mappedEvent = mapper.Map<EventVM>(eventEntity);
        mappedEvent.Metadata = eventMetadata.Select(mapper.Map<EventMetadataVM>);
        
        // return created event
        return mappedEvent;
    }

    public async Task<EventVM?> GetByIdAsync(Guid id, CancellationToken cf = default)
    {
        // retrieve event by id
        var eventEntity = await context.Events
            .AsNoTracking()
            .Include(@event => @event.Metadata)
            .FirstOrDefaultAsync(e => e.Id == id, cf);
        
        // if event not found, return null
        if (eventEntity == null)
        {
            return null;
        }
        
        // map to EventVM
        var mappedEvent = mapper.Map<EventVM>(eventEntity);
        mappedEvent.Metadata = eventEntity.Metadata.Select(mapper.Map<EventMetadataVM>);
        
        // return event view model
        return mappedEvent;
    }

    public async Task<IEnumerable<EventVM>> GetCollectionByCreatorAsync(Guid creatorId, List<string> includeMetadataKeys,
        CancellationToken cf = default)
    {
        // retrieve events by creator and include metadata keys
        var query = await context.Events.AsNoTracking()
            .Join(context.EventMetadataEntries,
                @event => @event.Id,
                eventMetadata => eventMetadata.EventId,
                (@event, eventMetadata) => new { Event = @event, MetadataEntry = eventMetadata })
            .Where(e => e.Event.CreatorId == creatorId
                        && includeMetadataKeys.Contains(e.MetadataEntry.Key))
            .ToListAsync(cf);

        // map to EventVM
        return query.Select(e => e.Event).Distinct().Select(e =>
        {
            var eventVm = mapper.Map<EventVM>(e);
            
            eventVm.Metadata = query
                .Where(em => em.Event.Id == e.Id)
                .Select(em => em.MetadataEntry)
                .Select(mapper.Map<EventMetadataVM>);
            
            return eventVm;
        });
    }

    public IQueryable<string> GetDistinctMetadataValuesAsync(string metadataKey,
        CancellationToken cf = default)
    {
        // retrieve distinct metadata values for a given metadata key
        return context.EventMetadataEntries
            .Where(em => em.Key == metadataKey)
            .OrderBy(em => em.Value)
            .Select(em => em.Value)
            .Take(20)
            .Distinct();
    }

    public async Task<BaseCollectionVM<EventVM>> GetAllPaginatedAndFilteredAsync(PaginationOptions paginationOptions,
        EventFilter filter, CancellationToken cf = default)
    {
        var query = context.Events.AsNoTracking();

        // filter for title and description by the property TextSearch
        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch)
                                     || e.Description.Contains(filter.TextSearch));
        }

        // pagination stage
        query = query
            .Skip((paginationOptions.Page - 1) * paginationOptions.PageSize)
            .Take(paginationOptions.PageSize);
        
        // retrieve events and their count
        var count = await query.CountAsync(cf);
        var items = await query.ToListAsync(cf);
        
        // join paginated events with their corresponding metadata
        var metadataQuery = await context.EventMetadataEntries.AsNoTracking().Join(items,
            m => m.Id,
            e => e.Id,
            (m, e) => new { Event = e, MetadataEntry = m })
            .ToListAsync(cf);
        
        // map to EventVM
        var result = metadataQuery.Select(e => e.Event).Distinct().Select(e =>
        {
            var eventVm = mapper.Map<EventVM>(e);
            
            eventVm.Metadata = metadataQuery
                .Where(em => em.Event.Id == e.Id)
                .Select(em => em.MetadataEntry)
                .Select(mapper.Map<EventMetadataVM>);
            
            return eventVm;
        }); 
        
        // return paginated and filtered events
        return new BaseCollectionVM<EventVM>
        {
            Items = result,
            Count = count
        };
    }
    
    public async Task DeleteAsync(Guid eventId, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId, cf);
        
        // if event not found, throw exception
        if (eventEntity == null)
        {
            throw new KeyNotFoundException("Event not found.");
        }
        // remove event and save changes
        context.Events.Remove(eventEntity);
        await context.SaveChangesAsync(cf);
    }

    public async Task<EventVM?> AddMetadataAsync(Guid eventId, EventMetadataIM metadata, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events
            .Include(e => e.Metadata)
            .FirstOrDefaultAsync(e => e.Id == eventId, cf);

        // if event not found, throw exception
        if (eventEntity == null)
        {
            throw new KeyNotFoundException("Event not found.");
        }
        
        // check if metadata key already exists
        var existingMetadata = eventEntity.Metadata.FirstOrDefault(m => m.Key == metadata.Key);
        if (existingMetadata != null)
        {
            // update existing metadata
            existingMetadata.Value = metadata.Value;
            context.EventMetadataEntries.Update(existingMetadata);
        }
        else
        {
            // insert new metadata
            var newMetadata = new EventMetadata
            {
                EventId = eventId,
                Key = metadata.Key,
                Value = metadata.Value
            };
            await context.EventMetadataEntries.AddAsync(newMetadata, cf);
        }
        // save changes and return updated event
        await context.SaveChangesAsync(cf);
        return await GetByIdAsync(eventId, cf);
    }

    public async Task<EventVM?> DeleteMetadataAsync(Guid eventId, string key, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events
            .Include(e => e.Metadata)
            .FirstOrDefaultAsync(e => e.Id == eventId, cf);
        
        // if event not found, throw exception
        if (eventEntity == null)
        {
            throw new KeyNotFoundException("Event not found.");
        }
        
        // if metadata key not found, throw exception
        if (eventEntity.Metadata.FirstOrDefault(m => m.Key == key) == null)
        {
            throw new KeyNotFoundException("Metadata not found.");
        }
        
        // remove metadata and save changes
        context.EventMetadataEntries.Remove(eventEntity.Metadata.FirstOrDefault(m => m.Key == key)!);
        await context.SaveChangesAsync(cf);
        return await GetByIdAsync(eventId, cf);
    }

    public async Task SetCancelledAsync(Guid id, bool cancelled, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cf);
        if (eventEntity == null)
        {
            throw new KeyNotFoundException("Event not found.");
        }
        
        // update cancellation status and save changes
        eventEntity.Cancelled = cancelled;
        context.Events.Update(eventEntity);
        await context.SaveChangesAsync(cf);
    }
}