using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Reviews
    {
        public Guid Id { get; set; } 
        public Guid HotelId { get; set; } 
        public Guid UserId { get; set; } 
        public float Rating { get; set; } 
        public string? Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User{ get; set; }
        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; }
    }
}
