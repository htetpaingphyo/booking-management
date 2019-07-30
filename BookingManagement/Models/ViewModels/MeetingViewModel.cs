using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;

namespace BookingManagement.Models.ViewModels
{
    public class MeetingViewModel : ISchedulerEvent
    {
        // Customized properties...
        public int MeetingID { get; set; }
        public int OwnerID { get; set; }
        public int? RecurrenceID { get; set; }
        public int? RoomID { get; set; }
        public IEnumerable<int> Attendees { get; set; }

        // Default implementations...
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }

        public Meeting ToEntity()
        {
            var meeting = new Meeting
            {
                MeetingID = MeetingID,
                OwnerID = OwnerID,
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                IsAllDay = IsAllDay,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                RoomID = RoomID
            };

            return meeting;
        }
    }
}
