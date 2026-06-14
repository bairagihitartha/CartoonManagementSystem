using System.ComponentModel.DataAnnotations;

namespace CartoonManagementSystem.Models
{
    public class Account
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } 

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        public bool RememberMe { get; set; }
    }
}