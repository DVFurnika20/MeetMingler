using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.Event;

public class EventDictionaryIM : EventBase
{
    [Required] public IDictionary<string, string> Metadata { get; set; } = null!;
}
