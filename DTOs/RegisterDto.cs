using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "The Email field must be a valid email address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,20}$",
    ErrorMessage = "The Password field must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "The Role field must be either 'Admin' or 'User'.")]
        public string Role { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(\+?\d{1,4}[\s\-]?)?(\(?\d{1,4}\)?[\s\-]?)?\d{1,4}[\s\-]?\d{1,4}[\s\-]?\d{1,4}$",
        ErrorMessage = "The Phone number format is invalid.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
