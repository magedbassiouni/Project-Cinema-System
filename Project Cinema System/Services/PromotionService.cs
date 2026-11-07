
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Services
{
    public class PromotionService
    {
        private readonly AppDbContext _context;

        public PromotionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, decimal Discount)>
            ValidateAndCalculateDiscount(string code, decimal totalAmount)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code);

            if (promotion == null)
                return (false, "Invalid promotion code.", 0);

            if (!promotion.IsActive)
                return (false, "This promotion is inactive.", 0);

            var now = DateTime.UtcNow;

            if (now < promotion.StartDate || now > promotion.EndDate)
                return (false, "This promotion has expired or is not active yet.", 0);

            if (promotion.UsageCount >= promotion.MaxUsage)
                return (false, "Promotion usage limit has been reached.", 0);

            if (totalAmount <= 0)
                return (false, "Invalid booking amount.", 0);

            if (promotion.DiscountPercentage <= 0 ||
                promotion.DiscountPercentage > 100)
                return (false, "Invalid discount percentage.", 0);

            var discount = totalAmount *
                           promotion.DiscountPercentage / 100;

            return (true, "Promotion applied successfully.", discount);
        }
    }
}
