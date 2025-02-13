using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.BookingDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.BookingRepo
{
    public class BookingRepository:IBookingRepository
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public BookingRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<BookingInfoDto> BookHotel(BookingInfoDto info)
        {
            BookingInfo billing =  _mapper.Map<BookingInfo>(info);
            billing.Id = Guid.NewGuid();
            UserForHotel user = new UserForHotel { HotelId = info.HotelId, UserId = info.UserId };
            _context.UserForHotels.Add(user);
            _context.BookingInfos.Add(billing);
            await _context.SaveChangesAsync();
            return  info;
        }
    }
}
