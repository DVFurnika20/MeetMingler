using System.ComponentModel.DataAnnotations;

namespace MeetMingler.BLL.Models.User;

public class PasswordUM
{
    [Required] public string Current { get; set; } = null!;

    [Required] public string New { get; set; } = null!;
}