using MeetMingler.BLL.Models.User;

namespace MeetMingler.BLL.Models.Event;

public class EventVM : EventBase
{
    public Guid Id { get; set; }
    
    public UserVM Creator { get; set; }
    
    public bool Cancelled { get; set; }
    
    public IEnumerable<EventMetadataVM> Metadata { get; set; } = null!;
}