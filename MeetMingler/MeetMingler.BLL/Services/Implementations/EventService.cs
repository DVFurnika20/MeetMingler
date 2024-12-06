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
        var eventEntity = mapper.Map<Event>(im);
        var eventMetadata = im.Metadata;

        await context.Events.AddAsync(eventEntity, cf);
        await context.EventMetadataEntries.AddRangeAsync(eventMetadata.Select(e =>
        {
            var m = mapper.Map<EventMetadata>(e);
            m.EventId = eventEntity.Id;
            return m;
        }), cf);

        await context.SaveChangesAsync(cf);
        var mappedEvent = mapper.Map<EventVM>(eventEntity);
        mappedEvent.Metadata = eventMetadata.Select(mapper.Map<EventMetadataVM>);

        return mappedEvent;
    }

    public async Task<EventVM?> GetByIdAsync(Guid id, CancellationToken cf = default)
    {
        var eventEntity = await context.Events
            .AsNoTracking()
            .Include(@event => @event.Metadata)
            .FirstOrDefaultAsync(e => e.Id == id, cf);

        if (eventEntity == null)
        {
            return null;
        }

        var mappedEvent = mapper.Map<EventVM>(eventEntity);
        mappedEvent.Metadata = eventEntity.Metadata.Select(mapper.Map<EventMetadataVM>);

        return mappedEvent;
    }

    public async Task<IEnumerable<EventVM>> GetCollectionByCreatorAsync(Guid creatorId, List<string> includeMetadataKeys,
        CancellationToken cf = default)
    {
        var query = await context.Events.AsNoTracking()
            .Join(context.EventMetadataEntries,
                @event => @event.Id,
                eventMetadata => eventMetadata.EventId,
                (@event, eventMetadata) => new { Event = @event, MetadataEntry = eventMetadata })
            .Where(e => e.Event.CreatorId == creatorId
                        && includeMetadataKeys.Contains(e.MetadataEntry.Key))
            .ToListAsync(cf);

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

        return new BaseCollectionVM<EventVM>
        {
            Items = result,
            Count = count
        };
    }

    public Task DeleteAsync(Guid eventId, CancellationToken cf = default)
    {
        throw new NotImplementedException();
    }

    public Task<EventVM?> AddMetadataAsync(Guid eventId, EventMetadataIM metadata, CancellationToken cf = default)
    {
        throw new NotImplementedException();
    }

    public Task<EventVM?> DeleteMetadataAsync(Guid eventId, string key, CancellationToken cf = default)
    {
        throw new NotImplementedException();
    }

    public Task SetCancelledAsync(Guid id, bool cancelled, CancellationToken cf = default)
    {
        throw new NotImplementedException();
    }
}