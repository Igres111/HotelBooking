using AutoMapper;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;
namespace HotelBooking.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<Hotel, GetHotelDto>().ReverseMap();
        }
    }
}
