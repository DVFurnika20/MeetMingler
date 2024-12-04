using AutoMapper;
using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.BLL.Services.Implementations;

public class EventService : IEventService
{
    private readonly IMapper _mapper;
    private readonly DbContext _context;

    public EventService(IMapper mapper, DbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<EventVM?> CreateAsync(EventIM im, CancellationToken cf = default)
    {
        var eventEntity = _mapper.Map<Event>(im);
        _context.Set<Event>().Add(eventEntity);
        await _context.SaveChangesAsync(cf);
        return _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<EventVM> GetEventByIdAsync(Guid id)
    {
        var eventEntity = await _context.Set<Event>().FindAsync(id);
        return _mapper.Map<EventVM>(eventEntity);
    }

    public async Task<IQueryable<EventVM>> GetEventsByCreatorIdAsync(Guid creatorId, List<string> includeMetadataKeys)
    {
        var query = _context.Set<Event>().Where(e => e.CreatorId == creatorId).AsQueryable();

        query = includeMetadataKeys.Aggregate(query, (current, key) => current.Include(key));

        var eventEntities = await query.ToListAsync();
        return eventEntities.AsQueryable().Select(e => _mapper.Map<EventVM>(e));
    }

    public async Task<IQueryable<string>> GetDistinctMetadataValuesAsync(string metadataKey)
    {
        var values = await _context.Set<MeetMingler.DAL.Models.EventMetadata>()
            .Where(em => em.Key == metadataKey)
            .Select(em => em.Value)
            .Distinct()
            .ToListAsync();

        return values.AsQueryable();
    }

    public async Task<IQueryable<EventVM>> GetFilteredCollectionAsync(EventFilter filter, CancellationToken cf = default)
    {
        var query = _context.Set<Event>().AsQueryable();

        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch) || e.Description.Contains(filter.TextSearch));
        }

        var eventEntities = await query.ToListAsync(cf);
        return eventEntities.AsQueryable().Select(e => _mapper.Map<EventVM>(e));
    }

    public async Task<BaseCollectionVM<EventVM>> GetAllPaginatedAndFilteredAsync(PaginationOptions paginationOptions, EventFilter filter, CancellationToken cf = default)
    {
        var query = _context.Set<Event>().AsQueryable();

        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch) || e.Description.Contains(filter.TextSearch));
        }
        
        var items = query
            .Skip((paginationOptions.Page - 1) * paginationOptions.PageSize)
            .Take(paginationOptions.PageSize)
            .Select(e => _mapper.Map<EventVM>(e));

        return new BaseCollectionVM<EventVM>
        {
            Items = items,
            Count = await items.CountAsync(cf)
        };
    }
}