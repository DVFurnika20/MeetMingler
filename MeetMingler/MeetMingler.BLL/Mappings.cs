using AutoMapper;
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
    }
}
