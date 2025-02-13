namespace HotelBooking.DTOs.ReviewDtos
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }
        public float Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
