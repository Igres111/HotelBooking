using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.BookingDtos;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

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
            _context.BookingInfos.Add(billing);

            UserForHotel user = new UserForHotel { HotelId = info.HotelId, UserId = info.UserId };
            _context.UserForHotels.Add(user);

            await _context.SaveChangesAsync();
            return  info;
        }
        public async Task<ReceiveBookingDto> GetBookingById(Guid id)
        {
            var result =await _context.BookingInfos.FirstOrDefaultAsync(x =>x.Id == id);
            if (result == null)
            {
                throw new Exception("Booking not found");
            }
            return _mapper.Map<ReceiveBookingDto>(result);
        }
        public async Task<ReceiveBookingDto> GetBookingByUser(Guid userId)
        {
            var result = await _context.BookingInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if (result == null)
            {
                throw new Exception("Booking not found");
            }
            return _mapper.Map<ReceiveBookingDto>(result);
        }
        public async Task DeleteBooking(Guid id)
        {
            var booking = await _context.BookingInfos.FirstOrDefaultAsync(x => x.Id == id);
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            _context.BookingInfos.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
