using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CartoonManagementSystem.Data;
using CartoonManagementSystem.Models;
using System.Security.Claims;

namespace CartoonManagementSystem.Controllers
{
    public class CartoonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CartoonController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        //Search & Filter integrated into Index
        public async Task<IActionResult> Index(string searchString, string genreFilter)
        {
            var cartoons = _context.Cartoons.Include(c => c.Ratings).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                cartoons = cartoons.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(genreFilter))
            {
                cartoons = cartoons.Where(x => x.Genre == genreFilter);
            }

            ViewBag.Genres = await _context.Cartoons.Select(c => c.Genre).Distinct().ToListAsync();
            return View(await cartoons.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var cartoon = await _context.Cartoons.Include(c => c.Ratings).FirstOrDefaultAsync(m => m.Id == id);
            if (cartoon == null) return NotFound();
            return View(cartoon);
        }

        //Only Admin can create/edit/delete
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cartoon cartoon, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;

                    // Generate clean, isolated tracking names
                    string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
                    string extension = Path.GetExtension(imageFile.FileName);
                    string fileName = uniqueId + extension;

                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Stores ONLY the clean string identifier token name ("abc12345.png")
                    cartoon.ImagePath = fileName;
                }
                else
                {
                    cartoon.ImagePath = "default.jpg"; // Baseline fallback standard asset
                }

                _context.Add(cartoon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartoon);
        }

        //Rating System 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(int cartoonId, int stars)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.CartoonId == cartoonId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Stars = stars;
            }
            else
            {
                _context.Ratings.Add(new Rating { CartoonId = cartoonId, UserId = userId, Stars = stars });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = cartoonId });
        }
        //ADMIN DELETE METHOD EXECUTION 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cartoon = await _context.Cartoons.FindAsync(id);
            if (cartoon == null)
            {
                return NotFound();
            }

            // Delete the physical image asset from wwwroot/images to save hosting space
            if (!string.IsNullOrEmpty(cartoon.ImagePath) && cartoon.ImagePath != "default.jpg")
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", cartoon.ImagePath);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Cartoons.Remove(cartoon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}