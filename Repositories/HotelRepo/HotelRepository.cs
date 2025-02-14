using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;


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
        public async Task RegisterHotel(RegisterHotelDto hotel)
        {
            var newHotel = _mapper.Map<Hotel>(hotel);
            newHotel.Id = Guid.NewGuid();
            await _context.Hotels.AddAsync(newHotel);
            await _context.SaveChangesAsync();
        }
        public async Task<List<GetHotelDto>> GetHotelsList()
        {
            var hotels = await _context.Hotels.ToListAsync();
            return _mapper.Map<List<GetHotelDto>>(hotels);
        }
        public async Task<List<GetHotelDto>> GetHotelById(Guid id)
        {
            var hotel = await _context.Hotels.Where(x => x.Id == id).ToListAsync();
            return _mapper.Map<List<GetHotelDto>>(hotel);
        }
        public async Task UpdateHotel(Guid id, HotelChangesDto hotel)
        {
            var found = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == id);
            if (found == null)
            {
                throw new Exception("Hotel not found");
            }
            _mapper.Map(hotel, found);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteHotel(Guid id)
        {
            var found = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == id);
            if (found == null)
            {
                throw new Exception("Hotel not found");
            }
            _context.Hotels.Remove(found);
            await _context.SaveChangesAsync();
        }
    }
}
