using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.HotelRepo
{
    public interface IHotelRepository
    {
        public Task RegisterHotel(RegisterHotelDto hotel);
        public Task<List<Hotel>> GetHotelsList();
        //public Task BookHotel();
    }
}
