using System;

namespace MeetMingler.DAL.Models
{
    public class EventParticipant
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public ParticipationStatus Status { get; set; }
        public DateTime? ResponseDate { get; set; }
        
        public virtual Event Event { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public enum ParticipationStatus
    {
        Pending,
        Accepted,
        Declined,
        MaybeAttending
    }
}