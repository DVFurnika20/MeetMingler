﻿using System.Linq.Expressions;
using AutoMapper;
using MeetMingler.BLL.Filters;
using MeetMingler.BLL.Models;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Models.User;
using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Data;
using MeetMingler.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.BLL.Services.Implementations;

public class EventService(IMapper mapper, ICurrentUser currentUser, ApplicationDbContext context) : IEventService
{
    public async Task<EventVM?> CreateAsync(EventIM im, CancellationToken cf = default)
    {
        // map input model to entity
        var eventEntity = mapper.Map<Event>(im);
        eventEntity.CreatorId = currentUser.User.Id;

        // insert event and metadata
        await context.Events.AddAsync(eventEntity, cf);
        // await context.EventMetadataEntries.AddRangeAsync(eventMetadata, cf);

        // save changes and map to view model
        await context.SaveChangesAsync(cf);
        var mappedEvent = mapper.Map<EventVM>(eventEntity);

        // return created event
        return mappedEvent;
    }

    public async Task<EventExtendedVM?> GetByIdAsync(Guid id, CancellationToken cf = default)
    {
        // retrieve event by id
        var eventEntity = await context.Events
            .AsNoTracking()
            .Include(@event => @event.Metadata)
            .Include(@event => @event.Creator)
            .FirstOrDefaultAsync(e => e.Id == id, cf);
        
        var attending = await context.EventParticipants
            .AnyAsync(ep => ep.EventId == id && ep.UserId == currentUser.User.Id, cf);

        // if event not found, return null
        if (eventEntity == null)
        {
            return null;
        }

        // map to EventVM
        var mappedEvent = mapper.Map<EventExtendedVM>(eventEntity);
        mappedEvent.Attending = attending;
        mappedEvent.Metadata = eventEntity.Metadata.Select(mapper.Map<EventMetadataVM>);

        // return event view model
        return mappedEvent;
    }

    public async Task<IEnumerable<EventVM>> GetCollectionByCreatorAsync(Guid creatorId,
        List<string> includeMetadataKeys,
        PaginationOptions pagination,
        CancellationToken cf = default)
    {
        // retrieve events by creator and include metadata keys
        var query = await context.Events.AsNoTracking()
            .Include(@event => @event.Metadata)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cf);

        // map to EventVM
        return query.Select(e =>
        {
            var eventVm = mapper.Map<EventVM>(e);

            eventVm.Metadata = eventVm.Metadata
                .Where(em => includeMetadataKeys.Contains(em.Key)).ToList();

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
        var query = context.Events.Include(e => e.Metadata).AsNoTracking();

        // filter for title and description by the property TextSearch
        if (!string.IsNullOrEmpty(filter.TextSearch))
        {
            query = query.Where(e => e.Title.Contains(filter.TextSearch)
                                     || e.Description.Contains(filter.TextSearch));
        }

        var parameter = Expression.Parameter(typeof(EventMetadata), "x");
        Expression? combinedCondition = null;
        foreach (var metadataFilter in filter.MetadataFilters)
        {
            // Build conditions for [key] == 'KeyN' and [value] == 'ValueN'
            var keyProperty = Expression.Property(parameter, nameof(EventMetadata.Key));
            var valueProperty = Expression.Property(parameter, nameof(EventMetadata.Value));

            var keyCondition = Expression.Equal(keyProperty, Expression.Constant(metadataFilter.Key));
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
            var containsExpression = Expression.Call(
                Expression.Constant(metadataFilter.Value),
                containsMethod!,
                valueProperty);
            // var valueCondition = Expression.Equal(valueProperty, Expression.Constant(metadataFilter.Value));

            var combinedFilter = Expression.AndAlso(keyCondition, containsExpression);

            // Combine with OR
            combinedCondition = combinedCondition == null
                ? combinedFilter
                : Expression.OrElse(combinedCondition, combinedFilter);
        }

        if (combinedCondition != null)
        {
            var events = query.Join(
                    context.EventMetadataEntries.Where(
                        Expression.Lambda<Func<EventMetadata, bool>>(combinedCondition, parameter)),
                    e => e.Id,
                    m => m.EventId,
                    (e, m) => new { Event = e, MetadataEntry = m })
                .GroupBy(e => e.Event);
            
            // let's hope entity framework streams the resultset instead of fetching the whole thing
            List<EventVM> result = [];
            var skipCounter = 0;
            var takeCounter = 0;
            foreach (var eventGroup in events)
            {
                skipCounter++;
                if (skipCounter < paginationOptions.PageSize * (paginationOptions.Page - 1))
                    continue;

                if (takeCounter >= paginationOptions.PageSize)
                    break;

                var @event = mapper.Map<EventVM>(eventGroup.Key);
                @event.Metadata = eventGroup
                    .Where(em => filter.IncludeMetadataKeys.Contains(em.MetadataEntry.Key))
                    .Select(e => mapper.Map<EventMetadataVM>(e.MetadataEntry)).ToList();
                result.Add(@event);
                
                takeCounter++;
            }
            
            return new BaseCollectionVM<EventVM>
            {
                Items = result,
                Count = result.Count
            };
        }
        else
        {
            var events = await query
                .Include(e => e.Creator)
                .Include(e => e.Metadata)
                .Skip((paginationOptions.Page - 1) * paginationOptions.PageSize)
                .Take(paginationOptions.PageSize)
                .ToListAsync(cf);
            
            var count = await query.CountAsync(cf);
            
            // map to EventVM
            var result = events.Select(e =>
            {
                var eventVm = mapper.Map<EventVM>(e);

                eventVm.Metadata = eventVm.Metadata
                    .Where(em => filter.IncludeMetadataKeys.Contains(em.Key)).ToList();

                return eventVm;
            });

            // return paginated and filtered events
            return new BaseCollectionVM<EventVM>
            {
                Items = result,
                Count = count
            };
        }
    }

    public async Task<bool> DeleteAsync(Guid eventId, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId, cf);

        // if event not found, return null
        if (eventEntity == null)
        {
            return false;
        }

        if (eventEntity.CreatorId != currentUser.User.Id)
        {
            return false;
        }

        // remove event and save changes
        context.Events.Remove(eventEntity);
        await context.SaveChangesAsync(cf);

        return true;
    }

    public async Task<EventVM?> AddMetadataAsync(Guid eventId, EventMetadataIM metadata, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events
            .Include(e => e.Metadata)
            .FirstOrDefaultAsync(e => e.Id == eventId, cf);

        // if event not found, return null
        if (eventEntity == null)
        {
            return null;
        }

        // if event doesn't belong to currentUser return null
        if (eventEntity.CreatorId != currentUser.User.Id)
        {
            return null;
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

        // if event not found, return null
        if (eventEntity == null)
        {
            return null;
        }

        // if event doesn't belong to currentUser return null
        if (eventEntity.CreatorId != currentUser.User.Id)
        {
            return null;
        }

        // if metadata key not found, return null
        if (eventEntity.Metadata.FirstOrDefault(m => m.Key == key) == null)
        {
            return null;
        }

        // remove metadata and save changes
        context.EventMetadataEntries.Remove(eventEntity.Metadata.FirstOrDefault(m => m.Key == key)!);
        await context.SaveChangesAsync(cf);
        return await GetByIdAsync(eventId, cf);
    }

    public async Task<bool> SetCancelledAsync(Guid id, bool cancelled, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cf);

        // if event doesn't exist return false
        if (eventEntity == null)
        {
            return false;
        }

        // if event doesn't belong to currentUser return false
        if (eventEntity.CreatorId != currentUser.User.Id)
        {
            return false;
        }

        // update cancellation status and save changes
        eventEntity.Cancelled = cancelled;
        context.Events.Update(eventEntity);
        await context.SaveChangesAsync(cf);
        return true;
    }

    public async Task<bool> RegisterUserForEventAsync(Guid eventId, CancellationToken cf = default)
    {
        var userId = currentUser.User.Id;
        
        // retrieve event
        var eventEntity = await context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId, cf);

        // if event doesn't exist return false
        if (eventEntity == null)
        {
            return false;
        }

        // check if user is already registered for the event
        var isUserRegistered = await context.EventParticipants
            .AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId, cf);

        if (isUserRegistered)
        {
            return false;
        }

        // register user for event
        eventEntity.Participants.Add(new EventParticipant
        {
            EventId = eventId,
            UserId = userId,
        });

        // save changes
        await context.SaveChangesAsync(cf);
        return true;
    }

    public async Task<IEnumerable<DateTime>> GetDatesAsync(DateTime startDateRange, DateTime endDateRange, CancellationToken cf = default)
    {
        // retrieve dates on which events are happening within the given date range
        return await context.Events
            .Where(e => e.StartTime >= startDateRange && e.StartTime <= endDateRange)
            .Select(e => e.StartTime.Date)
            .Distinct()
            .ToListAsync(cf);
    }
    
    public async Task<EventVM?> UpdateAsync(Guid eventId, EventUM updateModel, CancellationToken cf = default)
    {
        // retrieve event
        var eventEntity = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId, cf);

        // if event doesn't exist return null
        if (eventEntity == null)
        {
            return null;
        }

        // if event doesn't belong to currentUser return null
        if (eventEntity.CreatorId != currentUser.User.Id)
        {
            return null;
        }

        // map the updateModel to entity
        mapper.Map(updateModel, eventEntity);

        // update entity state and save changes
        context.Entry(eventEntity).State = EntityState.Modified;
        await context.SaveChangesAsync(cf);

        // map to EventVM and return
        return mapper.Map<EventVM>(eventEntity);
    }

    public async Task<BaseCollectionVM<UserVM>?> GetAttendees(Guid eventId, PaginationOptions pagination,
        CancellationToken cf = default)
    {
        var participants = await context.EventParticipants
            .Join(context.Users, participant => participant.UserId,
                user => user.Id, (participant, user) => new { Participant = participant, User = user })
            .Where(p => p.Participant.EventId == eventId)
            .Select(p => mapper.Map<UserVM>(p.User))
            .Take(pagination.PageSize)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .ToListAsync(cf);

        if (participants.Count == 0)
            return null;

        return new BaseCollectionVM<UserVM>
        {
            Count = participants.Count,
            Items = participants
        };
    }

    public async Task<BaseCollectionVM<EventVM>?> GetCurrentUserAttendance(PaginationOptions pagination,
        CancellationToken cf = default)
    {
        var events = await context.EventParticipants
            .Join(context.Events, participant => participant.EventId, @event => @event.Id,
                (participant, @event) => new { Participant = participant, Event = @event })
            .Where(p => p.Participant.UserId == currentUser.User.Id)
            .Select(p => mapper.Map<EventVM>(p.Event))
            .Take(pagination.PageSize)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .ToListAsync(cf);
        
        if (events.Count == 0)
            return null;

        return new BaseCollectionVM<EventVM>()
        {
            Count = events.Count,
            Items = events
        };
    }
}