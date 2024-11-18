using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MeetMingler.DAL.Models;

public class User : IdentityUser<Guid>
{
    [MaxLength(100)] [Required] public string FirstName { get; set; } = null!;

    [MaxLength(100)] [Required] public string LastName { get; set; } = null!;
}