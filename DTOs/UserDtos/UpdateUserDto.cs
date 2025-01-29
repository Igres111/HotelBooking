using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        [Required]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public string? Email { get; set; } = string.Empty;
        [Required]
        public string? Password { get; set; } = string.Empty;
        [Required]
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
