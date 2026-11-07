
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;
using Project_Cinema_System.ViewModels.Admin;

namespace Project_Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly AppDbContext _context;

        public PromotionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions
                .OrderByDescending(p => p.Id)
                .Select(p => new PromotionViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    DiscountPercentage = p.DiscountPercentage,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    MaxUsage = p.MaxUsage,
                    UsageCount = p.UsageCount,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return View(promotions);
        }

        public IActionResult Create()
        {
            return View(new PromotionViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionViewModel model)
        {
            model.Code = model.Code.Trim().ToUpperInvariant();

            if (model.StartDate > model.EndDate)
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "End date must be after start date.");

            if (await _context.Promotions.AnyAsync(p => p.Code == model.Code))
                ModelState.AddModelError(
                    nameof(model.Code),
                    "This promotion code already exists.");

            if (!ModelState.IsValid)
                return View(model);

            var promotion = new Promotion
            {
                Code = model.Code,
                DiscountPercentage = model.DiscountPercentage,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MaxUsage = model.MaxUsage,
                UsageCount = 0,
                IsActive = model.IsActive
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Promotion created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);

            if (promotion == null)
                return NotFound();

            var model = new PromotionViewModel
            {
                Id = promotion.Id,
                Code = promotion.Code,
                DiscountPercentage = promotion.DiscountPercentage,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                MaxUsage = promotion.MaxUsage,
                UsageCount = promotion.UsageCount,
                IsActive = promotion.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, PromotionViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            model.Code = model.Code.Trim().ToUpperInvariant();

            if (model.StartDate > model.EndDate)
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "End date must be after start date.");

            if (await _context.Promotions.AnyAsync(
                p => p.Code == model.Code && p.Id != id))
            {
                ModelState.AddModelError(
                    nameof(model.Code),
                    "This promotion code already exists.");
            }

            var promotion = await _context.Promotions.FindAsync(id);

            if (promotion == null)
                return NotFound();

            if (model.MaxUsage < promotion.UsageCount)
                ModelState.AddModelError(
                    nameof(model.MaxUsage),
                    "Maximum usage cannot be less than current usage.");

            if (!ModelState.IsValid)
            {
                model.UsageCount = promotion.UsageCount;
                return View(model);
            }

            promotion.Code = model.Code;
            promotion.DiscountPercentage = model.DiscountPercentage;
            promotion.StartDate = model.StartDate;
            promotion.EndDate = model.EndDate;
            promotion.MaxUsage = model.MaxUsage;
            promotion.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Promotion updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);

            if (promotion == null)
                return NotFound();

            promotion.IsActive = !promotion.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Promotion status updated.";

            return RedirectToAction(nameof(Index));
        }
    }
}
