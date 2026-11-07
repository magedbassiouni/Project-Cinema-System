
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Cinema_System.Models;
using Project_Cinema_System.ViewModels.Admin;

namespace Project_Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/Users
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Select(user => new UserManagementViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    IsLocked = user.LockoutEnd != null &&
                               user.LockoutEnd > DateTimeOffset.UtcNow
                })
                .ToList();

            return View(users);
        }

        // POST: Admin/Users/Lock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Prevent admin from locking his own account
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "You cannot lock your own account.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.SetLockoutEnabledAsync(user, true);

            var result = await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.MaxValue
            );

            if (result.Succeeded)
                TempData["Success"] = "User locked successfully.";
            else
                TempData["Error"] = "Failed to lock user.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/Unlock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var result = await _userManager.SetLockoutEndDateAsync(
                user,
                null
            );

            if (result.Succeeded)
                TempData["Success"] = "User unlocked successfully.";
            else
                TempData["Error"] = "Failed to unlock user.";

            return RedirectToAction(nameof(Index));
        }
    }
}
