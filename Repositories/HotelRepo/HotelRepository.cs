using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;


namespace HotelBooking.Repositories.HotelRepo
{
    public class HotelRepository : IHotelRepository
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public HotelRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task RegisterHotel(GetHotelDto hotel)
        {
            var newHotel = _mapper.Map<Hotel>(hotel);
            newHotel.Id = Guid.NewGuid();
            await _context.Hotels.AddAsync(newHotel);
            await _context.SaveChangesAsync();
        }
    }
}
