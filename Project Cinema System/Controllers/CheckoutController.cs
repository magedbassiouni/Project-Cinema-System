using System.Data;
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
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CartService _cartService;
        private readonly PromotionService _promotionService;

        public CheckoutController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            CartService cartService,
            PromotionService promotionService)
        {
            _context = context;
            _userManager = userManager;
            _cartService = cartService;
            _promotionService = promotionService;
        }

        // GET: Checkout
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var items = await _cartService.GetItemsAsync(userId);

            if (!items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            return View(new CheckoutViewModel { Items = items });
        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var userId = _userManager.GetUserId(User)!;

            if (model.PaymentMethod != "Cash" && model.PaymentMethod != "Online")
                ModelState.AddModelError(
                    nameof(model.PaymentMethod), "Select a payment method.");

            if (!ModelState.IsValid)
                return await ReloadAsync(model, userId);

            await using var tx = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Showtime)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // A second click (double submit) finds an empty cart,
                // so the same cart can never be paid twice.
                if (cart == null || !cart.Items.Any())
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }

                var items = cart.Items.ToList();

                foreach (var item in items)
                {
                    var showtime = item.Showtime;

                    if (showtime.Status != "Scheduled" ||
                        showtime.DateTime <= DateTime.Now)
                    {
                        await tx.RollbackAsync();
                        TempData["Error"] =
                            "A showtime in your cart is no longer available. Please remove it.";
                        return RedirectToAction("Index", "Cart");
                    }

                    var available =
                        await _cartService.GetAvailableTicketsAsync(showtime.Id);

                    if (item.Quantity > available)
                    {
                        await tx.RollbackAsync();
                        TempData["Error"] =
                            $"Only {available} tickets are left for one of your showtimes.";
                        return RedirectToAction("Index", "Cart");
                    }
                }

                var subtotal = items.Sum(i => i.Showtime.Price * i.Quantity);
                decimal discount = 0;
                Promotion? promotion = null;

                var code = model.PromotionCode?.Trim().ToUpperInvariant();

                if (!string.IsNullOrWhiteSpace(code))
                {
                    var result = await _promotionService
                        .ValidateAndCalculateDiscount(code, subtotal);

                    if (!result.Success)
                    {
                        ModelState.AddModelError(
                            nameof(model.PromotionCode), result.Message);

                        await tx.RollbackAsync();
                        return await ReloadAsync(model, userId);
                    }

                    promotion = await _context.Promotions
                        .FirstOrDefaultAsync(p => p.Code == code);

                    if (promotion == null ||
                        !promotion.IsActive ||
                        promotion.UsageCount >= promotion.MaxUsage)
                    {
                        ModelState.AddModelError(
                            nameof(model.PromotionCode),
                            "This promotion is no longer available.");

                        await tx.RollbackAsync();
                        return await ReloadAsync(model, userId);
                    }

                    discount = Math.Round(
                        result.Discount, 2, MidpointRounding.AwayFromZero);
                }

                var isOnline = model.PaymentMethod == "Online";
                var orderRef =
                    $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

                decimal allocated = 0;
                var bookings = new List<Booking>();

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var lineTotal = item.Showtime.Price * item.Quantity;
                    decimal lineDiscount = 0;

                    // Split the promotion discount across the bookings.
                    // The last booking takes the remainder, so the sum is exact.
                    if (discount > 0)
                    {
                        lineDiscount = i == items.Count - 1
                            ? discount - allocated
                            : Math.Round(discount * lineTotal / subtotal, 2,
                                MidpointRounding.AwayFromZero);

                        allocated += lineDiscount;
                    }

                    var booking = new Booking
                    {
                        UserId = userId,
                        ShowtimeId = item.ShowtimeId,
                        BookingDate = DateTime.UtcNow,
                        Quantity = item.Quantity,
                        UnitPrice = item.Showtime.Price,
                        Status = isOnline ? "Confirmed" : "Pending",
                        PromotionId = promotion?.Id,
                        DiscountAmount = lineDiscount
                    };

                    _context.Bookings.Add(booking);
                    bookings.Add(booking);

                    _context.Payments.Add(new Payment
                    {
                        Booking = booking,
                        Amount = lineTotal - lineDiscount,
                        Method = model.PaymentMethod,
                        Status = isOnline ? "Paid" : "Pending",
                        TransactionReference = $"{orderRef}-{i + 1}",
                        CreatedAt = DateTime.UtcNow,
                        PaidAt = isOnline ? DateTime.UtcNow : null
                    });
                }

                if (promotion != null)
                {
                    promotion.UsageCount++;

                    _context.PromotionUsages.Add(new PromotionUsage
                    {
                        PromotionId = promotion.Id,
                        Booking = bookings[0],
                        UserId = userId,
                        DiscountAmount = discount,
                        UsedAt = DateTime.UtcNow
                    });
                }

                _context.CartItems.RemoveRange(items);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = isOnline
                    ? "Payment successful and your booking is confirmed!"
                    : "Your booking is reserved. Please pay in cash at the cinema.";

                return RedirectToAction("MyBookings", "Bookings");
            }
            catch
            {
                await tx.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "An error occurred while processing your order.");

                return await ReloadAsync(model, userId);
            }
        }

        private async Task<IActionResult> ReloadAsync(
            CheckoutViewModel model, string userId)
        {
            model.Items = await _cartService.GetItemsAsync(userId);
            return View(model);
        }
    }
}