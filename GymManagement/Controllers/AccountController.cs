using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GymManagement.BLL.ViewModels;
using GymManagement.Controllers;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
          ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Invalid Login", "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Invalid Login", "Invalid email or password.");
                return View(model);

            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out: {Email}", model.Email);
                ModelState.AddModelError("Locked Out", "Your account has been locked out. Please contact the administrator.");
                return View(model);
            }
            else
            {
                _logger.LogInformation("User logged in successfully: {Email}", model.Email);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

    }
}