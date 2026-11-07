using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;
using Project_Cinema_System.Services;
using Project_Cinema_System.ViewModels;

namespace Project_Cinema_System.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const int MaxPerItem = 10;

        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CartService _cartService;

        public CartController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            CartService cartService)
        {
            _context = context;
            _userManager = userManager;
            _cartService = cartService;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            var model = new CartViewModel
            {
                Items = await _cartService.GetItemsAsync(userId)
            };

            return View(model);
        }

        // POST: Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int showtimeId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User)!;

            if (quantity < 1)
                quantity = 1;

            var showtime = await _context.Showtimes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                return NotFound();

            var backToMovie = RedirectToAction(
                "Details", "Movie", new { id = showtime.MovieId });

            if (showtime.Status != "Scheduled" ||
                showtime.DateTime <= DateTime.Now)
            {
                TempData["Error"] = "This showtime is not available.";
                return backToMovie;
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var item = cart.Items.FirstOrDefault(i => i.ShowtimeId == showtimeId);
            var newQuantity = (item?.Quantity ?? 0) + quantity;

            if (newQuantity > MaxPerItem)
            {
                TempData["Error"] = $"You can book up to {MaxPerItem} tickets per showtime.";
                return backToMovie;
            }

            var available = await _cartService.GetAvailableTicketsAsync(showtimeId);

            if (newQuantity > available)
            {
                TempData["Error"] = $"Only {available} tickets are available.";
                return backToMovie;
            }

            if (item == null)
            {
                cart.Items.Add(new CartItem
                {
                    ShowtimeId = showtimeId,
                    Quantity = newQuantity
                });
            }
            else
            {
                item.Quantity = newQuantity;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Added to your cart.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int itemId, int quantity)
        {
            var userId = _userManager.GetUserId(User)!;

            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.UserId == userId);

            if (item == null)
                return NotFound();

            if (quantity < 1)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                if (quantity > MaxPerItem)
                {
                    TempData["Error"] = $"You can book up to {MaxPerItem} tickets per showtime.";
                    return RedirectToAction(nameof(Index));
                }

                var available = await _cartService.GetAvailableTicketsAsync(item.ShowtimeId);

                if (quantity > available)
                {
                    TempData["Error"] = $"Only {available} tickets are available.";
                    return RedirectToAction(nameof(Index));
                }

                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int itemId)
        {
            var userId = _userManager.GetUserId(User)!;

            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Cart.UserId == userId);

            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}