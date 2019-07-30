using Kendo.Mvc.UI;
using System;

namespace BookingManagement.Models.ViewModels
{
    public class AttendeeViewModel : ISchedulerEvent
    {
        #region Default Implementation
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        #endregion

        #region Attendee's properties
        public int AttendeeID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Color { get; set; }

        public Attendee ToEntity()
        {
            var attendee = new Attendee
            {
                AttendeeID = AttendeeID,
                Name = Name,
                Email = Email,
                Password = Password,
                Salt = Salt,
                Color = Color
            };
            return attendee;
        }
        #endregion
    }
}
