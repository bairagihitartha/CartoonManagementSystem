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
        public async Task<IActionResult> RegisterUser(Account model) => await ExecRegistration(model, "User");

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(Account model) => await ExecRegistration(model, "Admin");

        private async Task<IActionResult> ExecRegistration(Account model, string role)
        {
            if (!ModelState.IsValid) return View("Register", model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Cartoon");
            }

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