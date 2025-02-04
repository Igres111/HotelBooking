namespace HotelBooking.Models
{
    public class Hotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;                        
        public float PricePerNight { get; set; }  
        public float AvgRating { get; set; }
        public float StarReviews { get; set; }
        public int Capacity { get; set; } 
        public string Description { get; set; } = string.Empty;
        public List<UserForHotel> UserForHotels { get; set; }
        public List<Reviews> Reviews { get; set; }
    }
}
