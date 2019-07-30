using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingManagement.Models;
using Microsoft.AspNetCore.Http;
using BookingManagement.Services;
using BookingManagement.Models.ViewModels;

namespace BookingManagement.Controllers
{
    public class AccountsController : Controller
    {
        private readonly BookingDbContext _context;
        private ISession Session { get; set; }
        private readonly ISecurityService securityService;
        private string email { get; set; }

        public AccountsController(IHttpContextAccessor httpContextAccessor, ISecurityService service)
        {
            Session = httpContextAccessor.HttpContext.Session;
            _context = new BookingDbContext();
            securityService = service;

            email = Session.GetString("account");
        }

        public IActionResult GotoLoginPage()
        {
            return RedirectToAction("Index", "Accounts");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string email, string password)
        {
            var accountExist = _context.Attendees.SingleOrDefault(a => a.Email == email);

            if (accountExist != null)
            {
                if (password == securityService.Decrypt(accountExist.Password, accountExist.Salt))
                {
                    Session.SetString("account", accountExist.Email);
                    return RedirectToAction("Index", "Booking");
                }
                else
                    return View(nameof(Index));
            }

            return NotFound();
        }

        public IActionResult ChangePassword(int? id)
        {
            var attendee = _context.Attendees.SingleOrDefault(a => a.AttendeeID == id);
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                AttendeeID = attendee.AttendeeID
            };

            ViewBag.Email = email;
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword(int? id, [Bind("AttendeeID, CurrentPassword", "NewPassword", "ConfirmPassword")]ChangePasswordViewModel model)
        {
            if (id != model.AttendeeID)
                return NotFound();

            if (ModelState.IsValid)
            {
                var attendee = _context.Attendees.SingleOrDefault(a => a.AttendeeID == model.AttendeeID);
                if (model.CurrentPassword == securityService.Decrypt(attendee.Password, attendee.Salt))
                {
                    attendee.Password = securityService.Encrypt(model.NewPassword, attendee.Salt);
                }

                _context.Update(attendee);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Booking");
        }

        public IActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}
