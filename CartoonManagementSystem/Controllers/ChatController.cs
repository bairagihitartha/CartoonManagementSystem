using Microsoft.AspNetCore.Mvc;

namespace CartoonManagementSystem.Controllers
{
    public class ChatController : Controller
    {
        // Simple DTO class matching the client-side fetch payload
        public class ChatMessageRequest
        {
            public string Message { get; set; } 
        }

        [HttpPost]
        public IActionResult SendMessage([FromBody] ChatMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Json(new { response = "Please type something before sending!" });
            }

            string userMessage = request.Message.ToLower();
            string botResponse;

            // Simple rule-based logic to respond about your cartoons!
            if (userMessage.Contains("hello") || userMessage.Contains("hi"))
            {
                botResponse = "👋 Hello! I'm your Toon AI Chatbot. Ask me about your favorite cartoons!";
            }
            else if (userMessage.Contains("tom") || userMessage.Contains("jerry"))
            {
                botResponse = "🐱🐭 **Tom & Jerry** is a legendary slapstick comedy about an endless chase. Your catalog lists it with a rating of 0.0/5.0!";
            }
            else if (userMessage.Contains("oggy"))
            {
                botResponse = "🐱🪳 **Oggy & the Cockroaches** is a brilliant comedy! Poor Oggy just wants to relax, but Dee Dee, Marky, and Joey won't let him.";
            }
            else if (userMessage.Contains("ninja") || userMessage.Contains("hattori"))
            {
                botResponse = "🥷 **Ninja Hattori** follows Kanzo Hattori running around helping Kenichi Mitsuba with everyday chores and problems!";
            }
            else if (userMessage.Contains("list") || userMessage.Contains("cartoons") || userMessage.Contains("total"))
            {
                botResponse = "🎬 Your system currently tracks classic entries like Tom & Jerry, Oggy, Ninja Hattori, Roll No 21, and Honey Bunny!";
            }
            else
            {
                botResponse = "🤖 That sounds fascinating! I am still learning more details about this cartoon. Try asking about 'Tom & Jerry' or 'Oggy'!";
            }

            return Json(new { response = botResponse });
        }
    }
}