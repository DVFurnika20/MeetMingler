namespace MeetMingler.BLL.Filters;

public class EventFilter
{
    public List<string> IncludeMetadataKeys { get; set; } = [];
    
    public List<Dictionary<string, string>> MetadataFilters { get; set; } = [];

    public string TextSearch { get; set; } = null!;
}