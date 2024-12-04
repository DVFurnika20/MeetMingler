using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.User;

public class AuthTokenVM
{
    [Required] public string Token { get; set; } = null!;

    [Required] public DateTime ExpirationDate { get; set; }
}