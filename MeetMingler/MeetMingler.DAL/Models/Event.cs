using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetMingler.DAL.Models;

public class Event
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CreatorId { get; set; }

    [Required] [MaxLength(100)] public string Title { get; set; } = null!;
    
    [MaxLength(4000)] public string Description { get; set; } = null!;
    
    [Required] public DateTime StartTime { get; set; }

    [Required] public DateTime EndTime { get; set; }

    [Required] public bool Cancelled { get; set; } = false;

    [ForeignKey(nameof(CreatorId))]
    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<EventParticipant> Participants { get; set; } = null!;
    
    public virtual ICollection<EventMetadata> Metadata { get; set; } = null!;
}