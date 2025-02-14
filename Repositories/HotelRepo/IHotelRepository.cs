using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.HotelRepo
{
    public interface IHotelRepository
    {
        public Task RegisterHotel(RegisterHotelDto hotel);
        public Task<List<GetHotelDto>> GetHotelsList();
        public Task<List<GetHotelDto>> GetHotelById(Guid id);
        public Task UpdateHotel(Guid id, HotelChangesDto hotel);
        public Task DeleteHotel(Guid id);
    }
}
