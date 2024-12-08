using System.ComponentModel.DataAnnotations;
using MeetMingler.BLL.Models.User;

namespace MeetMingler.BLL.Models.Event;

public class EventVM : EventBase
{
    [Required] public Guid Id { get; set; }
    
    [Required] public UserVM Creator { get; set; }
    
    [Required] public bool Cancelled { get; set; }
    
    [Required] public IEnumerable<EventMetadataVM> Metadata { get; set; } = null!;
}