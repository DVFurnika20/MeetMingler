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
        context.Set<Event>().Add(eventEntity);
        await context.SaveChangesAsync(cf);
        return mapper.Map<EventVM>(eventEntity);
    }
 
    public async Task<EventVM> GetEventByIdAsync(Guid id, CancellationToken cf = default)
    {
        var eventEntity = await context.Set<Event>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cf);
        return mapper.Map<EventVM>(eventEntity);
    }
 
    public async Task<IQueryable<EventVM>> GetEventsByCreatorIdAsync(Guid creatorId, List<string> includeMetadataKeys, CancellationToken cf = default)
    {
        var query = context.Set<Event>().AsNoTracking().Where(e => e.CreatorId == creatorId).AsQueryable();
 
        query = includeMetadataKeys.Aggregate(query, (current, key) => current.Include(key));
 
        var eventEntities = await query.ToListAsync(cf);
        return eventEntities.AsQueryable().Select(e => mapper.Map<EventVM>(e));
    }
 
    public async Task<IQueryable<string>> GetDistinctMetadataValuesAsync(string metadataKey, CancellationToken cf = default)
    {
        var values = await context.Set<MeetMingler.BLL.Models.Event.EventMetadata>()
            .Where(em => em.Key == metadataKey)
            .Select(em => em.Value)
            .Distinct()
            .ToListAsync(cf);
 
        return values.AsQueryable();
    }
 
    public async Task<IQueryable<EventVM>> GetFilteredCollectionAsync(EventFilter filter, CancellationToken cf = default)
    {
        var query = context.Set<Event>().AsQueryable();
 
        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch) || e.Description.Contains(filter.TextSearch));
        }
 
        var eventEntities = await query.ToListAsync(cf);
        return eventEntities.AsQueryable().Select(e => mapper.Map<EventVM>(e));
    }
 
    public async Task<BaseCollectionVM<EventVM>> GetAllPaginatedAndFilteredAsync(PaginationOptions paginationOptions, EventFilter filter, CancellationToken cf = default)
    {
        var query = context.Set<Event>().AsQueryable();
 
        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch) || e.Description.Contains(filter.TextSearch));
        }
 
        var items = query
            .Skip((paginationOptions.Page - 1) * paginationOptions.PageSize)
            .Take(paginationOptions.PageSize)
            .Select(e => mapper.Map<EventVM>(e));
 
        return new BaseCollectionVM<EventVM>
        {
            Items = items,
            Count = await items.CountAsync(cf)
        };
    }
}