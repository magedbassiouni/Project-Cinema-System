
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;
using Project_Cinema_System.ViewModels.Admin;

namespace Project_Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShowtimesController : Controller
    {
        private readonly AppDbContext _context;

        public ShowtimesController(AppDbContext context)
        {
            _context = context;
        }

        private async Task LoadDropdowns(ShowtimeViewModel model)
        {
            model.Movies = await _context.Movies
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                })
                .ToListAsync();

            model.Cinemas = await _context.Cinemas
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        // GET: Admin/Showtimes
        public async Task<IActionResult> Index()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .OrderBy(s => s.DateTime)
                .ToListAsync();

            return View(showtimes);
        }

        // GET: Admin/Showtimes/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ShowtimeViewModel
            {
                DateTime = DateTime.Now.AddHours(1),
                Status = "Scheduled"
            };

            await LoadDropdowns(model);

            return View(model);
        }

        // POST: Admin/Showtimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShowtimeViewModel model)
        {
            if (model.DateTime <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.DateTime),
                    "Showtime must be in the future.");
            }

            if (!await _context.Movies.AnyAsync(m => m.Id == model.MovieId))
            {
                ModelState.AddModelError(
                    nameof(model.MovieId),
                    "Invalid movie.");
            }

            if (!await _context.Cinemas.AnyAsync(c => c.Id == model.CinemaId))
            {
                ModelState.AddModelError(
                    nameof(model.CinemaId),
                    "Invalid cinema.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value != null)
                    .SelectMany(x => x.Value!.Errors.Select(e =>
                        $"{x.Key}: {e.ErrorMessage}"));

                ViewBag.DebugErrors = string.Join(" | ", errors);

                await LoadDropdowns(model);

                return View(model);
            }

            var showtime = new Showtime
            {
                MovieId = model.MovieId,
                CinemaId = model.CinemaId,
                DateTime = model.DateTime,
                Price = model.Price,
                TotalTickets = model.TotalTickets,
                Status = model.Status
            };

            try
            {
                _context.Showtimes.Add(showtime);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Showtime created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Database error: Could not save the showtime.");

                await LoadDropdowns(model);

                return View(model);
            }
        }

        // GET: Admin/Showtimes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);

            if (showtime == null)
                return NotFound();

            var model = new ShowtimeViewModel
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                CinemaId = showtime.CinemaId,
                DateTime = showtime.DateTime,
                Price = showtime.Price,
                TotalTickets = showtime.TotalTickets,
                Status = showtime.Status
            };

            await LoadDropdowns(model);

            return View(model);
        }

        // POST: Admin/Showtimes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ShowtimeViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (model.DateTime <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.DateTime),
                    "Showtime must be in the future.");
            }

            if (!await _context.Movies.AnyAsync(m => m.Id == model.MovieId))
            {
                ModelState.AddModelError(
                    nameof(model.MovieId),
                    "Invalid movie.");
            }

            if (!await _context.Cinemas.AnyAsync(c => c.Id == model.CinemaId))
            {
                ModelState.AddModelError(
                    nameof(model.CinemaId),
                    "Invalid cinema.");
            }

            var showtime = await _context.Showtimes.FindAsync(id);

            if (showtime == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value != null)
                    .SelectMany(x => x.Value!.Errors.Select(e =>
                        $"{x.Key}: {e.ErrorMessage}"));

                ViewBag.DebugErrors = string.Join(" | ", errors);

                await LoadDropdowns(model);

                return View(model);
            }

            showtime.MovieId = model.MovieId;
            showtime.CinemaId = model.CinemaId;
            showtime.DateTime = model.DateTime;
            showtime.Price = model.Price;
            showtime.TotalTickets = model.TotalTickets;
            showtime.Status = model.Status;

            try
            {
                await _context.SaveChangesAsync();

                TempData["Success"] = "Showtime updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Database error: Could not update the showtime.");

                await LoadDropdowns(model);

                return View(model);
            }
        }
    }
}
