using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetMingler.DAL.Models
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string OwnerId { get; set; }
        
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<CalendarShare> SharedWith { get; set; }
    }
}