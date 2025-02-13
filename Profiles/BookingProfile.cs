using AutoMapper;
using HotelBooking.DTOs.BookingDtos;
using HotelBooking.Models;

namespace HotelBooking.Profiles
{
    public class BookingProfile :Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingInfoDto, BookingInfo>().ReverseMap();
        }
    }
}
