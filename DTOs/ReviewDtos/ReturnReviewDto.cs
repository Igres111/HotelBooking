namespace HotelBooking.DTOs.ReviewDtos
{
    public class ReturnReviewDto
    {

    public Guid HotelId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public float PricePerNight { get; set; }
    public float AvgRating { get; set; }
    public float StarReviews { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; }
    public List<ReviewForUserDto> Reviews { get; set; }
    }
}
