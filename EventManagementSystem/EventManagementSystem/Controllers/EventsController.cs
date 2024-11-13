using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EventManagementSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
 
        }
        public IActionResult Index()
        {
            var eventsModel = _context.Events.ToList();
            return View(eventsModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
            ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
            ViewData["Locations"] = new SelectList(_context.Locations, "Id", "Name");
            ViewData["Speakers"] = new SelectList(_context.Speakers, "Id", "Surname");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventModel)
        {
            var location = await _context.Locations.FindAsync(eventModel.LocationId);
            if (location != null)
            {
                eventModel.Location = location;
            }

            _context.Add(eventModel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Speakers) 
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventModel == null)
            {
                return NotFound();
            }

            ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
            ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
            ViewData["Locations"] = new SelectList(_context.Locations, "Id", "Name");
            ViewData["Speakers"] = new MultiSelectList(_context.Speakers, "Id", "Surname"); // MultiSelectList для підтримки вибору кількох спікерів

            return View(eventModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, Event eventModel)
        {
            ViewData["Formats"] = new SelectList(Enum.GetValues(typeof(Format)).Cast<Format>().Select(f => new { ID = (int)f, Name = f.ToString() }), "ID", "Name");
            ViewData["Categories"] = new SelectList(Enum.GetValues(typeof(Category)).Cast<Category>().Select(c => new { ID = (int)c, Name = c.ToString() }), "ID", "Name");
            ViewData["Locations"] = new SelectList(_context.Locations, "Id", "Name");
            ViewData["Speakers"] = new MultiSelectList(_context.Speakers, "Id", "Surname");
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            
                try
                {
                var existingEvent = await _context.Events
        .Include(e => e.Location)
        .Include(e => e.Speakers) 
        .Include(e => e.Tickets)
        .FirstOrDefaultAsync(e => e.Id == id);

                if (existingEvent != null)
                    {
                    existingEvent.Title = eventModel.Title;
                    existingEvent.Location = eventModel.Location;
                    existingEvent.Category = eventModel.Category;
                    existingEvent.Format = eventModel.Format;
                    existingEvent.Date = eventModel.Date;
                    existingEvent.Description = eventModel.Description;
                    existingEvent.Price = eventModel.Price;

                    existingEvent.Speakers.Clear();
                    if (eventModel.Speakers != null && eventModel.Speakers.Any())
                    {
                        foreach (var speakerId in eventModel.Speakers.Select(s => s.Id))
                        {
                            var speaker = await _context.Speakers.FindAsync(speakerId);
                            if (speaker != null)
                            {
                                existingEvent.Speakers.Add(speaker);
                            }
                        }
                    }

                   

                    _context.Update(existingEvent);
                    await _context.SaveChangesAsync();
                }
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                        throw;
                    
                }
                return RedirectToAction(nameof(Index));
            

           

            return View(eventModel);
        }

    }
}
