
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Cinema_System.Models;
using Project_Cinema_System.Services;
using Project_Cinema_System.ViewModels;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Project_Cinema_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        // Register - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Register - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Create User
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(
                user, model.Password);

            if (result.Succeeded)
            {
                // 2. Assign Role
                var roleResult = await _userManager.AddToRoleAsync(
                    user, "User");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);

                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError(
                            string.Empty, error.Description);

                    return View(model);
                }

                // Generate Email Confirmation Token
                var token = await _userManager
                    .GenerateEmailConfirmationTokenAsync(user);

                // Encode Token for URL
                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                // Generate Confirmation Link
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    protocol: Request.Scheme
                );

                // 5. Prepare Email Body
                var emailBody = $"""
            <h2>Welcome to Cinema System</h2>
            <p>Please confirm your email by clicking the link below:</p>
            <a href="{HtmlEncoder.Default.Encode(confirmationLink)}">
                Confirm Email
            </a>
            """;

                // 6. Send Confirmation Email
                await _emailService.SendEmailAsync(
                    model.Email,
                    "Confirm Your Email - Cinema System",
                    emailBody
                );

                // 7. Redirect to Login
                TempData["RegistrationMessage"] =
                    "Account created successfully. Please check your email to confirm your account.";

                return RedirectToAction(nameof(Login));
            }

            // Registration Failed
            foreach (var error in result.Errors)
                ModelState.AddModelError(
                    string.Empty, error.Description);

            return View(model);
        }

        // Confirm Email - GET
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(
            string userId,
            string token)
        {
            // 1. Validate parameters
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid confirmation link.");
            }

            // 2. Find user
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            // 3. Decode Token
            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                return BadRequest("Invalid confirmation token.");
            }

            // 4. Confirm Email
            var result = await _userManager.ConfirmEmailAsync(
                user, decodedToken);

            if (result.Succeeded)
            {
                TempData["ConfirmationMessage"] =
                    "Your email has been confirmed successfully. You can now login.";

                return RedirectToAction(nameof(Login));
            }

            TempData["ConfirmationMessage"] =
                "Email confirmation failed. The link may be invalid or expired.";

            return RedirectToAction(nameof(Login));
        }

        // Resend Email Confirmation - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        // Resend Email Confirmation - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(
            ResendEmailConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Find User by Email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // 2. Check if User exists and Email is not confirmed
            if (user != null &&
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                // 3. Generate New Token
                var token = await _userManager
                    .GenerateEmailConfirmationTokenAsync(user);

                // 4. Encode Token
                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                // 5. Generate Confirmation Link
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new
                    {
                        userId = user.Id,
                        token = encodedToken
                    },
                    protocol: Request.Scheme
                );

                // 6. Send Email
                if (!string.IsNullOrEmpty(confirmationLink))
                {
                    var emailBody = $"""
                <h2>Cinema System</h2>
                <p>Please confirm your email using the link below:</p>
                <a href="{HtmlEncoder.Default.Encode(confirmationLink)}">
                    Confirm Email
                </a>
                """;

                    await _emailService.SendEmailAsync(
                        user.Email!,
                        "Resend Email Confirmation - Cinema System",
                        emailBody
                    );
                }
            }

            // 7. Generic Response
            TempData["ResendConfirmationMessage"] =
                "If an account exists with this email and is not confirmed, a confirmation email has been sent.";

            return RedirectToAction(nameof(ResendEmailConfirmation));
        }

        // Forgot Password - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot Password - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null &&
                await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager
                    .GeneratePasswordResetTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    protocol: Request.Scheme
                );

                if (!string.IsNullOrEmpty(resetLink))
                {
                    var emailBody = $"""
                <h2>Cinema System</h2>
                <p>Click below to reset your password:</p>
                <a href="{HtmlEncoder.Default.Encode(resetLink)}">
                    Reset Password
                </a>
                """;

                    await _emailService.SendEmailAsync(
                        user.Email!,
                        "Reset Password - Cinema System",
                        emailBody
                    );
                }
            }

            TempData["ForgotPasswordMessage"] =
                "If an eligible account exists, a password reset email has been sent.";

            return RedirectToAction(nameof(ForgotPassword));
        }

        // Reset Password - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(
            string userId,
            string token)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid reset link.");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        // Reset Password - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty, "Invalid password reset request.");

                return View(model);
            }

            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(model.Token));
            }
            catch (FormatException)
            {
                ModelState.AddModelError(
                    string.Empty, "Invalid or expired reset link.");

                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.Password
            );

            if (result.Succeeded)
            {
                TempData["ResetPasswordMessage"] =
                    "Password reset successfully. Please login.";

                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(
                    string.Empty, error.Description);

            return View(model);
        }

        // Login - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Login - POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(
                string.Empty, "Invalid login attempt.");

            return View(model);
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
