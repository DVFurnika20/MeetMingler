using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.DAL.Models;

[PrimaryKey(nameof(UserId), nameof(EventId))]
public class EventParticipant
{
    public Guid UserId { get; set; }
    
    public Guid EventId { get; set; }
    
    public ParticipationStatus Status { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(EventId))]
    public virtual Event Event { get; set; } = null!;
}

public enum ParticipationStatus
{
    Pending,
    Accepted,
    Declined
}
