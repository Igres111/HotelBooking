﻿namespace HotelBooking.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<UserForHotel> UserForHotels { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public List<Reviews> Reviews { get; set; }
        public List<BookingInfo> BillingInfos { get; set; }
    }
}
