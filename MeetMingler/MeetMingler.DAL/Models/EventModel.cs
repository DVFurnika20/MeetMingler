using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetMingler.DAL.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public EventStatus Status { get; set; }
        public int CalendarId { get; set; }
        public string CreatorId { get; set; }
        
        public virtual Calendar Calendar { get; set; }
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; }
        public virtual ICollection<EventParticipant> Participants { get; set; }
    }

    public enum EventStatus
    {
        Draft,
        Scheduled,
        Cancelled,
        Completed
    }
}