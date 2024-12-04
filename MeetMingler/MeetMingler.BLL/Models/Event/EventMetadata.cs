using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.Event;

public class EventMetadata
{
    [Required] public string Key { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;
}