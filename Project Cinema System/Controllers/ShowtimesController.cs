using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.ViewModels;

namespace Project_Cinema_System.Controllers
{
    public class ShowtimesController : Controller
    {
        private readonly AppDbContext _context;

        public ShowtimesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Showtimes
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            var showtimes = await _context.Showtimes
                .AsNoTracking()
                .Where(s => s.Status == "Scheduled" && s.DateTime > now)
                .OrderBy(s => s.DateTime)
                .Select(s => new ShowtimeListItem
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie.Name,
                    CinemaName = s.Cinema.Name,
                    DateTime = s.DateTime,
                    Price = s.Price,
                    TotalTickets = s.TotalTickets
                })
                .ToListAsync();

            var ids = showtimes.Select(s => s.Id).ToList();

            // Confirmed and Pending bookings both hold tickets.
            var booked = await _context.Bookings
                .Where(b => ids.Contains(b.ShowtimeId) &&
                            (b.Status == "Confirmed" || b.Status == "Pending"))
                .GroupBy(b => b.ShowtimeId)
                .Select(g => new
                {
                    ShowtimeId = g.Key,
                    Quantity = g.Sum(b => b.Quantity)
                })
                .ToDictionaryAsync(x => x.ShowtimeId, x => x.Quantity);

            foreach (var s in showtimes)
            {
                var taken = booked.TryGetValue(s.Id, out var q) ? q : 0;
                s.AvailableTickets = Math.Max(0, s.TotalTickets - taken);
            }

            return View(showtimes);
        }
    }
}
