using System.ComponentModel.DataAnnotations;
using MeetMingler.BLL.Models.User;

namespace MeetMingler.BLL.Models.Event;

public class EventIM : EventBase
{
    [Required] public Guid CreatorId { get; set; }
}