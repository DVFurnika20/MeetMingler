using MeetMingler.BLL.Services.Contracts;
using MeetMingler.DAL.Models;

namespace MeetMingler.BLL.Services.Implementations;

public class CurrentUser : ICurrentUser
{
    public User User { get; set; } = null!;
}