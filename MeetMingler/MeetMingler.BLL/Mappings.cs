using AutoMapper;
using MeetMingler.BLL.Models.Event;
using MeetMingler.BLL.Models.User;
using MeetMingler.DAL.Models;

namespace MeetMingler.BLL;

internal class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<UserIM, User>();
        CreateMap<User, UserIM>();
        CreateMap<User, UserVM>();

        CreateMap<EventIM, Event>();
        CreateMap<Event, EventVM>();
        
        CreateMap<EventUM, Event>();
        CreateMap<Event, EventUM>();
        
        CreateMap<EventMetadataIM, EventMetadata>();
        CreateMap<EventMetadata, EventMetadataVM>();
    }
}
