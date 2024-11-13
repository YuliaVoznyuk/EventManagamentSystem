using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventManagementSystem.Controllers
{
    public class UserEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //public IActionResult Index()
        //{
        //    ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
        //    ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Search(string searchTerm, string format, string category, DateTime? startDate, DateTime? endDate)
        //{
        //    var events = from e in _context.Events.Include(e => e.Location)
        //                 select e;

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        events = events.Where(e => e.Title.Contains(searchTerm));
        //    }

        //    if (!string.IsNullOrEmpty(format) && Enum.TryParse(format, out Format formatEnum))
        //    {
        //        events = events.Where(e => e.Format == formatEnum);
        //    }

        //    if (!string.IsNullOrEmpty(category) && Enum.TryParse(category, out Category categoryEnum))
        //    {
        //        events = events.Where(e => e.Category == categoryEnum);
        //    }

        //    if (startDate.HasValue)
        //    {
        //        events = events.Where(e => e.StartDateTime >= startDate.Value);
        //    }

        //    if (endDate.HasValue)
        //    {
        //        events = events.Where(e => e.EndDateTime <= endDate.Value);
        //    }

        //    ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
        //    ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
        //    ViewData["Speakers"] = new MultiSelectList(await _context.Speakers.ToListAsync(), "Id", "Surname");

        //    return View(await events.ToListAsync());
        //}
        public async Task<IActionResult> Index(string searchTerm, string format, string category, DateTime? date)
        {
            var events = from e in _context.Events.Include(e => e.Location)
                         select e;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                events = events.Where(e => e.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(format) && Enum.TryParse(format, out Format formatEnum))
            {
                events = events.Where(e => e.Format == formatEnum);
            }

            if (!string.IsNullOrEmpty(category) && Enum.TryParse(category, out Category categoryEnum))
            {
                events = events.Where(e => e.Category == categoryEnum);
            }

            if (date.HasValue)
            {
                events = events.Where(e => e.Date >= date.Value);
            }

           

            ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
            ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
            ViewData["Speakers"] = new MultiSelectList(await _context.Speakers.ToListAsync(), "Id", "Surname");

            return View(events);
        }

        [HttpGet]
        public IActionResult Search(string searchTerm, string format, string category, DateTime? date)
        {
            return RedirectToAction("Index", new { searchTerm, format, category, date });
        }
        [HttpGet]

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .Include(e => e.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View("Details", eventItem);
        }
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> RegisterForEvent(int eventId)
        {
            var user = await _userManager.GetUserAsync(User);
            var eventToRegister = await _context.Events
                                                .Include(e => e.Tickets)
                                                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventToRegister == null)
            {
                TempData["Error"] = "Подія не знайдена.";
                return RedirectToAction("Index", "Home");
            }

            if (eventToRegister.AvailableSeats <= 0)
            {
                TempData["Error"] = "Немає доступних місць для цієї події.";
                return RedirectToAction("Details", new { id = eventId });
            }
            var existingTicket = await _context.Tickets
                                      .FirstOrDefaultAsync(t => t.EventId == eventId && t.UserId == user.Id);
            if (existingTicket != null)
            {
                TempData["Error"] = "Ви вже зареєстровані на цю подію.";
                return RedirectToAction("Details", new { id = eventId });
            }


            var ticket = new Ticket
            {
                EventId = eventToRegister.Id,
                UserId = user.Id,
            };

            eventToRegister.AvailableSeats -= 1; 
            await _context.Tickets.AddAsync(ticket);
            _context.Events.Update(eventToRegister); 

       
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ви успішно зареєстровані на подію!";
            return RedirectToAction("Details", new { id = eventId });
        }
        [Authorize]
        public IActionResult ListEvents()
        {
            var model = new EventSearchViewModel
            {
                DateEvent = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ListEvents(EventSearchViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEvents = await _context.Events
                .Include(e => e.Tickets)
                .Include(e => e.Location) 
                .Where(e => e.Tickets.Any(t => t.UserId == userId) &&
                   e.Date.Date == model.DateEvent.Date)
        .ToListAsync();

            ViewBag.UserEvents = userEvents;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CancelRegistration(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.EventId == eventId && t.UserId == userId);

            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Ви успішно скасували реєстрацію на подію.";
            }
            else
            {
                TempData["Error"] = "Помилка скасування реєстрації або квиток не знайдено.";
            }

            return RedirectToAction("ListEvents");
        }
    }
}
