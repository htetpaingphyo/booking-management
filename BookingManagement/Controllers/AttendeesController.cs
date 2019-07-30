using BookingManagement.Models.ViewModels;
using BookingManagement.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookingManagement.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly AttendeeService _attendeeService;
        private ISession Session { get; set; }
        private string Email { get; set; }

        public AttendeesController(IHttpContextAccessor httpContextAccessor)
        {
            _attendeeService = new AttendeeService();
            Session = httpContextAccessor.HttpContext.Session;
            Email = Session.GetString("account");
        }

        public IActionResult GotoLoginPage()
        {
            return RedirectToAction("Index", "Accounts");
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(Email))
                return GotoLoginPage();
            else
            {
                ViewBag.Email = Email;
                return View();
            }
        }

        public virtual JsonResult Read_Attendees([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_attendeeService.GetAll().ToDataSourceResult(request));
        }

        public ActionResult Create_Attendees([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AttendeeViewModel> attendees)
        {
            var results = new List<AttendeeViewModel>();
            if (attendees != null && ModelState.IsValid)
            {
                foreach (var attendee in attendees)
                {
                    _attendeeService.Insert(attendee, ModelState);
                    results.Add(attendee);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_Attendees([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AttendeeViewModel> attendees)
        {
            if (attendees != null && ModelState.IsValid)
            {
                foreach (var attendee in attendees)
                {
                    _attendeeService.Update(attendee, ModelState);
                }
            }

            return Json(attendees.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Destroy_Attendees([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AttendeeViewModel> attendees)
        {
            if (attendees != null && ModelState.IsValid)
            {
                foreach (var attendee in attendees)
                {
                    _attendeeService.Delete(attendee, ModelState);
                }
            }

            return Json(attendees.ToDataSourceResult(request, ModelState));
        }
    }
}