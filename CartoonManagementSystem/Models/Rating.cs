using System.ComponentModel.DataAnnotations;

namespace CartoonManagementSystem.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; }

        public int CartoonId { get; set; }
        public virtual Cartoon? Cartoon { get; set; }

        public string UserId { get; set; } 
    }
}