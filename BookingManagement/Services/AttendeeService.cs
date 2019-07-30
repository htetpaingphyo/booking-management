using BookingManagement.Extensions;
using BookingManagement.Models;
using BookingManagement.Models.ViewModels;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BookingManagement.Services
{
    public class AttendeeService : ISchedulerEventService<AttendeeViewModel>
    {
        private BookingDbContext db;
        private ISession Session { get; set; }
        private const string SESSION_KEY = "Attendees";

        public AttendeeService(BookingDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            Session = httpContextAccessor.HttpContext.Session;
            db = context;
        }

        public AttendeeService() : this(new BookingDbContext(), new HttpContextAccessor())
        {
            //...
        }

        public IQueryable<AttendeeViewModel> GetAll()
        {
            return GetAllAttendees().AsQueryable();
        }

        public void Insert(AttendeeViewModel attendee, ModelStateDictionary modelState)
        {
            bool isSaved = true;

            if (!modelState.IsValid)
                modelState.AddModelError("error", "Invalid attendee.");

            // Save in database
            using (db)
            {
                var _attendee = attendee.ToEntity();

                _attendee.Salt = new SecurityService().GetUniqueString;
                _attendee.Password = new SecurityService().Encrypt("Welcome@123", _attendee.Salt);

                db.Attendees.Add(_attendee);
                isSaved = db.SaveChanges() > 0;
            }

            // Save in session
            if (isSaved)
            {
                var attendees = GetAllAttendees();
                attendee.AttendeeID = attendees.OrderByDescending(a => a.AttendeeID).First().AttendeeID + 1;
                attendees.Insert(0, attendee);
                Session.SetObjectAsJson(SESSION_KEY, attendees);
            }
        }

        public void Update(AttendeeViewModel attendee, ModelStateDictionary modelState)
        {
            bool isSaved = true;

            using (db)
            {
                var orig = db.Attendees.SingleOrDefault(a => a.AttendeeID == attendee.AttendeeID);

                // Update in database
                if (orig != null)
                {
                    orig.Name = attendee.Name;
                    orig.Email = attendee.Email;
                    orig.Password = attendee.Password;
                    orig.Salt = attendee.Salt;
                    orig.Color = attendee.Color;

                    db.Update(orig);
                    isSaved = db.SaveChanges() > 0;
                }

                // Update in session
                if (isSaved)
                {
                    var attendees = GetAllAttendees();
                    var origOnSession = attendees.Where(a => a.AttendeeID == attendee.AttendeeID).FirstOrDefault();

                    if (origOnSession != null)
                    {
                        origOnSession.Name = attendee.Name;
                        origOnSession.Email = attendee.Email;
                        origOnSession.Password = attendee.Password;
                        origOnSession.Salt = attendee.Salt;
                        origOnSession.Color = attendee.Color;

                        Session.SetObjectAsJson(SESSION_KEY, attendees);
                    }
                }
            }
        }

        public void Delete(AttendeeViewModel attendee, ModelStateDictionary modelState)
        {
            bool isDeleted = true;

            using (db)
            {
                var _attendee = db.Attendees.SingleOrDefault(a => a.AttendeeID == attendee.AttendeeID);

                if (_attendee != null)
                {
                    db.Remove(_attendee);
                    isDeleted = db.SaveChanges() > 0;
                }

                if (isDeleted)
                {
                    // Delete just from Session
                    var attendees = GetAllAttendees();
                    var orig = attendees.Where(a => a.AttendeeID == attendee.AttendeeID).FirstOrDefault();

                    if (orig != null)
                    {
                        attendees.Remove(orig);
                        Session.SetObjectAsJson(SESSION_KEY, attendees);
                    }
                }
            }
        }

        public IList<AttendeeViewModel> GetAllAttendees()
        {
            using (db)
            {
                var result = Session.GetObjectFromJson<IList<AttendeeViewModel>>("Attendees");

                if (result == null)
                {
                    result = db.Attendees
                        .Select(atd => new AttendeeViewModel
                        {
                            AttendeeID = atd.AttendeeID,
                            Name = atd.Name,
                            Email = atd.Email,
                            Password = atd.Password,
                            Salt = atd.Salt,
                            Color = atd.Color
                        }).ToList();

                    Session.SetObjectAsJson(SESSION_KEY, result);
                }

                return result;
            }
        }

        public AttendeeViewModel GetAttendeeById(int id)
        {
            var result = db.Attendees
                .Where(attendee => attendee.AttendeeID == id)
                .Select(attendee => new AttendeeViewModel
                {
                    AttendeeID = attendee.AttendeeID,
                    Name = attendee.Name,
                    Email = attendee.Email,
                    Password = attendee.Password,
                    Salt = attendee.Salt,
                    Color = attendee.Color
                })
                .SingleOrDefault();

            return result;
        }

        public int GetAttendeeIdByEmail(string email)
        {
            return db.Attendees.SingleOrDefault(a => a.Email == email).AttendeeID;
        }

        public AttendeeViewModel GetAttendeeByEmail(string email)
        {
            var result = db.Attendees
                .Where(attendee => attendee.Email == email)
                .Select(attendee => new AttendeeViewModel
                {
                    AttendeeID = attendee.AttendeeID,
                    Name = attendee.Name,
                    Email = attendee.Email,
                    Password = attendee.Password,
                    Salt = attendee.Salt,
                    Color = attendee.Color
                })
                .SingleOrDefault();

            return result;
        }
    }
}
