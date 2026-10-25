
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Controllers
{
    public class ActorController : Controller
    {
        private readonly AppDbContext _context;

        public ActorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors
                .AsNoTracking()
                .ToListAsync();

            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor)
        {
            if (!ModelState.IsValid)
                return View(actor);

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor input)
        {
            if (id != input.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(input);

            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            actor.Name = input.Name;
            actor.Img = input.Img;
            actor.Biography = input.Biography;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return NotFound();

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
