using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CartoonManagementSystem.Data;
using CartoonManagementSystem.Models;

namespace CartoonManagementSystem.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the ApplicationDbContext to communicate with your SQL database
        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Simple DTO class matching the client-side fetch payload
        public class ChatMessageRequest
        {
            public string Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Json(new { response = "Please type something before sending!" });
            }

            string userMessage = request.Message.ToLower().Trim();
            string botResponse;

            // 🟢 Pulling live database entries using your exact DbSet name 'Cartoons'
            // We include the 'Ratings' table relation so that '.AverageRating' can calculate accurately!
            var databaseCartoonsList = await _context.Cartoons
                .Include(c => c.Ratings)
                .OrderByDescending(c => c.Id)
                .Take(5)
                .ToListAsync();

            // Simple rule-based logic to respond about your cartoons!
            if (userMessage.Contains("hello") || userMessage.Contains("hi"))
            {
                botResponse = "👋 Hello! I'm your Toon AI Chatbot. Ask me about your favorite cartoons!";
            }
            else if (userMessage.Contains("tom") || userMessage.Contains("jerry"))
            {
                botResponse = "🐱🐭 **Tom & Jerry** is a legendary slapstick comedy about an endless chase!";
            }
            else if (userMessage.Contains("oggy") || userMessage.Contains("cockroaches"))
            {
                botResponse = "🐱🪳 **Oggy & the Cockroaches** is a brilliant comedy! Poor Oggy just wants to relax, but Dee Dee, Marky, and Joey won't let him.";
            }
            else if (userMessage.Contains("ninja") || userMessage.Contains("hattori"))
            {
                botResponse = "🥷 **Ninja Hattori** follows Kanzo Hattori running around helping Kenichi Mitsuba with everyday chores and problems!";
            }
            else if (userMessage.Contains("roll no 21") || userMessage.Contains("kanishk") || userMessage.Contains("kris"))
            {
                botResponse = "🔥 **Roll No 21** features Kris, an intelligent incarnation of Lord Krishna, who constantly uses his wit and mystical powers to outsmart the evil principal Kanishk!";
            }
            // 🎯 Dynamic block listing newly added records using '.Name' and '.AverageRating'
            else if (userMessage.Contains("list") || userMessage.Contains("cartoons") || userMessage.Contains("total") || userMessage.Contains("new") || userMessage.Contains("latest"))
            {
                if (databaseCartoonsList != null && databaseCartoonsList.Any())
                {
                    botResponse = "🎬 **Here are the latest cartoons added to the project database:**\n\n" +
                                  string.Join("\n", databaseCartoonsList.Select(x => $"• **{x.Name}** [{x.Genre}] - Rated ⭐ {x.AverageRating:F1}/5"));
                }
                else
                {
                    botResponse = "📭 Your database cartoon catalog is currently empty! Try adding some entries through the administration portal panel.";
                }
            }
            else
            {
                // 🔍 Dynamic Lookup: Scans the database records using '.Name' matching
                var foundCartoon = databaseCartoonsList.FirstOrDefault(x => x.Name != null && userMessage.Contains(x.Name.ToLower()));

                if (foundCartoon != null)
                {
                    botResponse = $"🔍 **Found it in your project database!**\n\n📺 **Title:** {foundCartoon.Name}\n🎭 **Genre:** {foundCartoon.Genre}\n⭐ **Rating:** {foundCartoon.AverageRating:F1}/5\n📝 **Description:** {foundCartoon.Description}";
                }
                else
                {
                    botResponse = "🤖 That sounds fascinating! I am still learning more details about this cartoon. Try asking about 'Tom & Jerry' or 'Oggy'!";
                }
            }

            return Json(new { response = botResponse });
        }
    }
}