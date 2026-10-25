
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Controllers
{
    public class CinemaController : Controller
    {
        private readonly AppDbContext _context;

        public CinemaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .AsNoTracking()
                .ToListAsync();

            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
                return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema)
        {
            if (id != cinema.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(cinema);

            var existingCinema = await _context.Cinemas.FindAsync(id);

            if (existingCinema == null)
                return NotFound();

            existingCinema.Name = cinema.Name;
            existingCinema.Description = cinema.Description;
            existingCinema.Address = cinema.Address;
            existingCinema.Img = cinema.Img;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
                return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
                return NotFound();

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
