namespace BookingManagement.Models
{
    public class MeetingAttendee
    {        
        public int MeetingID { get; set; }
        public int AttendeeID { get; set; }

        public virtual Meeting Meeting { get; set; }
    }
}