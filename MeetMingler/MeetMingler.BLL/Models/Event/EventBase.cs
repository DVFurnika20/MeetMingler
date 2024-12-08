using System.ComponentModel.DataAnnotations;
using MeetMingler.BLL.Validators;

namespace MeetMingler.BLL.Models.Event;

public class EventBase
{
    [Required] public string Title { get; set; } = null!;
    
    [Required] public string Description { get; set; } = null!;
    
    [Required] public DateTime StartTime { get; set; }
    
    [GreaterThan(nameof(StartTime))] 
    [Required]
    public DateTime EndTime { get; set; }
}