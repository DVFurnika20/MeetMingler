using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.DAL.Models;

[PrimaryKey(nameof(UserId), nameof(EventId))]
public class EventParticipant
{
    public Guid UserId { get; set; }
    
    public Guid EventId { get; set; }
    
    public ParticipationStatus Status { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}

public enum ParticipationStatus
{
    Pending,
    Accepted,
    Declined
}
