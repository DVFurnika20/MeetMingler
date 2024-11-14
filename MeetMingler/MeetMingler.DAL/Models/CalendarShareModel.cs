using System.ComponentModel.DataAnnotations.Schema;

namespace MeetMingler.DAL.Models
{
    public class CalendarShare
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public string UserId { get; set; }
        public CalendarAccessLevel AccessLevel { get; set; }
        
        public virtual Calendar Calendar { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public enum CalendarAccessLevel
    {
        Viewer,
        Editor,
        Admin
    }
}