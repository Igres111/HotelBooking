using HotelBooking.Models.BaseTypes;

namespace HotelBooking.Models.Entities
{
    public class IdempotencyRecord : BaseEntity
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string RequestHash { get; set; } = string.Empty;
        public string ResponseBody { get; set; } = string.Empty;
        public int ResponseStatusCode { get; set; }
    }
}
