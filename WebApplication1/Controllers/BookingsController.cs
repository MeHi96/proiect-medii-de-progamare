using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using travelling.agency.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NToastNotify;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public BookingsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value.Equals("agency"))
            {
                var model = await _context.Bookings.Include(b => b.Vacation.Destination).Include(b => b.IdentityUser)
                                .Where(a => a.Vacation.IdentityUser.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                                .ToListAsync();
                return View(model);
            }
            else
            {
                var applicationDbContext = _context.Bookings.Include(b => b.Vacation.Destination);
                return View(await applicationDbContext.ToListAsync());
            }
            
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Vacation.Destination)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }


        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                return NotFound();
            }
            ViewData["VacationId"] = new SelectList(_context.Vacation, "VacationId", "VacationId", bookings.VacationId);
            return View(bookings);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingsId,VacationId")] Bookings bookings)
        {
            if (id != bookings.BookingsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingsExists(bookings.BookingsId))
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
            ViewData["VacationId"] = new SelectList(_context.Vacation, "VacationId", "VacationId", bookings.VacationId);
            return View(bookings);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Vacation.Destination)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bookings'  is null.");
            }
            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings != null)
            {
                _context.Bookings.Remove(bookings);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Accept/5
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Accept(int? id)
        {
            if (id == null)
            {
                _toastNotification.AddErrorToastMessage("Could not accept this vacation!");
                return RedirectToAction(nameof(Index));
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                _toastNotification.AddErrorToastMessage("Could not accept this vacation!");
                return RedirectToAction(nameof(Index));
            } else
            {
                bookings.Accepted = ACCEPTED.ACCEPTED;
                _context.Update(bookings);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Booking accepted!");
                return RedirectToAction(nameof(Index));

            }
        }

        // GET: Bookings/Reject/5
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Reject(int? id)
        {

            if (id == null)
            {
                _toastNotification.AddErrorToastMessage("Could not reject this vacation!");
                return RedirectToAction(nameof(Index));
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                _toastNotification.AddErrorToastMessage("Could not reject this vacation!");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                bookings.Accepted = ACCEPTED.REJECTED;
                _context.Update(bookings);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Booking rejected!");
                return RedirectToAction(nameof(Index));

            }
        }
        private bool BookingsExists(int id)
        {
          return _context.Bookings.Any(e => e.BookingsId == id);
        }
    }
}
