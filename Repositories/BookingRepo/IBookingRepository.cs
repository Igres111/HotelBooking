using HotelBooking.DTOs.BookingDtos;

namespace HotelBooking.Repositories.BookingRepo
{
    public interface IBookingRepository
    {
        public Task<BookingInfoDto> BookHotel(BookingInfoDto info);
    }
}
