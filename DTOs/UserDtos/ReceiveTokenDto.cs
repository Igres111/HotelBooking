namespace HotelBooking.DTOs.UserDtos
{
    public class ReceiveTokenDto
    {
        public Guid Id { get; set; }
        public string AccsessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
