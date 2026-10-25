
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _context;

        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Category)
                .AsNoTracking()
                .ToListAsync();

            return View(movies);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // GET: Movie/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (string.IsNullOrWhiteSpace(movie.Name))
                ModelState.AddModelError(
                    nameof(movie.Name), "Movie name is required.");

            if (string.IsNullOrWhiteSpace(movie.Status))
                ModelState.AddModelError(
                    nameof(movie.Status), "Status is required.");

            if (movie.CategoryId <= 0 ||
                !await _context.Categories
                    .AnyAsync(c => c.Id == movie.CategoryId))
            {
                ModelState.AddModelError(
                    nameof(movie.CategoryId), "Select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(movie.CategoryId);
                return View(movie);
            }

            if (movie.Price < 0)
                ModelState.AddModelError(nameof(movie.Price), "Price cannot be negative.");

            if (movie.DateTime == default)
                ModelState.AddModelError(nameof(movie.DateTime), "Date and time are required.");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(movie.CategoryId, movie.CinemaId);
                return View(movie);
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            await LoadCategoriesAsync(movie.CategoryId, movie.CinemaId);
            return View(movie);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Description,Status,MainImg,CategoryId")]
            Movie input)
        {
            if (id != input.Id)
                return NotFound();

            if (string.IsNullOrWhiteSpace(input.Name))
                ModelState.AddModelError(
                    nameof(input.Name), "Movie name is required.");

            if (string.IsNullOrWhiteSpace(input.Status))
                ModelState.AddModelError(
                    nameof(input.Status), "Status is required.");

            if (input.CategoryId <= 0 ||
                !await _context.Categories
                    .AnyAsync(c => c.Id == input.CategoryId))
            {
                ModelState.AddModelError(
                    nameof(input.CategoryId), "Select a valid category.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(input.CategoryId, input.CinemaId);
                return View(input);
            }

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            movie.Name = input.Name;
            movie.Description = input.Description;
            movie.Status = input.Status;
            movie.MainImg = input.MainImg;
            movie.CategoryId = input.CategoryId;
            movie.CinemaId = input.CinemaId;
            movie.Price = input.Price;
            movie.DateTime = input.DateTime;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            try
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Movie deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This movie may be linked to showtimes or bookings. " +
                    "Remove or reassign related records before deleting it.");

                return View("Delete", movie);
            }
        }


        private async Task LoadCategoriesAsync(
            int? selectedCategoryId = null,
            int? selectedCinemaId = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.CategoryId = new SelectList(
                categories, "Id", "Name", selectedCategoryId);

            var cinemas = await _context.Cinemas
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.CinemaId = new SelectList(
                cinemas, "Id", "Name", selectedCinemaId);
        }

    }
}
