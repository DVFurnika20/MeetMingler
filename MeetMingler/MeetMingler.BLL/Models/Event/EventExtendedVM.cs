using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.Event;

public class EventExtendedVM : EventVM
{
    [Required] public bool Attending { get; set; }
}