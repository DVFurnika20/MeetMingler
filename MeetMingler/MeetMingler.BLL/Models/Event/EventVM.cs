using MeetMingler.BLL.Models.User;

namespace MeetMingler.BLL.Models.Event;

public class EventVM : EventBase
{
    public UserVM Creator { get; set; }

    public IEnumerable<EventMetadataVM> Metadata { get; set; } = null!;
}