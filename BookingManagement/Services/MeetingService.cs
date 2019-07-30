using BookingManagement.Extensions;
using BookingManagement.Models;
using BookingManagement.Models.ViewModels;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingManagement.Services
{
    public class MeetingService : ISchedulerEventService<MeetingViewModel>
    {
        private readonly BookingDbContext db;
        // private bool UpdateDatabase = false;
        // private ISession Session { get; set; }

        public MeetingService()
        {
            db = new BookingDbContext();
            // Session = httpContextAccessor.HttpContext.Session;
        }

        public virtual IQueryable<MeetingViewModel> GetAll()
        {
            return GetAllMeetings().AsQueryable();
        }

        public virtual IList<MeetingViewModel> GetAllMeetings()
        {
            // var result = Session.GetObjectFromJson<IList<MeetingViewModel>>("meetings");

            var result = db.Meetings
                    .Include(model => model.MeetingAttendees)
                    .ToList()
                    .Select(meeting => new MeetingViewModel
                    {
                        MeetingID = meeting.MeetingID,
                        Title = meeting.Title,
                        Start = meeting.Start,
                        End = meeting.End,
                        StartTimezone = meeting.StartTimezone,
                        EndTimezone = meeting.EndTimezone,
                        Description = meeting.Description,
                        IsAllDay = meeting.IsAllDay,
                        RoomID = meeting.RoomID,
                        OwnerID = meeting.OwnerID,
                        RecurrenceRule = meeting.RecurrenceRule,
                        RecurrenceException = meeting.RecurrenceException,
                        RecurrenceID = meeting.RecurrenceID,
                        Attendees = meeting.MeetingAttendees.Select(m => m.AttendeeID).ToArray()
                    }).ToList();

            // Session.SetObjectAsJson("meetings", result);

            return result;
        }

        public virtual void Insert(MeetingViewModel meeting, ModelStateDictionary modelState)
        {
            if (ValidateModel(meeting, modelState))
            {
                using (db)
                {
                    if (meeting.Attendees == null)
                    {
                        meeting.Attendees = new int[0];
                    }

                    if (string.IsNullOrEmpty(meeting.Title))
                    {
                        meeting.Title = "";
                    }

                    var entity = meeting.ToEntity();

                    foreach (var attendeeId in meeting.Attendees)
                    {
                        entity.MeetingAttendees.Add(new MeetingAttendee
                        {
                            AttendeeID = attendeeId
                        });
                    }

                    db.Meetings.Add(entity);
                    db.SaveChanges();

                    meeting.MeetingID = entity.MeetingID;
                }
            }
        }

        public virtual void Update(MeetingViewModel meeting, ModelStateDictionary modelState)
        {
            if (ValidateModel(meeting, modelState))
            {
                if (string.IsNullOrEmpty(meeting.Title))
                {
                    meeting.Title = "";
                }

                var entity = db.Meetings.Include("MeetingAttendees").FirstOrDefault(m => m.MeetingID == meeting.MeetingID);

                entity.OwnerID = meeting.OwnerID;
                entity.Title = meeting.Title;
                entity.Start = meeting.Start;
                entity.End = meeting.End;
                entity.Description = meeting.Description;
                entity.IsAllDay = meeting.IsAllDay;
                entity.RoomID = meeting.RoomID;
                entity.RecurrenceID = meeting.RecurrenceID;
                entity.RecurrenceRule = meeting.RecurrenceRule;
                entity.RecurrenceException = meeting.RecurrenceException;
                entity.StartTimezone = meeting.StartTimezone;
                entity.EndTimezone = meeting.EndTimezone;

                foreach (var meetingAttendee in entity.MeetingAttendees.ToList())
                {
                    entity.MeetingAttendees.Remove(meetingAttendee);
                }

                if (meeting.Attendees != null)
                {
                    foreach (var attendeeId in meeting.Attendees)
                    {
                        var meetingAttendee = new MeetingAttendee
                        {
                            MeetingID = entity.MeetingID,
                            AttendeeID = attendeeId
                        };

                        entity.MeetingAttendees.Add(meetingAttendee);
                    }
                }

                db.SaveChanges();

                /** Error Code
                if (ValidateModel(meeting, modelState))
                {
                    using (db)
                    {
                        if (string.IsNullOrEmpty(meeting.Title))
                        {
                            meeting.Title = "";
                        }

                        var entity = meeting.ToEntity();
                        db.Meetings.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;

                        var oldMeeting = db.Meetings
                            .Include(model => model.MeetingAttendees)
                            .FirstOrDefault(m => m.MeetingID == meeting.MeetingID);

                        foreach (var attendee in oldMeeting.MeetingAttendees.ToList())
                        {
                            db.MeetingAttendees.Attach(attendee);

                            if (meeting.Attendees == null || !meeting.Attendees.Contains(attendee.AttendeeID))
                            {
                                db.Entry(attendee).State = EntityState.Deleted;
                            }
                            else
                            {
                                db.Entry(attendee).State = EntityState.Unchanged;

                                ((List<int>)meeting.Attendees).Remove(attendee.AttendeeID);
                            }

                            entity.MeetingAttendees.Add(attendee);
                        }

                        if (meeting.Attendees != null)
                        {
                            foreach (var attendeeId in meeting.Attendees)
                            {
                                var meetingAttendee = new MeetingAttendee
                                {
                                    MeetingID = entity.MeetingID,
                                    AttendeeID = attendeeId
                                };

                                db.MeetingAttendees.Attach(meetingAttendee);
                                db.Entry(meetingAttendee).State = EntityState.Added;

                                entity.MeetingAttendees.Add(meetingAttendee);
                            }
                        }

                        db.SaveChanges();
                    }
                    **/
            }
        }

        public virtual void Delete(MeetingViewModel meeting, ModelStateDictionary modelState)
        {
            using (db)
            {
                var meetingAttendees = db.MeetingAttendees.Where(m => m.MeetingID == meeting.MeetingID).ToList();

                foreach(var attendee in meetingAttendees)
                {
                    db.MeetingAttendees.Remove(attendee);
                    db.SaveChanges();
                }

                var _meeting = db.Meetings.SingleOrDefault(m => m.MeetingID == meeting.MeetingID);
                db.Meetings.Remove(_meeting);
                db.SaveChanges();
            }
        }

        public bool IsExisted(MeetingViewModel meeting)
        {
            return db.Meetings.SingleOrDefault(
                m => m.RoomID == meeting.RoomID &&
                m.Start == meeting.Start
            ) != null;
        }

        private bool ValidateModel(MeetingViewModel appointment, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid || (appointment.Start > appointment.End))
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }
    }
}
