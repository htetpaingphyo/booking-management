using System.Collections.Generic;

namespace BookingManagement.Models
{
    public class MeetingRooms
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }

        private HashSet<MeetingRooms> meetingRooms;

        public MeetingRooms()
        {
            meetingRooms = new HashSet<MeetingRooms>();
        }

        public HashSet<MeetingRooms> GetRooms()
        {
            meetingRooms.Add(new MeetingRooms { RoomID = 1, RoomName = "Lashio" });
            meetingRooms.Add(new MeetingRooms { RoomID = 2, RoomName = "Bagan" });
            meetingRooms.Add(new MeetingRooms { RoomID = 3, RoomName = "Dawai" });
            meetingRooms.Add(new MeetingRooms { RoomID = 4, RoomName = "Pathein" });
            meetingRooms.Add(new MeetingRooms { RoomID = 5, RoomName = "Myitkyina" });

            return meetingRooms;
        }
    }
}
