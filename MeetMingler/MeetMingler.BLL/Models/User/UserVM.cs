using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.User;

public class UserVM
{
    [Required] public Guid Id { get; set; }
    
    [Required] public string FirstName { get; set; } = null!;

    [Required] public string LastName { get; set; } = null!;
    
    [Required] public string Email { get; set; } = null!;
    
    [Required] public string UserName { get; set; } = null!;
}