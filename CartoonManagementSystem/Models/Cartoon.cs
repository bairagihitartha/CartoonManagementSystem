using System.ComponentModel.DataAnnotations;

namespace CartoonManagementSystem.Models
{
    public class Cartoon
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Genre { get; set; }

        public string? ImagePath { get; set; }

        // Navigation property for ratings
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        // Helper property to calculate average rating
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Stars) : 0.0;
    }
}