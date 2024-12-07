using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Filters;

public class EventFilter
{
    /// <summary>
    /// Metadata key/value pairs to include in the result set
    /// </summary>
    public List<string> IncludeMetadataKeys { get; set; } = [];

    /// <summary>
    /// Metadata filters to apply to the result set
    /// </summary>
    public Dictionary<string, string> MetadataFilters { get; set; } = new();

    /// <summary>
    /// General text search to filter out both titles and descriptions
    /// </summary>
    [Required] public string TextSearch { get; set; } = null!;
}