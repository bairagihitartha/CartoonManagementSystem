using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CartoonManagementSystem.Models;

namespace CartoonManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet] public IActionResult Register() => View(new Account());
        [HttpGet] public IActionResult Login() => View(new Account());

        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against cross-site request forgery
        public async Task<IActionResult> RegisterUser(Account model)
        {
            // If the incoming model fails basic validation (e.g., invalid email structure), return early
            if (!ModelState.IsValid) return View("Register", model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Hardcoded safety constraint: Public signups can ONLY be assigned the "User" role
                string targetRole = "User";

                if (!await _roleManager.RoleExistsAsync(targetRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(targetRole));
                }

                await _userManager.AddToRoleAsync(user, targetRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Cartoon");
            }

            // If Identity password/validation requirements fail, attach errors to UI
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View("Register", model);
        }

        [HttpPost] public async Task<IActionResult> LoginUser(Account model) => await ExecLogin(model, "User");
        [HttpPost] public async Task<IActionResult> LoginAdmin(Account model) => await ExecLogin(model, "Admin");

        private async Task<IActionResult> ExecLogin(Account model, string expectedRole)
        {
            if (!ModelState.IsValid) return View("Login", model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, expectedRole))
                {
                    ModelState.AddModelError("", $"Access Denied: Account is not explicitly verified inside '{expectedRole}'.");
                    return View("Login", model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) return RedirectToAction("Index", "Cartoon");
            }

            ModelState.AddModelError("", "Invalid sign-in credentials.");
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Cartoon");
        }
    }
}