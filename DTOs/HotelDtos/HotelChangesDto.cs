using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs.HotelDtos
{
    public class HotelChangesDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Address { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price per night must be a non-negative value.")]
        public float PricePerNight { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive integer.")]
        public int Capacity { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
