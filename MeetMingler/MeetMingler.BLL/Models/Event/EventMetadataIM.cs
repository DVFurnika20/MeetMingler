using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.Event;

public class EventMetadataIM
{
    [Required] public string Key { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;
}