using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "User name is required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Passowrd is required")]
        public string? Password { get; set; }

        [RegularExpression(@"^(\+?\d{1,4}[\s-]?)?\(?\d{1,4}?\)?[\s-]?\d{1,4}[\s-]?\d{1,9}$|^$",
        ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
    }
}
