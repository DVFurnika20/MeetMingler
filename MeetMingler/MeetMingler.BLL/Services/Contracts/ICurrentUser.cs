using MeetMingler.DAL.Models;

namespace MeetMingler.BLL.Services.Contracts;

public interface ICurrentUser
{
    public User User { get; set; }
}