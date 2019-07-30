using BookingManagement.Models;
using BookingManagement.Models.ViewModels;
using BookingManagement.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Linq;

namespace BookingManagement.Controllers
{
    public class BookingController : Controller
    {
        private MeetingService _meetingService;
        private AttendeeService _attendeeService;
        private ISession Session { get; set; }
        private ISecurityService service;

        public string email { get; set; }

        public BookingController(IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
        {
            Session = httpContextAccessor.HttpContext.Session;

            _meetingService = new MeetingService();
            _attendeeService = new AttendeeService();
            service = securityService;

            email = Session.GetString("account");
        }

        public IActionResult GotoLoginPage()
        {
            return RedirectToAction("Index", "Accounts");
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(email))
                return GotoLoginPage();
            else
            {
                ViewBag.Email = email;
                ViewBag.UserId = _attendeeService.GetAttendeeIdByEmail(email);
                return View(new MeetingViewModel());
            }
        }

        public virtual JsonResult Meetings_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_meetingService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult Meetings_Destroy([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            {
                var currentOwnerId = _attendeeService.GetAttendeeIdByEmail(email);
                var meetingOwnerId = _meetingService.GetAll().SingleOrDefault(m => m.MeetingID == meeting.MeetingID).OwnerID;

                if (currentOwnerId == meetingOwnerId)
                {
                    _meetingService.Delete(meeting, ModelState);

                    foreach (var attendeeId in meeting.Attendees.ToList())
                    {
                        var recepient = _attendeeService.GetAttendeeById(attendeeId);
                        var sender = _attendeeService.GetAttendeeByEmail(email);

                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(sender.Name, sender.Email));
                        message.To.Add(new MailboxAddress(recepient.Name, recepient.Email));
                        message.Subject = "Meeting Cancelled.";
                        message.Body = new TextPart(TextFormat.Html)
                        {
                            Text = $"Hi {recepient.Name}, " +
                            $"{sender.Name} cancelled the meeting of {new MeetingRooms().GetRooms().SingleOrDefault(m => m.RoomID == meeting.RoomID).RoomName} meeting room."
                        };

                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.office365.com", 587, false);
                            client.Authenticate(sender.Email, service.Decrypt(sender.Password, sender.Salt));
                            client.Send(message);
                            client.Disconnect(true);
                        }
                    }
                }
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Meetings_Create([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            {
                if (!_meetingService.IsExisted(meeting))
                {
                    meeting.OwnerID = _attendeeService.GetAttendeeIdByEmail(email);
                    meeting.StartTimezone = "Asia/Rangoon";
                    meeting.EndTimezone = "Asia/Rangoon";
                    _meetingService.Insert(meeting, ModelState);

                    foreach (var attendeeId in meeting.Attendees.ToList())
                    {
                        var recepient = _attendeeService.GetAttendeeById(attendeeId);
                        var sender = _attendeeService.GetAttendeeByEmail(email);

                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(sender.Name, sender.Email));
                        message.To.Add(new MailboxAddress(recepient.Name, recepient.Email));
                        message.Subject = "Meeting Invitation";
                        message.Body = new TextPart(TextFormat.Html)
                        {
                            Text = $"Hi {recepient.Name}, " +
                            $"{sender.Name} invite you to join meeting at {new MeetingRooms().GetRooms().SingleOrDefault(m => m.RoomID == meeting.RoomID).RoomName} meeting room - {meeting.Start.ToString("dd-MMM-yyyy hh:mm tt")}."
                        };

                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.office365.com", 587, false);
                            client.Authenticate(sender.Email, service.Decrypt(sender.Password, sender.Salt));
                            client.Send(message);
                            client.Disconnect(true);
                        }
                    }
                }
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Meetings_Update([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            {
                var currentOwnerId = _attendeeService.GetAttendeeIdByEmail(email);
                var meetingOwnerId = _meetingService.GetAll().SingleOrDefault(m => m.MeetingID == meeting.MeetingID).OwnerID;

                if (currentOwnerId == meetingOwnerId)
                {
                    _meetingService.Update(meeting, ModelState);

                    foreach (var attendeeId in meeting.Attendees.ToList())
                    {
                        var recepient = _attendeeService.GetAttendeeById(attendeeId);
                        var sender = _attendeeService.GetAttendeeByEmail(email);

                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(sender.Name, sender.Email));
                        message.To.Add(new MailboxAddress(recepient.Name, recepient.Email));
                        message.Subject = "Meeting Invitation (Updated)";
                        message.Body = new TextPart(TextFormat.Html)
                        {
                            Text = $"Hi {recepient.Name}, " +
                            $"{sender.Name} invite you to join meeting at {new MeetingRooms().GetRooms().SingleOrDefault(m => m.RoomID == meeting.RoomID).RoomName} meeting room - {meeting.Start.ToString("dd-MMM-yyyy hh:mm tt")}."
                        };

                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.office365.com", 587, false);
                            client.Authenticate(sender.Email, service.Decrypt(sender.Password, sender.Salt));
                            client.Send(message);
                            client.Disconnect(true);
                        }
                    }
                }
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Read_Attendees([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_attendeeService.GetAll().ToDataSourceResult(request));
        }
    }
}