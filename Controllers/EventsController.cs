using ASP_Ticket_Center.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASP_Ticket_Center.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            var events = _context.Events.AsQueryable();

            if (categoryId.HasValue)
            {
                events = events.Where(e => e.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                events = events.Where(e => e.Name.Contains(searchTerm));
            }

            ViewBag.Categories = _context.Categories.ToList();

            return View(await events.ToListAsync());
        }

        public IActionResult Buy(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
            {
                return NotFound();
            }

            return View(ev);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Buy(int id, int quantity)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            if (quantity <= 0)
            {
                ModelState.AddModelError(nameof(quantity), "Невалидно количество.");
                return View(ev);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            await _context.Entry(ev).ReloadAsync();
            if (quantity > ev.Capacity)
            {
                ModelState.AddModelError(nameof(quantity), "Няма достатъчно налични билети.");
                return View(ev);
            }

            // Create one Ticket + Reservation per unit (keeps FK integrity)
            var now = DateTime.UtcNow;
            for (var i = 0; i < quantity; i++)
            {
                var ticket = new Ticket
                {
                    EventId = ev.Id,
                    CategoryId = ev.CategoryId,
                    QRCode = Guid.NewGuid().ToString(),
                    RegisterDate = now,
                    Seat = string.Empty, // DB column not-null — provide value or make nullable in model+migration
                    Line = string.Empty
                };

                var reservation = new Reservation
                {
                    ClientId = userId,
                    Quantity = 1,
                    RegisterDate = now,
                    Tickets = ticket // link via navigation so EF inserts Ticket then Reservation correctly
                };

                _context.Tickets.Add(ticket);
                _context.Reservations.Add(reservation);
            }

            ev.Capacity -= quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Грешка при запис. Моля опитайте отново.");
                return View(ev);
            }

            TempData["Success"] = "Резервацията е направена успешно.";
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //public async Task<IActionResult> Buy(int id, int quantity)
        //{
        //    var ev = await _context.Events.FindAsync(id);
        //    if (ev == null)
        //        return NotFound();

        //    if (quantity <= 0)
        //    {
        //        ModelState.AddModelError(nameof(quantity), "Невалидно количество.");
        //        return View(ev);
        //    }

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        // Challenge will redirect to the login page when using cookie/auth schemes
        //        return Challenge();
        //    }

        //    // Reload the entity to reduce chances of acting on stale capacity
        //    await _context.Entry(ev).ReloadAsync();

        //    if (quantity > ev.Capacity)
        //    {
        //        ModelState.AddModelError(nameof(quantity), "Няма достатъчно налични билети.");
        //        return View(ev);
        //    }

        //    ev.Capacity -= quantity;

        //    var reservation = new Reservation
        //    {
        //        ClientId = userId,
        //        TicketId = ev.Id,
        //        Quantity = quantity,
        //        RegisterDate = DateTime.Now
        //    };

        //    _context.Reservations.Add(reservation);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        ModelState.AddModelError(string.Empty, "Възникна конфликт при запис. Моля опитайте отново.");
        //        return View(ev);
        //    }

        //    TempData["Success"] = "Резервацията е направена успешно.";
        //    return RedirectToAction(nameof(Index));
        //}
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(а => а.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId,Location,Description,ImageURL,Organizer,Capacity,Date,MaxPrice,MinPrice,Status")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Last_Update = DateTime.Now;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Location,Description,ImageURL,Organizer,Capacity,Date,MaxPrice,MinPrice,Status,Last_Update")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", @event.CategoryId);
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(а => а.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}