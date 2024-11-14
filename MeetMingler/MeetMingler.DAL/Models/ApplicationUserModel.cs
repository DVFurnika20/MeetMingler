using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MeetMingler.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public virtual ICollection<Event> CreatedEvents { get; set; }
        public virtual ICollection<EventParticipant> EventParticipations { get; set; }
        public virtual ICollection<Calendar> Calendars { get; set; }
    }
}