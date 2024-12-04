using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.User;

public class UserLoginIM
{
    [Required] public string Email { get; set; } = null!;
    
    [Required] public string Password { get; set; } = null!;
}
