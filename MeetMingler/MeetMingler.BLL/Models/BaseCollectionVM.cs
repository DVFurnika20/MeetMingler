using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models;

public class BaseCollectionVM<T> where T : class
{
    [Required] public IQueryable<T> Items { get; set; } = null!;
    
    [Required] public int Count { get; set; }
}
