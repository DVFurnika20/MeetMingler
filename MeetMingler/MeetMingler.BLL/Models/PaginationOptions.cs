using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models;

public enum SortOrder
{
    Ascending,
    Descending
}

public class PaginationOptions
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Page { get; set; }
    
    [Required]
    [Range(2, 15)]
    public int PageSize { get; set; }
    
    public string? SortByColumn { get; set; }
    
    public SortOrder? Order { get; set; }
}
