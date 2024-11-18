using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetMingler.DAL.Models;

public class EventMetadata
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid EventId { get; set; }

    [Required] public string Key { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;

    [ForeignKey(nameof(EventId))] public virtual Event Event { get; set; } = null!;
}