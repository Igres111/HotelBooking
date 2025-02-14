using AutoMapper;
using HotelBooking.DTOs.HotelDtos;
using HotelBooking.Models;
namespace HotelBooking.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<Hotel, RegisterHotelDto>().ReverseMap();
            CreateMap<Hotel, GetHotelDto>().ReverseMap();
            CreateMap<Hotel, HotelChangesDto>().ReverseMap();
        }
    }
}
