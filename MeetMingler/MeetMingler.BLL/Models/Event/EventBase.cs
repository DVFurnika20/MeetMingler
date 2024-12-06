using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.Event;

public class EventBase
{
    [Required] public string Title { get; set; } = null!;
    
    [Required] public string Description { get; set; } = null!;
    
    [Required] public DateTime StartTime { get; set; }
    
    [Required] public DateTime EndTime { get; set; }
    
    [Required] public bool Cancelled { get; set; }
}