using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.ViewModels;

namespace Project_Cinema_System.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        // Pending bookings hold tickets too, so they are counted.
        public async Task<int> GetAvailableTicketsAsync(int showtimeId)
        {
            var showtime = await _context.Showtimes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                return 0;

            var booked = await _context.Bookings
                .Where(b => b.ShowtimeId == showtimeId &&
                            (b.Status == "Confirmed" || b.Status == "Pending"))
                .SumAsync(b => (int?)b.Quantity) ?? 0;

            return Math.Max(0, showtime.TotalTickets - booked);
        }

        public async Task<List<CartItemViewModel>> GetItemsAsync(string userId)
        {
            return await _context.CartItems
                .AsNoTracking()
                .Where(i => i.Cart.UserId == userId)
                .Select(i => new CartItemViewModel
                {
                    ItemId = i.Id,
                    ShowtimeId = i.ShowtimeId,
                    MovieName = i.Showtime.Movie.Name,
                    CinemaName = i.Showtime.Cinema.Name,
                    ShowtimeDate = i.Showtime.DateTime,
                    UnitPrice = i.Showtime.Price,
                    Quantity = i.Quantity
                })
                .ToListAsync();
        }
    }
}