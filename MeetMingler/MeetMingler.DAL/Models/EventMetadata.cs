using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MeetMingler.DAL.Models;

[Index(nameof(EventId), nameof(Key), IsUnique = true)]
public class EventMetadata
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid EventId { get; set; }

    [Required] public string Key { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;

    [ForeignKey(nameof(EventId))] public virtual Event Event { get; set; } = null!;
}