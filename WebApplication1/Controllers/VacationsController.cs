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
using Microsoft.CodeAnalysis;
using NToastNotify;
using WebApplication1.Models;
using System.Collections;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class VacationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public VacationsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification= toastNotification;
        }

        // GET: Vacations
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value.Equals("agency"))
            {
                var model = await _context.Vacation.Include(v => v.Destination)
                                .Where(a => a.IdentityUser.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                                .ToListAsync();
                return View(model);
            }
            else
            {
                var applicationDbContext = _context.Vacation.Include(v => v.Destination);
                return View(await applicationDbContext.ToListAsync());
            }
            
        }

        // GET: Vacations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vacation == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation
                .Include(v => v.Destination)
                .FirstOrDefaultAsync(m => m.VacationId == id);
            if (vacation == null)
            {
                return NotFound();
            }

            return View(vacation);
        }

        // GET: Vacations/Create
        [Authorize(Roles = "agency")]
        public IActionResult Create()
        {
            ViewData["DestinationId"] = new SelectList(_context.Destination, "DestinationId", "hotel");
            return View();
        }

        // POST: Vacations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Create([Bind("VacationId,startDate,endDate,price,DestinationId")] Vacation vacation)
        {
            var id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            vacation.IdentityUser =await _context.Users.FindAsync(id);
            vacation.Destination = await _context.Destination.FindAsync(vacation.DestinationId);
            DateTime now = DateTime.Now;
            int nowStart = DateTime.Compare(now, vacation.startDate);
            int startEnd = DateTime.Compare(vacation.startDate, vacation.endDate);

            if (nowStart >= 0)
            {
                ViewData["DestinationId"] = new SelectList(_context.Destination, "DestinationId", "hotel", vacation.DestinationId);
                _toastNotification.AddErrorToastMessage("Start Date needs to be later than now!");
                return View(vacation);
            }

            if (startEnd >= 0)
            {
                ViewData["DestinationId"] = new SelectList(_context.Destination, "DestinationId", "hotel", vacation.DestinationId);
                _toastNotification.AddErrorToastMessage("End date needs to be later than start date!");
                return View(vacation);
            }
            
            _context.Add(vacation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
          
        }

        // GET: Vacations/Edit/5
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vacation == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation.FindAsync(id);
            if (vacation == null)
            {
                return NotFound();
            }
            ViewData["DestinationId"] = new SelectList(_context.Destination, "DestinationId", "hotel", vacation.DestinationId);
            return View(vacation);
        }

        // POST: Vacations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Edit(int id, [Bind("VacationId,startDate,endDate,price,DestinationId")] Vacation vacation)
        {
            if (id != vacation.VacationId)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(vacation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacationExists(vacation.VacationId))
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

        // GET: Vacations/Delete/5
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vacation == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacation
                .Include(v => v.Destination)
                .FirstOrDefaultAsync(m => m.VacationId == id);
            if (vacation == null)
            {
                return NotFound();
            }

            return View(vacation);
        }

        // POST: Vacations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "agency")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vacation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vacation'  is null.");
            }
            var vacation = await _context.Vacation.FindAsync(id);
            if (vacation != null)
            {
                _context.Vacation.Remove(vacation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Vacations/BookVacation/5
        [Authorize(Roles = "client")]
        public async Task<IActionResult> BookVacation(int? id)
        {
            if (id == null)
            {
                _toastNotification.AddErrorToastMessage("Could not book vacation!");
                return RedirectToAction(nameof(Index));
            }
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var existBookings = await _context.Bookings
                .FirstOrDefaultAsync(m => m.IdentityUser.Id == idUser & m.VacationId == id);

            if (existBookings == null)
            {
                Bookings bookings = new Bookings();

                bookings.IdentityUser = await _context.Users.FindAsync(idUser);
                bookings.Vacation = await _context.Vacation.FindAsync(id);
                bookings.VacationId = id.Value;
                bookings.Accepted = ACCEPTED.PROCESSING;

                _context.Add(bookings);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("You have booked a vacation!");
                return RedirectToAction(nameof(Index));
            } else
            {
                _toastNotification.AddWarningToastMessage("You already booked this vacation!");
                return RedirectToAction(nameof(Index));
            }

        }

        // GET: Vacations/addToFavorite/5
        [Authorize(Roles = "client")]
        public async Task<IActionResult> addToFavorite(int? id)
        {
            if (id == null)
            {
                _toastNotification.AddErrorToastMessage("Could not add to favrovite the vacation!");
                return RedirectToAction(nameof(Index));
            }
            var idUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var existFav = await _context.Favorites
                .FirstOrDefaultAsync(m => m.IdentityUser.Id == idUser & m.VacationId == id);

            if (existFav == null)
            {
                Favorites favorites = new Favorites();

                favorites.IdentityUser = await _context.Users.FindAsync(idUser);
                favorites.Vacation = await _context.Vacation.FindAsync(id);
                favorites.VacationId = id.Value;

                _context.Add(favorites);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("You have added to favorite the vacation!");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _toastNotification.AddWarningToastMessage("You already added to favorite this vacation!");
                return RedirectToAction(nameof(Index));
            }

        }

        private bool VacationExists(int id)
        {
          return _context.Vacation.Any(e => e.VacationId == id);
        }
    }
}
