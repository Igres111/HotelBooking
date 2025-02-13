namespace HotelBooking.DTOs.ReviewDtos
{
    public class ReviewForUserDto
    {
        public List<ReviewDto> Review { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
    }
}
